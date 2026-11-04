namespace FacilityApi.Application.Helpers;

public sealed class PaginationHelper
{
    public static int CalculateTotalPages(int totalCount, int pageSize)
    {
        return (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}
