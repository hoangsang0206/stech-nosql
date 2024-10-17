using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace STech.Services.Services
{
    public class AzureService : IAzureService
    {
        private readonly string ConnectionString;
        private readonly string BlobContainerName ;
        private readonly string BlobUrl;

        private readonly BlobServiceClient BlobServiceClient;
        private readonly BlobContainerClient ContainerClient;

        public AzureService(string connectionString, string blobContainerName, string blobUrl)
        {
            ConnectionString = connectionString;
            BlobContainerName = blobContainerName;
            BlobUrl = blobUrl;

            BlobServiceClient = new BlobServiceClient(ConnectionString);
            ContainerClient = BlobServiceClient.GetBlobContainerClient(BlobContainerName);

        }

        public string GetContainerName()
        {
            return BlobContainerName;
        }
        public string GetBlobUrl()
        {
            return BlobUrl;
        }

        public async Task DownloadAllBlobs(string downloadPath)
        {
            var blobs = ContainerClient.GetBlobs();

            foreach(BlobItem blob in blobs)
            {
                BlobClient blobClient = ContainerClient.GetBlobClient(blob.Name);
                string localFilePath = Path.Combine(downloadPath, blob.Name);

                Directory.CreateDirectory(Path.GetDirectoryName(localFilePath));

                using (FileStream fileStream = File.OpenWrite(localFilePath))
                {
                    await blobClient.DownloadToAsync(fileStream);
                }
            }
        }

        public async Task<string?> UploadImage(string imagePath, byte[] imageBytes)
        {
            BlobClient blobClient = ContainerClient.GetBlobClient(imagePath);
            BlobContentInfo info = await blobClient.UploadAsync(new MemoryStream(imageBytes), true);
            if(!string.IsNullOrEmpty(info.ETag.ToString()))
            {
                return $"{BlobUrl}/{BlobContainerName}/{imagePath}";
            }


            return null;
        }

        public async Task<bool> DeleteImage(string imageUrl)
        {
            BlobClient blobClient = ContainerClient.GetBlobClient(imageUrl.Replace($"{BlobUrl}/{BlobContainerName}/", ""));
            return await blobClient.DeleteIfExistsAsync();
        }

        public AsyncPageable<BlobItem> GetBlobs(string folderName)
        {
            AsyncPageable<BlobItem> blobs = ContainerClient.GetBlobsAsync(prefix: folderName);
            return blobs;
        }
    }
}
