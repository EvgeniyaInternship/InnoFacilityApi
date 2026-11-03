namespace FacilityApi.Infrastructure.Constants;

internal static class SpecializationRepositoryConstants
{
    public const string Columns = "\"Id\", \"Title\", \"Description\"";
    public const string InsertSql = """
        INSERT INTO "Specializations" ("Id", "Title", "Description")
        VALUES (@Id, @Title, @Description)
        """;
    public const string UpdateSql = """
        UPDATE "Specializations"
        SET "Title" = @Title,
            "Description" = @Description
        WHERE "Id" = @Id
        """;
    public const string DeleteSql = """
        DELETE FROM "Specializations"
        WHERE "Id" = @Id
        """;
}
