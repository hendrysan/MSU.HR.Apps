using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Entities;
using System.Text.Json;
using static Models.Entities.EnumEntities;

namespace WebClient.Filters
{
    public class GrantAccessActionFilter : ActionFilterAttribute
    {
        public EnumModule EnumModule { get; set; }
        public EnumAction EnumAction { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool isAccess = false;
            var grants = filterContext.HttpContext.User.Claims.FirstOrDefault(i => i.Type.Equals("GrantAccess"));

            if (grants != null)
            {
                List<GrantAccess> access = JsonSerializer.Deserialize<List<GrantAccess>>(grants.Value);

                var modul = access.Where(i => i.Module == EnumModule).FirstOrDefault();
                if (modul != null)
                {
                    switch (EnumAction)
                    {
                        case EnumAction.View:
                            isAccess = modul.IsView;
                            break;
                        case EnumAction.Create:
                            isAccess = modul.IsCreate;
                            break;
                        case EnumAction.Edit:
                            isAccess = modul.IsEdit;
                            break;
                        case EnumAction.Delete:
                            isAccess = modul.IsDelete;
                            break;
                        case EnumAction.Export:
                            isAccess = modul.IsExport;
                            break;
                        default:
                            isAccess = false;
                            break;
                    }
                }
            }

            if (!isAccess)
            {
                filterContext.Result = new UnauthorizedResult();
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }

        }
    }
}
