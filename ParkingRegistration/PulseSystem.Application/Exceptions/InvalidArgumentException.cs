namespace PulseSystem.Application.Exceptions;

public class InvalidArgumentException : Exception
{
    public InvalidArgumentException(string argument)
    : base($"{argument} é inválido")
    {
        
    }
}