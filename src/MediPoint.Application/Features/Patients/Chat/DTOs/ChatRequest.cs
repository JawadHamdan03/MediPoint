namespace MediPoint.Application.Features.Patients.Chat.DTOs;

// Request body for POST /patients/chat. The patient id is NOT taken from the body —
// the controller reads it from the JWT claim and passes it into the command.
public class ChatRequest
{
    public string Message { get; set; } = null!;
}
