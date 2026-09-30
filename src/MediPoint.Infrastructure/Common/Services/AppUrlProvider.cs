using MediPoint.Application.Common.Services;
using MediPoint.Infrastructure.Common.Utils;
using Microsoft.Extensions.Options;

namespace MediPoint.Infrastructure.Common.Services;

public class AppUrlProvider(IOptions<FrontendSettings> options) : IAppUrlProvider
{
    private readonly FrontendSettings _settings = options.Value;

    public string BuildPasswordResetUrl(string role, string token)
    {
        var baseUrl = _settings.BaseUrl.TrimEnd('/');
        return $"{baseUrl}/reset-password?role={Uri.EscapeDataString(role)}&token={Uri.EscapeDataString(token)}";
    }
}
