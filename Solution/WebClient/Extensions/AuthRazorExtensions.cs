using Microsoft.AspNetCore.Mvc.Razor;
using System.Security.Claims;

namespace WebClient.Extensions
{
    public static class AuthRazorExtensions
    {
        public static string GetUserRoleName(this RazorPage page)
        {
            string data = page.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "GetUserRole Error";
            return data;
        }
    }
}
