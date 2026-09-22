using CornDome.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CornDome.Pages.Errors
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class Error404Model : BasePageModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly ILogger<Error404Model> _logger;

        public Error404Model(ILogger<Error404Model> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}
