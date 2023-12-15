namespace Creative.Api.Exceptions;
public class ObjectNotFoundException : Exception
{
    private const string message = "Object could not be found in the database.";
    public ObjectNotFoundException(string message) : base(message) { }
    public ObjectNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    public ObjectNotFoundException(Exception innerException) : base(message, innerException) { }
    public ObjectNotFoundException() : base(message) { }
}