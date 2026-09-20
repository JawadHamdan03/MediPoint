namespace MediPoint.Application.Common.Services;

public interface IFileStorageService
{
    Task<string> SaveAsync(Stream content, string fileName, CancellationToken cancellationToken);
}
