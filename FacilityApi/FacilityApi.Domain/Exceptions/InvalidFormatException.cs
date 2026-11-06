namespace FacilityApi.Domain.Exceptions;

public sealed class InvalidFormatException(string message) : AppException(message);
