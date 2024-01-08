using CornDome.Models.Articles;

namespace CornDome.ContentManager
{
    public class ContentLoader
    {
        public IEnumerable<Article> GetContentFromStore(string articlePath, string imagePath)
        {
            var files = Directory.EnumerateFiles(articlePath, "*.md", SearchOption.AllDirectories);
            var articles = new List<Article>();

            foreach (var file in files)
            {
                var createdDate = File.GetCreationTime(file);
                var modifiedDate = File.GetLastWriteTime(file);
                var contentFilename = Path.GetFileName(file);
                var title = contentFilename.Replace(".md", "");

                articles.Add(new Article()
                {
                    CreatedDate = createdDate,
                    UpdatedDate = modifiedDate,
                    Title = title,
                    Location = title,
                    ImagePath = Path.Combine(imagePath, $"{title}.png")
                });
            }

            return articles;
        }
    }
}
