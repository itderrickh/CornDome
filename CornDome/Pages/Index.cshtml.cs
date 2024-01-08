using CornDome.ContentManager;
using CornDome.Models.Articles;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages
{
    public class IndexModel(IConfiguration config, ILogger<IndexModel> logger) : PageModel
    {
        private readonly ILogger<IndexModel> _logger = logger;
        private readonly ContentLoader _contentLoader = new();
        private readonly IConfiguration _config = config;
        public IEnumerable<Article> Articles = new List<Article>();

        public string GetFinalDirectory(Article article) => $"Article?page={article.Location.Replace(".md", "")}";

        public void OnGet()
        {
            var articlePath = _config.GetValue<string>("ContentStore:Articles");
            if (articlePath == null)
            {
                _logger.LogError($"Configuration not set: \"ContentStore:Articles\"");
                return;
            }

            Articles = _contentLoader.GetContentFromStore(articlePath, "/EmbeddedImages");
        }
    }
}
