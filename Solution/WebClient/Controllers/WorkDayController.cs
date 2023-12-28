using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebClient.ViewModels.WorkDay;

namespace WebClient.Controllers
{
    [Authorize]
    public class WorkDayController(ILogger<WorkDayController> logger) : BaseController
    {
        private readonly ILogger<WorkDayController> _logger = logger;

        
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult CalendarData([FromBody] CalendarDataRequest request)
        {

            var result = new List<CalendarDataResponse>();

            var data = new CalendarDataResponse
            {
                Title = "Test HardCode",
                Start = DateTime.Now.AddDays(1)
            };

            result.Add(data);

            return Json(result);
        }

    }
}
