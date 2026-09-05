using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TangyAzureFunc.Models;

namespace TangyAzureFunc;

public class OnSalesUploadWriteToQueue
{
    private readonly ILogger<OnSalesUploadWriteToQueue> _logger;

    public OnSalesUploadWriteToQueue(ILogger<OnSalesUploadWriteToQueue> logger)
    {
        _logger = logger;
    }

    [Function("OnSalesUploadWriteToQueue")]
    [QueueOutput("SalesRequestInBound",Connection = "AzureWebJobsStorage")]
    public async Task<SalesRequest> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        //Read the JSON data sent in the HTTP request body and convert it into a string.
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        // Deserialize the JSON string back into a SalesRequest C# object so we can work with its properties.
        SalesRequest data = JsonConvert.DeserializeObject<SalesRequest>(requestBody);
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return data ?? new SalesRequest();
    }
}
// Return the SalesRequest object.
// The QueueOutput binding automatically writes this returned object
// as a message to the "SalesRequestOutBound" Azure Storage Queue.