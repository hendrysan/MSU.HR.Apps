using Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Newtonsoft.Json.Linq;
using Repositories.Interfaces;
using System.Security.Claims;
using System.Web;
using WebClient.ViewModels.Auth;
using WebClient.ViewModels.Others;
using static System.Net.Mime.MediaTypeNames;

namespace WebClient.Controllers
{
    public class AuthController(IUserRepository userRepository, ITokenRepository tokenRepository) : _BaseController
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ITokenRepository _tokenRepository = tokenRepository;

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
            var model = new LoginFormRequest
            {
                UserInput = "hendry.priyatno@gmail.com",
                Password = "123456"
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginFormRequest formRequest)
        {
            if (!ModelState.IsValid || formRequest == null)
            {
                ModelState.AddModelError("ModelState", "Invalid login attempt");
                return View(formRequest);
            }

            var request = new Models.Request.LoginRequest
            {
                Password = formRequest.Password,
                UserInput = formRequest.UserInput,
            };

            var response = await _userRepository.Login(request);

            if (response == null)
            {
                ModelState.AddModelError("ModelState", "Login invalid");
                return View(formRequest);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                ModelState.AddModelError("ModelState", response.Message ?? "Message Error Not Found");
                return View(formRequest);
            }

            MasterUser? masterUser = response.Data as MasterUser;

            var claims = _tokenRepository.CreateClaims(masterUser);
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            string? returnUrl = HttpContext.Request.Query["returnUrl"];
            return Redirect(returnUrl ?? "/");
        }

        [HttpGet]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {

            var listMetode = new List<SelectListItem>()
            {
                 new SelectListItem{ Text = "Email", Value = "1", Selected=true},
                 new SelectListItem{ Text = "Whats App (WA)", Value = "2"},
            };

            var model = new RegisterFormRequest()
            {
                DropdownMethod = listMetode
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAsync(RegisterFormRequest formRequest)
        {
            if (!ModelState.IsValid || formRequest == null)
            {
                ModelState.AddModelError("ModelState", "Invalid login attempt");
                return View(formRequest);
            }

            var request = new Models.Request.RegisterRequest()
            {
                IdNumber = formRequest.IdNumber,
                Password = formRequest.Password,
                FullName = formRequest.FullName,
                RegisterVerify = formRequest.RegisterMethod == 1 ? Models.Request.RegisterVerify.Email : Models.Request.RegisterVerify.PhoneNumber,
                UserInput = formRequest.RegisterMethod == 1 ? formRequest.Email : formRequest.PhoneNumber
            };

            var response = await _userRepository.Register(request);

            if (response == null)
            {
                ModelState.AddModelError("ModelState", "Response not found");
                return View(formRequest);
            }

            if (response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                ModelState.AddModelError("ModelState", response.Message);
                return View(formRequest);
            }


            SetAlert(response.Message, AlertType.Success);

            return RedirectToAction("Login");


        }

    }
}
