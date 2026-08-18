namespace MediPoint.Application.Features.Patients.Chat.DTOs;

// Named ChatResult (not ChatResponse) to avoid clashing with Microsoft.Extensions.AI.ChatResponse.
public class ChatResult
{
    public string Reply { get; set; } = null!;
}
