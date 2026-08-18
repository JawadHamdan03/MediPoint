using MediatR;
using MediPoint.Application.Features.Patients.Chat.DTOs;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace MediPoint.Application.Features.Patients.Chat;

public class ChatCommandHandler(IChatClient chatClient, ISender sender)
    : IRequestHandler<ChatCommand, ChatResult>
{
    private const string SystemPrompt =
        """
        You are MediPoint's assistant for the currently signed-in patient.
        You can help with two things only:
          - finding available doctors by specialty, and
          - looking up the patient's own medical records.
        Always use the provided tools to answer; never invent doctors, appointments, or records.
        Never ask the patient for their patient id or account id — you already act on their behalf.
        Booking or cancelling appointments is not supported yet; if asked, say so politely.
        Keep replies concise and friendly, and never reveal another patient's data.
        """;

    public async Task<ChatResult> Handle(ChatCommand request, CancellationToken cancellationToken)
    {
        // Tools are bound to THIS request's patient id (from the JWT) and cancellation token.
        var tools = new PatientChatTools(sender, request.PatientId, cancellationToken);

        AIAgent agent = chatClient.AsAIAgent(
            instructions: SystemPrompt,
            tools:
            [
                AIFunctionFactory.Create(tools.FindDoctorsAsync),
                AIFunctionFactory.Create(tools.GetMyMedicalRecordsAsync),
            ]);

        var response = await agent.RunAsync(request.Message);

        return new ChatResult { Reply = response.ToString() };
    }
}
