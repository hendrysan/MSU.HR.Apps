using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Requests;
using System.Text.Json.Serialization;
using Repositories.Interfaces;
using System.Collections;
using Newtonsoft.Json.Converters;

namespace WebClient.Controllers
{
    public class GrantAccessController(ILogger<GrantAccessController> logger, IGrantAccessRepository grantAccessRepository) : BaseController
    {
        private readonly ILogger<GrantAccessController> _logger = logger;
        private readonly IGrantAccessRepository _grantAccessRepository = grantAccessRepository;

        public IActionResult WebClient()
        {
            var data = _grantAccessRepository.ListAccess(EnumEntities.EnumSource.WebClient);

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> PostDataTableGrantAccess()
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

            var dataTable = await _grantAccessRepository.DataTableAsync(request);
            var enumerable = dataTable.Data as IEnumerable;
            dataTable.Data = enumerable.Cast<GrantAccess>().ToList();

            //foreach (var i in (dataTable.Data as IEnumerable).Cast<GrantAccess>().ToList())
            //{
            //    i.Module.ToString();
            //    i.Source.ToString();
            //}

            Newtonsoft.Json.JsonSerializerSettings settings = new();
            //settings.Converters.Add(item: new StringEnumConverter { AllowIntegerValues = false });
            settings.Converters.Add(item: new StringEnumConverter { CamelCaseText = true });


            return Json(dataTable);
        }
    }
}
