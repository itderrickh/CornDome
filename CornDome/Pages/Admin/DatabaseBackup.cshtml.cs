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
                    var entry = archive.CreateEntry(Path.GetFileName(filePath));

                    using var entryStream = entry.Open();
                    // Open file with read-only and shared access
                    using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    fileStream.CopyTo(entryStream);
                }
            }

            memoryStream.Position = 0;
            return File(memoryStream.ToArray(), "application/zip", "db-backup.zip");
        }
    }
}
