using Markdig;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Resources
{
    public class PDFCreatorModel(IWebHostEnvironment environment) : PageModel
    {
        public string HtmlContent { get; private set; } = string.Empty;

        public void OnGet()
        {
            var readmePath = Path.Combine(
                environment.WebRootPath,
                "pdfcreator.md");

            var markdown = System.IO.File.ReadAllText(readmePath);

            HtmlContent = Markdown.ToHtml(
                markdown,
                new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()
                    .Build());
        }
    }
}