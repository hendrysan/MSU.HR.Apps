using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using System.Web;
using WebClient.ViewModels.Auth;

namespace WebClient.Controllers
{
    public class AuthController : _BaseController
    {
        private readonly IUserRepository _userRepository;
        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> EmailVerify(string secure, string requester)
        {
            secure = HttpUtility.UrlEncode(secure);
            var data = await _userRepository.EmailVerify(secure, requester);

            return View(data);

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
