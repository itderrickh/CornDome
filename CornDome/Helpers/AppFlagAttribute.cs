using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CornDome.Helpers
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method,
        AllowMultiple = false,
        Inherited = true)]
    public class AppFlagAttribute : Attribute, IAsyncActionFilter, IAsyncPageFilter
    {
        private readonly string _flagName;

        public AppFlagAttribute(string flagName)
        {
            _flagName = flagName;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var appFlags = context.HttpContext.RequestServices
                .GetRequiredService<AppFlagHelper>();

            var isActive = appFlags.IsActive(_flagName);

            if (!isActive)
            {
                context.Result = new JsonResult(new
                {
                    message = "Feature not available"
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };

                return;
            }

            await next();
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            var appFlags = context.HttpContext.RequestServices
                .GetRequiredService<AppFlagHelper>();

            var isActive = appFlags.IsActive(_flagName);

            if (!isActive)
            {
                context.Result = new NotFoundResult();
                return;
            }

            await next();
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }
    }
}
