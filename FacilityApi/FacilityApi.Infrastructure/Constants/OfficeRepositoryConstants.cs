namespace FacilityApi.Infrastructure.Constants;

internal static class OfficeRepositoryConstants
{
    public const string Columns = "\"Id\", \"Title\"";
    public const string InsertSql = """
        INSERT INTO "Offices" ("Id", "Title")
        VALUES (@Id, @Title)
        """;
    public const string UpdateSql = """
        UPDATE "Offices"
        SET "Title" = @Title
        WHERE "Id" = @Id
        """;
    public const string DeleteSql = """
        DELETE FROM "Offices"
        WHERE "Id" = @Id
        """;
}
