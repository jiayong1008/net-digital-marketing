using Microsoft.AspNetCore.Mvc;

namespace DigitalMarketing2.Controllers
{
    public class ResourcesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
