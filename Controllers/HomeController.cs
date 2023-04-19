using DigitalMarketing2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DigitalMarketing2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private UserManager<User> userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<User> userMgr)
        {
            _logger = logger;
            userManager = userMgr;
        }


        //[Authorize]
        //public IActionResult Index()
        //{
        //    return View();
        //}

        // [Authorize]
        public async Task<IActionResult> Index()
        {
            User user = await userManager.GetUserAsync(HttpContext.User);
            if (user != null && await userManager.IsInRoleAsync(user, "Admin"))
                return View("AdminIndex", user.UserName);

            // Return member index view
            string message = "Hello " + (user != null ? user.UserName : "");
            return View((object)message);
        }

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