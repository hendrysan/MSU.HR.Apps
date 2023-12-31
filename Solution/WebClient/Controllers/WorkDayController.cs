using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Repositories.Interfaces;
using System.Globalization;
using WebClient.Filters;
using WebClient.ViewModels.WorkDay;
using static Models.Entities.EnumEntities;

namespace WebClient.Controllers
{
    [Authorize]
    public class WorkDayController(ILogger<WorkDayController> logger, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IGrantAccessRepository grantAccessRepository, IWorkDayRepository workDayRepository) : BaseController
    {
        private readonly ILogger<WorkDayController> _logger = logger;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IGrantAccessRepository _grantAccessRepository = grantAccessRepository;


        private readonly IWorkDayRepository _workDayRepository = workDayRepository;


        [GrantAccessActionFilter(EnumAction = EnumAction.View, EnumModule = EnumModule.WorkDay)]
        public async Task<IActionResult> Index()
        {

            var model = new WorkDayIndexFormRequest();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkDayIndexFormRequest formRequest)
        {
            var user = await GetCurrentUser(_httpContextAccessor, _userRepository);
            if (formRequest.Submit.ToLower() == "create")
            {
                var rangeDate = formRequest.DateRangePicker.Split(" - ");
                if (rangeDate.Any())
                {
                    DateTime startDate = DateTime.ParseExact(rangeDate[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    DateTime endDate = DateTime.ParseExact(rangeDate[1], "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                    {
                        await _workDayRepository.UpdateDate(user, date, formRequest.Remarks);
                        SetAlert($"Success insert / update data on {formRequest.DateRangePicker}", ViewModels.Others.AlertType.Success);
                    }
                }
            }
            else if (formRequest.Submit.ToLower() == "generate")
            {
                var responseGenerate = await _workDayRepository.GenerateDate(user, formRequest.Year);
                if (responseGenerate.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    SetAlert("Generate Week End Success on Year = " + formRequest.Year, ViewModels.Others.AlertType.Success);
                }
            }

            return RedirectToAction("Index", "WorkDay");
        }

        [HttpPost]
        public async Task<JsonResult> CalendarData([FromBody] CalendarDataRequest request)
        {
            var result = new List<CalendarDataResponse>();
            var response = await _workDayRepository.SearchAsync(start: request.Start, end: request.End);
            result = response.MasterWorkDays?.Select(i => new CalendarDataResponse()
            {
                AllDay = true,
                Start = i.DateWork.ToString("yyyy-MM-dd"),
                Title = i.Remarks,
                Stick = true
            }).ToList();


            return Json(result);
        }

        [HttpPost]
        [Obsolete]
        public async Task<JsonResult> DataTableWorkDay()
        {
            _logger.LogInformation("Data Table Start");
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            var request = new DataTableRequest
            {
                Draw = draw,
                SortColumn = sortColumn,
                SortColumnDirection = sortColumnDirection,
                SearchValue = searchValue,
                PageSize = pageSize,
                Skip = skip
            };

            var response = await _workDayRepository.DataTableAsync(request);

            return Json(response);
        }

        [HttpGet("Delete/{id}/{date}")]
        public async Task<IActionResult> Delete(int id, string date)
        {
            var user = await GetCurrentUser(_httpContextAccessor, _userRepository);
            await _workDayRepository.DeleteDate(user, id);

            SetAlert("Deleted Success on date = " + date, ViewModels.Others.AlertType.Success);
            return RedirectToAction("Index", "WorkDay");

        }
    }
}
