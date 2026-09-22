using CornDome.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;

namespace CornDome.Pages.Errors
{
    public class ErrorModel : BasePageModel
    {
        public string RequestId { get; set; } = string.Empty;
        public Exception Exception { get; set; }

        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            Exception = HttpContext.Features.Get<IExceptionHandlerPathFeature>()?.Error;
        }
    }
}
