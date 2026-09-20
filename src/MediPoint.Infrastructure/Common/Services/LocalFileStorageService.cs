using MediPoint.Application.Common.Services;
using Microsoft.Extensions.Configuration;

namespace MediPoint.Infrastructure.Common.Services;

public class LocalFileStorageService(IConfiguration configuration) : IFileStorageService
{
    public async Task<string> SaveAsync(Stream content, string fileName, CancellationToken cancellationToken)
    {
        var relativeRoot = configuration["FileStorage:UsersImagesPath"] ?? "wwwroot/uploads/users";
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), relativeRoot);
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await content.CopyToAsync(fileStream, cancellationToken);

        return $"/uploads/users/{uniqueFileName}";
    }
}
