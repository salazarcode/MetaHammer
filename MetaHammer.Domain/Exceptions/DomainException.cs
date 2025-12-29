namespace MetaHammer.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string errorMessage) : base(errorMessage)
    {
        
    }
}