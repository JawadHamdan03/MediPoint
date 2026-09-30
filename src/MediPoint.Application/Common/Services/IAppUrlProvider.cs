namespace MediPoint.Application.Common.Services;

public interface IAppUrlProvider
{
    string BuildPasswordResetUrl(string role, string token);
}
