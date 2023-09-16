namespace RedHerring.Clues;

public abstract class FailedToInitializeDefinitionException : Exception
{
    protected FailedToInitializeDefinitionException() : base()
    {
    }

    protected FailedToInitializeDefinitionException(string message) : base(message)
    {
    }

    protected FailedToInitializeDefinitionException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}