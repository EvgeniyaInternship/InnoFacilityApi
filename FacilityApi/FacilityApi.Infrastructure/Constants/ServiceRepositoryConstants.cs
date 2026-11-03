namespace FacilityApi.Infrastructure.Constants;

internal static class ServiceRepositoryConstants
{
    public const string Columns = "\"Id\", \"Title\", \"Description\", \"Price\", \"IsActive\", \"DoctorId\"";
    public const string InsertSql = """
        INSERT INTO "Services" ("Id", "Title", "Description", "Price", "IsActive", "DoctorId")
        VALUES (@Id, @Title, @Description, @Price, @IsActive, @DoctorId)
        """;
    public const string UpdateSql = """
        UPDATE "Services"
        SET "Title" = @Title,
            "Description" = @Description,
            "Price" = @Price,
            "IsActive" = @IsActive,
            "DoctorId" = @DoctorId
        WHERE "Id" = @Id
        """;
    public const string DeleteSql = """
        DELETE FROM "Services"
        WHERE "Id" = @Id
        """;
}
