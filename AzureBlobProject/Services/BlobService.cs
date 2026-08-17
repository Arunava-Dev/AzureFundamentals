using AzureBlobProject.Models;

namespace AzureBlobProject.Services
{
    public class BlobService : IBlobService
    {
        public Task CreateBlob(string name, IFormFile file, string containerName, BlobModel model)
        {
            throw new NotImplementedException();
        }

        public Task DeleteBlob(string name, string containerName)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetAllBlobs(string containerName)
        {
            throw new NotImplementedException();
        }

        public Task<List<BlobModel>> GetAllBlobsWithUri(string containerName)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetBlob(string name, string containerName)
        {
            throw new NotImplementedException();
        }
    }
}
