using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureFunctionTangyWeb.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;

namespace AzureFunctionTangyWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly BlobServiceClient _blobServiceClient;

        public HomeController(IHttpClientFactory httpClientFactory, BlobServiceClient blobServiceClient)
        {
            _httpClientFactory = httpClientFactory;
            _blobServiceClient = blobServiceClient;
        }

        public IActionResult Index()
        {
            return View();
        }
        //http://localhost:7070/api/OnSalesUploadWriteToQueue

        [HttpPost]
        public async Task<IActionResult> Index(SalesRequest salesRequest,IFormFile file)
        {
            salesRequest.Id = Guid.NewGuid().ToString();
            using var client = _httpClientFactory.CreateClient(); //creates an HttpClient that MVC application can use to communicate with another application/ service over HTTP.
            client.BaseAddress = new Uri("http://localhost:7070/api/");   //Set the Azure Function's base URL
            using (var content = new StringContent(JsonConvert.SerializeObject(salesRequest), System.Text.Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage response = await client.PostAsync("OnSalesUploadWriteToQueue", content);
                string returnValue = await response.Content.ReadAsStringAsync();

            }

            if (file!=null)
            {
                var fileName = salesRequest.Id + Path.GetExtension(file.FileName);
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient("functionsalesrep");
                var blobClient = containerClient.GetBlobClient(fileName);

                var httpheaders = new BlobHttpHeaders()
                {
                    ContentType = file.ContentType
                };
                await blobClient.UploadAsync(file.OpenReadStream(), httpheaders);
            }

            // retirive the container in our storage account

         

            return RedirectToAction(nameof(Index));
           
        }
        /*
 * FLOW: MVC → Azure Function
 *
 * 1. The user submits the Sales Upload form from Index.cshtml.
 *    MVC model binding converts the submitted form data into a SalesRequest object.
 *
 * 2. Create an HttpClient using IHttpClientFactory.
 *    IHttpClientFactory was registered in Program.cs using AddHttpClient().
 *
 * 3. Set the BaseAddress to the Azure Function's local URL.

 *
 *       http://localhost:7070/api/
 *
 * 4. Convert the SalesRequest C# object into JSON using
 *    JsonConvert.SerializeObject().
 *
 *       C# Object → JSON
 *
 * 5. Create StringContent with:
 *       - JSON data
 *       - UTF-8 encoding
 *       - "application/json" content type
 *
 * 6. Send the JSON to the Azure Function using HTTP POST.
 *
 *       MVC Web App
 *           ↓
 *       HTTP POST + JSON
 *           ↓
 *       Azure Function
 *       OnSalesUploadWriteToQueue
 *
 * 7. The Azure Function processes the request and sends back an
 *    HttpResponseMessage.
 *
 * 8. Read the response body using ReadAsStringAsync().
 *
 *       Azure Function
 *           ↓
 *       HTTP Response
 *           ↓
 *       MVC Web App
 *
 * 9. Finally, redirect the user back to the Index page.
 *
 * Overall:
 *
 *       Index.cshtml
 *           ↓
 *       HomeController
 *           ↓
 *       SalesRequest (C# object)
 *           ↓
 *       SerializeObject()
 *           ↓
 *       JSON
 *           ↓
 *       HTTP POST
 *           ↓
 *       Azure Function
 *           ↓
 *       HTTP Response
 *           ↓
 *       Redirect to Index
 */
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
