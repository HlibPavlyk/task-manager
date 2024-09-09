namespace TaskManager.Domain.Exceptions;

public class NotFoundException : Exception
{
    // Constructor that accepts a custom error message.
    public NotFoundException(string message) : base(message) { }

    // Constructor that accepts a custom error message and an inner exception for more detailed error information.
    public NotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
}
