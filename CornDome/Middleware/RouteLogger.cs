using CornDome.Repository;

namespace CornDome.Middleware
{
    public class RouteLogger(RequestDelegate next, ILoggingRepository loggingRepository)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var currentRoute = context.Request.Path.ToString();
                var previousRoute = "";

                var refererPath = context.Request.Headers.Referer;
                if (!string.IsNullOrEmpty(refererPath))
                {
                    var prevUrl = new Uri(refererPath);
                    previousRoute = prevUrl.AbsolutePath;
                }

                // Log in format: (FromRoute, ToRoute, HitCount)
                await loggingRepository.LogRouteChange(previousRoute, currentRoute);
            }
            catch { } // Silently ignore if failed, for now

            await _next(context);
        }
    }
}
