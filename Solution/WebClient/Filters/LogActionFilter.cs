using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace WebClient.Filters
{
    public class LogActionFilter : IAsyncActionFilter
    {
        private Stopwatch stopwatch;
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            stopwatch = Stopwatch.StartNew();




            await next();
        }

        public async Task OnActionExecutedAsync(ActionExecutedContext context)
        {
            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            var message = $"Action took {elapsedMilliseconds} ms to execute.";
            Debug.WriteLine(message);
        }
    }
}
