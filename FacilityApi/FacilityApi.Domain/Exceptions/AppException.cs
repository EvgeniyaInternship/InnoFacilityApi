namespace FacilityApi.Domain.Exceptions;

public abstract class AppException(string message, Exception? innerException = null) : Exception(message, innerException);
