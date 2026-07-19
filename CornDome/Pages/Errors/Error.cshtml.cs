using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace CornDome.Pages.Errors
{
    public class ErrorModel : PageModel
    {
        public string? RequestId { get; set; }
        public Exception Exception { get; set; }

        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            Exception = HttpContext.Features.Get<IExceptionHandlerPathFeature>()?.Error;
        }
    }
}
