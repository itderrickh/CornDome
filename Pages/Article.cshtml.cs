using CornDome.ContentManager;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages
{
    public class ArticleModel(IConfiguration configuration) : PageModel
    {
        public required string MdLocation { get; set; } = configuration.GetValue<string>("ContentStore:Articles") ?? "";
        public string MarkdownContent { get; set; } = string.Empty;
        [FromQuery(Name = "page")]
        public string MarkdownPage { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public IActionResult OnGet()
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var article = string.IsNullOrEmpty(MarkdownPage) ? "Index" : MarkdownPage;
            var path = Path.Join(MdLocation, $"{article}.md");

            var content = FileReader.SafeFileRead(MdLocation, path);
            if (content == null)
                return Redirect("Errors/Error404");

            Created = System.IO.File.GetCreationTime(path);
            Modified = System.IO.File.GetLastWriteTime(path);

            Title = article;
            MarkdownContent = Markdown.ToHtml(content, pipeline);

            return Page();
        }
    }
}
