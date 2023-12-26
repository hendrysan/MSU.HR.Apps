using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories.Interfaces;
using System.Security.Claims;
using System.Text.Json;
using WebClient.Extensions;
using WebClient.ViewModels.Others;

namespace WebClient.ViewComponents
{
    [Authorize]
    public class NavigationViewComponent(IHttpContextAccessor httpContextAccessor) : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        public IViewComponentResult Invoke()
        {

            NavigationModel model = new();
            var navigation = HttpContext.Session.GetString("Navigation");
            if (!string.IsNullOrEmpty(navigation))
            {
                model = JsonSerializer.Deserialize<NavigationModel>(navigation) ?? new();
            }
            else
            {
                var alert = new AlertModel
                {
                    Message = "Session is expired, Please re-login",
                    Type = AlertType.Danger
                };
                HttpContext.Session.SetString("Alert", JsonSerializer.Serialize(alert));
                _httpContextAccessor.HttpContext.Response.Redirect("/Auth/Logout");
                return View(new NavigationModel());
            }


            return View(model);
        }
    }
}
