using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;

namespace MediPoint.Infrastructure.Ai;

public static class OpenAiChatClientExtensions
{
    /// <summary>
    /// Registers a singleton <see cref="IChatClient"/> backed by OpenAI. The API key is read from
    /// configuration ("OpenAI:ApiKey"), falling back to the OPENAI_API_KEY environment variable, and
    /// the model from "OpenAI:Model" (defaults to gpt-4o-mini). The client is built lazily on first
    /// resolve, so the app still starts with no key configured — only the chat path needs it.
    /// </summary>
    public static IServiceCollection AddOpenAiChatClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IChatClient>(_ =>
        {
            var apiKey = configuration["OpenAI:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException(
                    "OpenAI API key is not configured. Set \"OpenAI:ApiKey\" in appsettings.json (or the OPENAI_API_KEY environment variable).");

            var model = configuration["OpenAI:Model"] ?? "gpt-4o-mini";

            return new OpenAIClient(apiKey)
                .GetChatClient(model)
                .AsIChatClient();
        });

        return services;
    }
}
