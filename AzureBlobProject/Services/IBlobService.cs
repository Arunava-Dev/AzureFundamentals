using AzureBlobProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobProject.Services
{
    public interface IBlobService
    {
        Task<List<string>> GetAllBlobs(string containerName);
        Task<List<BlobModel>> GetAllBlobsWithUri(string containerName);
        Task<string> GetBlob(string name,string containerName);
        Task<bool> CreateBlob(string name,IFormFile file, string containerName,BlobModel model);
        Task<bool> DeleteBlob(string name,string containerName);
    }
}
