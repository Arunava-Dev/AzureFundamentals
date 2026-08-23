using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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

        public async Task<bool> CreateBlob(string name, IFormFile file, string containerName, BlobModel blobmodel)
        {
            //to retrive the container
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(name); //return the blob with the passing name

            var httpHeaders = new BlobHttpHeaders()
            {
                ContentType = file.ContentType
            };


            IDictionary<string, string> metaData = new Dictionary<string, string>();
            if(!string.IsNullOrEmpty(blobmodel.Comment))
            {
                metaData.Add("title", blobmodel.Title);
            }
            if(!string.IsNullOrEmpty(blobmodel.Comment))
            {
                metaData.Add("comment", blobmodel.Comment);
            }

            var result = await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders,metaData);

            if(result != null)
            {
                return true;
            }
            return false;
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
