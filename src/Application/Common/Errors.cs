namespace Application;

public enum ErrorKind { Validation, NotFound, Conflict }

public class AppException(ErrorKind kind, string message) : Exception(message)
{
    public ErrorKind Kind { get; } = kind;
}