using Dapper;
using System.Linq.Expressions;

namespace FacilityApi.Infrastructure.Repositories;

internal static class SqlPredicateBuilder
{
    public static (string Where, DynamicParameters Parameters) Build<T>(Expression<Func<T, bool>> predicate)
    {
        var parameters = new DynamicParameters();
        var index = 0;
        var where = Visit(predicate.Body, predicate.Parameters[0], parameters, ref index);
        return (where, parameters);
    }

    public static string AppendPage(string sql, int pageNumber, int pageSize, DynamicParameters parameters)
    {
        if (pageNumber < 0 || pageSize <= 0)
            return sql;

        parameters.Add("offset", pageNumber * pageSize);
        parameters.Add("limit", pageSize);
        return sql + " ORDER BY \"Id\" OFFSET @offset LIMIT @limit";
    }

    private static string Visit(
        Expression expression,
        ParameterExpression parameter,
        DynamicParameters parameters,
        ref int index)
    {
        expression = Strip(expression);

        return expression switch
        {
            BinaryExpression binary => VisitBinary(binary, parameter, parameters, ref index),
            UnaryExpression { NodeType: ExpressionType.Not } unary =>
                $"NOT ({Visit(unary.Operand, parameter, parameters, ref index)})",
            MemberExpression member when IsColumn(member, parameter) => Quote(member.Member.Name),
            MethodCallExpression => throw NotSupported(expression),
            _ => ValueSql(expression, parameters, ref index)
        };
    }

    private static string VisitBinary(
        BinaryExpression binary,
        ParameterExpression parameter,
        DynamicParameters parameters,
        ref int index)
    {
        if (binary.NodeType is ExpressionType.AndAlso or ExpressionType.OrElse)
        {
            var logic = binary.NodeType == ExpressionType.AndAlso ? "AND" : "OR";
            var left = Visit(binary.Left, parameter, parameters, ref index);
            var right = Visit(binary.Right, parameter, parameters, ref index);
            return $"({left} {logic} {right})";
        }

        var op = binary.NodeType switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.NotEqual => "<>",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            _ => throw NotSupported(binary)
        };

        var leftSql = Visit(binary.Left, parameter, parameters, ref index);
        var rightSql = Visit(binary.Right, parameter, parameters, ref index);

        if (binary.NodeType is ExpressionType.Equal or ExpressionType.NotEqual)
        {
            if (rightSql == "NULL")
                return binary.NodeType == ExpressionType.Equal ? $"{leftSql} IS NULL" : $"{leftSql} IS NOT NULL";
            if (leftSql == "NULL")
                return binary.NodeType == ExpressionType.Equal ? $"{rightSql} IS NULL" : $"{rightSql} IS NOT NULL";
        }

        return $"({leftSql} {op} {rightSql})";
    }

    private static string ValueSql(Expression expression, DynamicParameters parameters, ref int index)
    {
        var value = Evaluate(expression);
        if (value is null)
            return "NULL";

        var name = $"p{index}";
        index++;
        parameters.Add(name, value);
        return "@" + name;
    }

    private static object? Evaluate(Expression expression)
    {
        try
        {
            var boxed = Expression.Convert(Strip(expression), typeof(object));
            return Expression.Lambda<Func<object?>>(boxed).Compile()();
        }
        catch (Exception exception)
        {
            throw NotSupported(expression, exception);
        }
    }

    private static Expression Strip(Expression expression)
    {
        while (expression is UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } unary)
            expression = unary.Operand;

        return expression;
    }

    private static bool IsColumn(MemberExpression member, ParameterExpression parameter)
        => member.Expression is not null && Strip(member.Expression) == parameter;

    private static string Quote(string name) => $"\"{name}\"";

    private static NotSupportedException NotSupported(Expression expression, Exception? innerException = null)
        => new($"Predicate '{expression}' cannot be translated to SQL.", innerException);
}
