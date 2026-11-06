namespace FacilityApi.Domain.Exceptions;

public sealed class ConflictException(string message) : AppException(message);
