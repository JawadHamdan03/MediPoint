namespace MediPoint.Domain.Common.Results;

public record Error(string Id, ErrorType Type, string Description);
