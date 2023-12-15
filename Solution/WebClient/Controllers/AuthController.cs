using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Repositories.Interfaces;
using System.Security.Claims;
using System.Web;
using WebClient.ViewModels.Auth;
using WebClient.ViewModels.Others;

namespace WebClient.Controllers
{
    public class AuthController(IUserRepository userRepository, ITokenRepository tokenRepository) : BaseController
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ITokenRepository _tokenRepository = tokenRepository;

        [HttpGet]
        public async Task<IActionResult> EmailVerify(string secure, string requester)
        {
            secure = HttpUtility.UrlEncode(secure);
            var response = await _userRepository.EmailVerify(secure, requester);

            SetAlert(response.Message, response.StatusCode == System.Net.HttpStatusCode.OK ? AlertType.Success : AlertType.Danger);

            return RedirectToAction("Login");

        }

        [HttpGet]
        public async Task<IActionResult> OtpVerify(string phoneNumber, string idNumber)
        {
            GetAlert();

            var response = await _userRepository.CheckExpiredToken(phoneNumber, idNumber);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                SetAlert(response.Message, AlertType.Danger);
                return RedirectToAction("Login");
            }

            var model = new OtpVerifyFormRequest()
            {
                IdNumber = idNumber,
                Requester = phoneNumber,
                CountDown = response.Minutes ?? "00:00",
                ShowTimer = response.StatusCode == System.Net.HttpStatusCode.OK
            };

            

            //model.CountDown = response.Data.

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OtpVerify(OtpVerifyFormRequest formRequest)
        {
            if (!ModelState.IsValid || formRequest == null)
            {
                SetAlert("Invalid login attempt", AlertType.Danger);
                return View(formRequest);
            }

            var response = await _userRepository.PhoneNumberVerify(formRequest.OtpSecure, formRequest.Requester, formRequest.IdNumber);

            if (response == null)
            {
                SetAlert("Response not found", AlertType.Danger);
                return View(formRequest);
            }

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                SetAlert(response.Message, AlertType.Danger);
                return RedirectToAction("OtpVerify", new { @phoneNumber = formRequest.Requester, @idNumber = formRequest.IdNumber });
            }

            SetAlert("Verify Successfuly, Please try login", AlertType.Success);

            return RedirectToAction("Login");

        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "")
        {

            GetAlert();
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
                SetAlert("Invalid login attempt", AlertType.Danger);
                return View(formRequest);
            }

            var request = new Models.Requests.LoginRequest
            {
                Password = formRequest.Password,
                UserInput = formRequest.UserInput,
            };

            var response = await _userRepository.Login(request);

            if (response == null)
            {
                SetAlert("Login invalid", AlertType.Danger);
                return View(formRequest);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                SetAlert(response.Message ?? "Message Error Not Found", AlertType.Danger);
                return View(formRequest);
            }

            if (response.MasterUser == null)
            {
                SetAlert(response.Message ?? "Data User Error Not Found", AlertType.Danger);
                return View(formRequest);
            }

            //MasterUser? masterUser = response.Data as MasterUser;

            var claims = _tokenRepository.CreateClaims(response.MasterUser);
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
            GetAlert();
            var listMetode = new List<SelectListItem>()
            {
                 new() { Text = "Email", Value = "1", Selected=true},
                 new() { Text = "Whats App (WA)", Value = "2"},
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
                SetAlert("Invalid login attempt", AlertType.Danger);
                return View(formRequest);
            }

            if (formRequest.IdNumber == null)
            {
                SetAlert("Id Number is Required", AlertType.Danger);
                return View(formRequest);
            }

            if (formRequest.Password == null)
            {
                SetAlert("Password is Required", AlertType.Danger);
                return View(formRequest);
            }

            if (formRequest.FullName == null)
            {
                SetAlert("Password is Required", AlertType.Danger);
                return View(formRequest);
            }

            Models.Requests.RegisterRequest? request;
            if (formRequest.RegisterMethod == 1)
            {
                if (formRequest.Email == null)
                {
                    SetAlert("Email is Required", AlertType.Danger);
                    return View(formRequest);
                }

                request = new Models.Requests.RegisterRequest()
                {
                    IdNumber = formRequest.IdNumber,
                    Password = formRequest.Password,
                    FullName = formRequest.FullName,
                    RegisterVerify = formRequest.RegisterMethod == 1 ? Models.Requests.RegisterVerify.Email : Models.Requests.RegisterVerify.PhoneNumber,
                    UserInput = formRequest.Email
                };
            }
            else
            {
                if (formRequest.PhoneNumber == null)
                {
                    SetAlert("PhoneNumber is Required", AlertType.Danger);
                    return View(formRequest);
                }

                request = new Models.Requests.RegisterRequest()
                {
                    IdNumber = formRequest.IdNumber,
                    Password = formRequest.Password,
                    FullName = formRequest.FullName,
                    RegisterVerify = formRequest.RegisterMethod == 1 ? Models.Requests.RegisterVerify.Email : Models.Requests.RegisterVerify.PhoneNumber,
                    UserInput = formRequest.PhoneNumber
                };
            }

            var response = await _userRepository.Register(request);

            if (response == null)
            {
                SetAlert("Response not found", AlertType.Danger);
                return View(formRequest);
            }

            if (response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                SetAlert(response.Message, AlertType.Danger);
                return View(formRequest);
            }

            SetAlert(response.Message, AlertType.Success);

            if (request.RegisterVerify == Models.Requests.RegisterVerify.PhoneNumber)
                return RedirectToAction("OtpVerify", new { @phoneNumber = formRequest.PhoneNumber, @idNumber = formRequest.IdNumber });
            else
                return RedirectToAction("Login");
        }
    }
}
