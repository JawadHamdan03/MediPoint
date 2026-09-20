using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Users.UploadProfileImage.DTOs;
using MediPoint.Domain.Entities.User.Shared;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Users.UploadProfileImage;

public class UploadProfileImageCommandHandler(IAppDbContext dbContext, IFileStorageService fileStorageService)
    : IRequestHandler<UploadProfileImageCommand, UploadProfileImageResult>
{
    public async Task<UploadProfileImageResult> Handle(UploadProfileImageCommand request, CancellationToken cancellationToken)
    {
        BaseUser? user = request.Role switch
        {
            "Admin" => await dbContext.Admins.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken),
            "Doctor" => await dbContext.Doctors.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken),
            "Patient" => await dbContext.Patients.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken),
            _ => null
        };

        if (user is null)
            throw new NotFoundException("User", request.UserId.ToString());

        var imageUrl = await fileStorageService.SaveAsync(request.Content, request.FileName, cancellationToken);
        user.ImageUrl = imageUrl;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UploadProfileImageResult(imageUrl);
    }
}
