namespace Maresa.Domain.Exceptions;

public class ClienteValidacionException : Exception
{
    public ClienteValidacionException(string message) : base(message)
    {
    }

    public ClienteValidacionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
