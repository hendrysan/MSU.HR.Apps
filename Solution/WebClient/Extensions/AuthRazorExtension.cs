using Microsoft.AspNetCore.Mvc.Razor;

namespace WebClient.Extensions
{
    public static class AuthRazorExtension
    {
        public static string GetUserId(this RazorPage page)
        {
            string? data = page.User.Claims.FirstOrDefault(c => c.Type.Contains("Id")).Value;
            return data ?? "GetUserId Error";
        }

        public static string GetUserIdNumber(this RazorPage page)
        {
            string? data = page.User.Claims.FirstOrDefault(c => c.Type.Contains("IdNumber")).Value;
            return data ?? "GetUserCode Error";
        }

        public static string GetUserFullName(this RazorPage page)
        {
            string? data = page.User.Claims.FirstOrDefault(c => c.Type.Contains("FullName")).Value;
            return data ?? "GetUserFullName Error";
        }

        //public static string GetUserRoleId(this RazorPage page)
        //{
        //    string? data = page.User.Claims.FirstOrDefault(c => c.Type.Contains("RoleId")).Value;
        //    return data ?? "GetUserRoleId Error";
        //}

        public static string GetUserRoleName(this RazorPage page)
        {
            string? data = page.User.Claims.FirstOrDefault(c => c.Type.Contains("RoleName")).Value;
            return data ?? "GetUserRole Error";
        }


    }
}
