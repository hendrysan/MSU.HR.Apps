using Microsoft.AspNetCore.Mvc;
using WebClient.ViewModels.Auth;

namespace WebClient.Controllers
{
    public class AuthController : _BaseController
    {
        public AuthController()
        {

        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "")
        {
            ViewData["returnUrl"] = returnUrl;
            var model = new LoginRequest
            {
                CodeNIK = "123456789",
                Password = "123456789"
            };
            return View(model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> LoginAsync(LoginRequest request)
        //{
        //}

        public IActionResult RegisterEmail()
        {
            return View();
        }
    }
}
