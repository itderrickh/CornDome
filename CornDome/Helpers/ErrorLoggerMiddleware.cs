using CornDome.Models;
using CornDome.Repository;
using System.Diagnostics;

namespace CornDome.Helpers
{
    public class ErrorLoggerMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(
        HttpContext context,
        ILogEntryRepository repository)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await next(context);
                stopwatch.Stop();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                await repository.InsertAsync(new LogEntry
                {
                    Timestamp = DateTime.Now,
                    Method = context.Request.Method,
                    Path = context.Request.Path,
                    QueryString = context.Request.QueryString.Value,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    DurationMs = stopwatch.ElapsedMilliseconds,
                    UserName = context.User.Identity?.Name,
                    RemoteIp = context.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = context.Request.Headers.UserAgent.ToString(),
                    Exception = ex?.ToString()
                });

                throw;
            }
        }
    }
}
