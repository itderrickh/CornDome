using CornDome.Repository;
using Microsoft.Extensions.Configuration;

namespace CornDome.Utilities;
public class App(CardDatabaseContext db, IConfiguration configuration)
{
    private readonly string imageDirectory = configuration["Cards:Images"];
    public async Task RunAsync()
    {
        ImageCleanupUtility imageCleanupUtility = new(db, configuration);

        var niuDirectory = Path.Combine(imageDirectory, "notinuse");
        if (!Directory.Exists(niuDirectory))
        {
            Directory.CreateDirectory(niuDirectory);
        }

        var filesNotInUse = imageCleanupUtility.GetAllFilesNotInUse();

        var fullPath = filesNotInUse.Select(x => Path.Combine(imageDirectory, x));
        long totalBytes = fullPath
            .Where(File.Exists)               // Make sure file exists
            .Sum(file => new FileInfo(file).Length);

        Console.WriteLine($"Total Potential Savings: {(double)(totalBytes / (1024.0 * 1024.0 * 1024.0))} GB");
        foreach (var file in filesNotInUse)
        {
            var priorLocation = Path.Combine(imageDirectory, file).Replace("/", "\\");
            var finalLocation = Path.Combine(niuDirectory, file).Replace("/", "\\");
            File.Move(priorLocation, finalLocation);
            Console.WriteLine($"Moving from {priorLocation} to {finalLocation}");
        }
    }
}