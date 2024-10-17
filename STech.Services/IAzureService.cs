using Azure.Storage.Blobs.Models;
using Azure;

namespace STech.Services
{
    public interface IAzureService
    {
        string GetContainerName();
        string GetBlobUrl();
        Task DownloadAllBlobs(string downloadPath);
        Task<string?> UploadImage(string imagePath, byte[] imageBytes);
        Task<bool> DeleteImage(string imageUrl);
        AsyncPageable<BlobItem> GetBlobs(string folderName);
    }
}
