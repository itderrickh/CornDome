using CornDome.Repository;
using Microsoft.Extensions.Configuration;

namespace CornDome.Utilities
{
    internal class ImageCleanupUtility(CardDatabaseContext db, IConfiguration configuration)
    {
        private readonly CardDatabaseContext _db = db;
        private readonly string _directory = configuration["Cards:Images"];
        private List<string> InUseFiles = [];
        private readonly List<string> AllFiles = [];

        public List<string> GetAllFilesNotInUse()
        {
            Console.WriteLine("App starting…");

            GetAllInUseFiles();
            GetAllFiles();

            // Files not in use
            var notInUseFiles = AllFiles.Except(InUseFiles).ToList();

            return notInUseFiles;
        }

        private void GetAllInUseFiles()
        {
            InUseFiles = [.. _db.CardImages.Select(x => Uri.UnescapeDataString(x.ImageUrl))];
        }

        private void GetAllFiles()
        {
            if (!Directory.Exists(_directory))
            {
                Console.WriteLine("Directory does not exist.");
                return;
            }

            var files = Directory.GetFiles(_directory, "*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var fileFormatted = file.Replace(_directory + "\\", "").Replace("\\", "/");
                AllFiles.Add(fileFormatted);
            }
        }
    }
}
