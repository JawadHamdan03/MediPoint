using MediatR;
using MediPoint.Application.Features.Users.UploadProfileImage.DTOs;

namespace MediPoint.Application.Features.Users.UploadProfileImage;

public record UploadProfileImageCommand(
    Guid UserId,
    string Role,
    Stream Content,
    string FileName,
    string ContentType,
    long Length) : IRequest<UploadProfileImageResult>;
