using Microsoft.AspNetCore.Mvc;
using WebClient.ViewModels.Others;

namespace WebClient.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            NavigationModel model = new();
            return View(model);
        }
    }
}
