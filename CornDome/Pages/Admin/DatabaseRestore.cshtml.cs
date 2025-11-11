using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Admin
{
    [Authorize(Policy = "admin")]
    public class DatabaseRestoreModel(Config config) : PageModel
    {
        [BindProperty]
        public IFormFile CardsDb { get; set; }

        [BindProperty]
        public IFormFile TournamentsDb { get; set; }

        [BindProperty]
        public IFormFile MasterDb { get; set; }

        public string Message { get; set; }

        public void OnGet()
        {
            // Just render the page
        }

        public IActionResult OnPost()
        {
            var uploadedFiles = new[] { CardsDb, MasterDb, TournamentsDb };

            // Validate all files were provided
            if (uploadedFiles.All(f => f == null))
            {
                Message = "Please upload a database file.";
                return Page();
            }

            try
            {
                if (CardsDb != null)
                {
                    Backup(config.DatabasePaths.CardsDb, config.DatabasePaths.BackupLocation);
                    Restore(config.DatabasePaths.CardsDb, CardsDb);
                }

                if (MasterDb != null)
                {
                    Backup(config.DatabasePaths.MasterDb, config.DatabasePaths.BackupLocation);
                    Restore(config.DatabasePaths.MasterDb, MasterDb);
                }

                if (TournamentsDb != null)
                {
                    Backup(config.DatabasePaths.TournamentDb, config.DatabasePaths.BackupLocation);
                    Restore(config.DatabasePaths.TournamentDb, TournamentsDb);
                }

                Message = "Databases uploaded and replaced successfully!";
            }
            catch (Exception ex)
            {
                Message = "Error: " + ex.Message;
            }

            return Page();
        }

        private static void Backup(string origin, string destintation)
        {
            var filename = Path.GetFileName(origin);
            var backedUpFileName = Path.Combine(destintation, filename.Replace(".db", $"-{DateTime.Now:MMddyyyy-HH-mm}.bak"));

            if (!Directory.Exists(destintation))
                Directory.CreateDirectory(destintation);

            if (System.IO.File.Exists(origin))
            {
                System.IO.File.Copy(origin, backedUpFileName, overwrite: true);
            }
        }

        private static void Restore(string path, IFormFile newFile)
        {
            // Save new uploaded file
            using var stream = new FileStream(path, FileMode.Create);
            newFile.CopyTo(stream);
        }
    }
}
