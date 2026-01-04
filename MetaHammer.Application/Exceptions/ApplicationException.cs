namespace MetaHammer.Application.Exceptions;

public abstract class ApplicationException : Exception
{
    public ApplicationException(string exception) : base(exception)
    {
    }
}