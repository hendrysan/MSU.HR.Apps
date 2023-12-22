using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebClient.ViewModels.Others;

namespace WebClient.Controllers
{
    //[Authorize]
    public class HomeController(ILogger<HomeController> logger) : BaseController
    {
        private readonly ILogger<HomeController> _logger = logger;

        [Route("404")]
        public IActionResult PageNotFound()
        {
            if (HttpContext.Items.ContainsKey("originalPath"))
            {
                _ = HttpContext.Items["originalPath"] as string;
            }
            return View();
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
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
