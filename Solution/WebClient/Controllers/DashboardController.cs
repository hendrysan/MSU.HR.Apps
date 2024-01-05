using Microsoft.AspNetCore.Mvc;

namespace WebClient.Controllers
{
    public class DashboardController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
