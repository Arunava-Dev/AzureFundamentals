using Azure.Storage.Blobs;
using AzureBlobProject.Models;

namespace AzureBlobProject.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobClient;

        public BlobService(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }

        public Task CreateBlob(string name, IFormFile file, string containerName, BlobModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteBlob(string name, string containerName)
        {
            //to retrive the container
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(name); //return the blob with the passing name
            return await blobClient.DeleteIfExistsAsync();
        }

        public async Task<List<string>> GetAllBlobs(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            var blobs = blobContainerClient.GetBlobsAsync();

            List<string>blobNames = new List<string>();
            await foreach (var blob in blobs)
            {
                blobNames.Add(blob.Name);
            }

            return blobNames;
        }

        public Task<List<BlobModel>> GetAllBlobsWithUri(string containerName)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetBlob(string name, string containerName)
        {
            //to retrive the container
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);

            var blobClient = blobContainerClient.GetBlobClient(name); //return the blob with the passing name

            if(blobClient != null)
            {
                return blobClient.Uri.AbsoluteUri;
            }
            return "";
        }
    }
}
