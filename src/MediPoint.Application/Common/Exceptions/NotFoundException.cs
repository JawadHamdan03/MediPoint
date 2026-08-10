namespace MediPoint.Application.Common.Exceptions;

public class NotFoundException(string entity,string Id):Exception($"the {entity} with '{Id}' not found")
{
}
