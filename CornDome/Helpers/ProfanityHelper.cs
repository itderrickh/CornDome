
using DotnetBadWordDetector;

namespace CornDome.Helpers
{
    public static class ProfanityHelper
    {
        private static readonly ProfanityDetector Detector = new();

        public static string RelieveTheProfane(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            return Detector.MaskProfanity(text, '*');
        }
    }
}
