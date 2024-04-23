namespace CommunicationsApp.Application.Common.Exceptions;

public class EntityNotFoundException : AppException
{
    public EntityNotFoundException(string entity) : base($"{entity} could not be found")
    {
    }
}
