using FluentValidation;

namespace MediPoint.Application.Features.Users.UploadProfileImage;

public class UploadProfileImageCommandValidator : AbstractValidator<UploadProfileImageCommand>
{
    private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/webp"];
    private const long MaxSizeBytes = 5 * 1024 * 1024;

    public UploadProfileImageCommandValidator()
    {
        RuleFor(x => x.ContentType)
            .Must(ct => AllowedContentTypes.Contains(ct))
            .WithMessage("Only JPEG, PNG, or WebP images are allowed.");

        RuleFor(x => x.Length)
            .GreaterThan(0)
            .WithMessage("The image is empty.")
            .LessThanOrEqualTo(MaxSizeBytes)
            .WithMessage("The image must be 5MB or smaller.");
    }
}
