using Microsoft.AspNetCore.Mvc;

namespace AzureBlobProject.Controllers
{
    public class ContainerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
