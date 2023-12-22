using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebClient.ViewModels.Others;

namespace WebClient.ViewComponents
{
    public class AlertViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {

            AlertModel? model = default;
            var alert = HttpContext.Session.GetString("Alert");
            if (!string.IsNullOrEmpty(alert))
            {
                model = JsonSerializer.Deserialize<AlertModel>(alert);
                HttpContext.Session.SetString("Alert", "");
            }
            return View(model);

        }
    }
}
