namespace MediPoint.Domain.Common.Results;

public static class Errors
{
    public static Error NotFound { get; } = new("NotFound", ErrorType.NotFound, "Account not found.");
}