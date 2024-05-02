using CornDome.Models.Articles;
using System.Text.Json;

namespace CornDome.ContentManager
{
    public class ContentLoader
    {
        public IEnumerable<Article> GetContentFromStore(string articlePath, string imagePath)
        {
            var articleMap = File.ReadAllText(Path.Combine(articlePath, "articles.json"));
            var jsonMap = JsonSerializer.Deserialize<IList<Article>>(articleMap);
            foreach (var item in jsonMap)
            {
                item.CreatedDate = File.GetCreationTime(Path.Combine(articlePath, item.Location));
                item.UpdatedDate = File.GetLastWriteTime(Path.Combine(articlePath, item.Location));
                item.ImagePath = Path.Combine(imagePath, item.ImagePath);
            }

            return jsonMap.OrderByDescending(x => x.UpdatedDate);
        }
    }
}
