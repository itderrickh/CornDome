using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO.Compression;

namespace CornDome.Pages.Admin
{
    [Authorize(Policy = "admin")]
    public class DatabaseBackupModel(Config config) : PageModel
    {
        // Path to the file you want to serve
        private readonly string _cardsDbPath = config.DatabasePaths.CardsDb;

        public IActionResult OnGet()
        {
            var files = new[] { config.DatabasePaths.CardsDb, config.DatabasePaths.MasterDb, config.DatabasePaths.TournamentDb };
            using var memoryStream = new MemoryStream();

            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var filePath in files)
                {
                    var fileBytes = System.IO.File.ReadAllBytes(filePath);
                    var entry = archive.CreateEntry(Path.GetFileName(filePath));
                    using var entryStream = entry.Open();
                    entryStream.Write(fileBytes, 0, fileBytes.Length);
                }
            }

            memoryStream.Position = 0;
            return File(memoryStream.ToArray(), "application/zip", "db-backup.zip");
        }
    }
}
