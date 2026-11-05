namespace FacilityApi.Domain.Exceptions;

public sealed class ServerException(string message, Exception? innerException = null) : AppException(message, innerException);
