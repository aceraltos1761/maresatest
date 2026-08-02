namespace Maresa.Domain.Exceptions;

public class ClienteInvalidoException : Exception
{
    public ClienteInvalidoException(string message) : base(message)
    {
    }
}
