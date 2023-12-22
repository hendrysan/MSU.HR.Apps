using Microsoft.AspNetCore.Mvc;
using WebClient.ViewModels.Others;

namespace WebClient.ViewComponents
{
    public class NavigationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            NavigationModel model = new();
            model.Title = "model test";
            return View(model);
        }
    }
}
