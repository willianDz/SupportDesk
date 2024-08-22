using Microsoft.AspNetCore.Http;

namespace SupportDesk.Application.Contracts.Infraestructure.FileStorage
{
    public interface IFileStorageService
    {
        Task<List<string>> UploadFilesAsync(List<IFormFile> files, string containerName);
    }
}
