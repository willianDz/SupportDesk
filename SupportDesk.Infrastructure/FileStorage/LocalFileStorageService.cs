using Microsoft.AspNetCore.Http;
using SupportDesk.Application.Contracts.Infraestructure.FileStorage;

namespace SupportDesk.Infrastructure.FileStorage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _storagePath;
    private readonly string _baseUrl;

    public LocalFileStorageService(string storagePath, string baseUrl)
    {
        _storagePath = storagePath;
        _baseUrl = baseUrl;
    }

    public async Task<List<string>> UploadFilesAsync(List<IFormFile> files, string containerName)
    {
        var urls = new List<string>();

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                // Generar un nombre de archivo único usando un GUID
                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                // Crear la ruta completa donde se almacenará el archivo
                var filePath = Path.Combine(_storagePath, containerName, uniqueFileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                // Guardar el archivo en el sistema de archivos
                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                // Generar la URL pública para acceder al archivo
                var fileUrl = $"{_baseUrl}{containerName}/{uniqueFileName}";
                urls.Add(fileUrl);
            }
        }

        return urls;
    }
}
