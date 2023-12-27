using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Requests;
using System.Diagnostics;
using WebClient.ViewModels.Others;
using WebClient.ViewModels.WorkDay;

namespace WebClient.Controllers
{
    [Authorize]
    public class WorkDayController(ILogger<WorkDayController> logger) : BaseController
    {
        private readonly ILogger<WorkDayController> _logger = logger;

        public IActionResult Calendar()
        {
            return View();
        }

        [HttpPost]
        [Obsolete]
        public async Task<JsonResult> PostCalendarData(PostCalendarDataRequest request)
        {
            var result = new
            {
                title = "",
                start = DateTime.Now,
                end = DateTime.Now.AddDays(1),
            };

            return Json(result);
        }

    }
}
