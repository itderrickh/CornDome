using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages
{
    public class IndexModel(IConfiguration config, ILogger<IndexModel> logger) : PageModel
    {
        private readonly ILogger<IndexModel> _logger = logger;
        private readonly IConfiguration _config = config;

        public void OnGet()
        {
        }
    }
}
