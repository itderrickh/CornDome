using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CornDome.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class RequireUserAgentAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(
            ActionExecutingContext context)
        {
            var userAgent = context.HttpContext.Request.Headers.UserAgent.ToString();

            if (string.IsNullOrWhiteSpace(userAgent))
            {
                context.Result = new BadRequestObjectResult(
                    new ProblemDetails
                    {
                        Title = "User-Agent required",
                        Detail = "Requests must include a non-empty User-Agent header.",
                        Status = StatusCodes.Status400BadRequest
                    });
            }
        }
    }
}
