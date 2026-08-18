using Microsoft.AspNetCore.Mvc;

namespace AzureBlobProject.Controllers
{
    public class BlobController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
