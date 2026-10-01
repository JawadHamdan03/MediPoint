using MediatR;

namespace MediPoint.Application.Features.Users.GetPrescriptionPdf;

public record GetPrescriptionPdfQuery(Guid PrescriptionId, Guid UserId, string Role) : IRequest<byte[]>;
