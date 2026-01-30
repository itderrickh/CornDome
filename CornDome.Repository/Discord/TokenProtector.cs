using Microsoft.AspNetCore.DataProtection;

namespace CornDome.Repository.Discord
{
    public interface ITokenProtector
    {
        string Protect(string value);
        string Unprotect(string value);
    }

    public class TokenProtector : ITokenProtector
    {
        private readonly IDataProtector _protector;

        public TokenProtector(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("DiscordTokenProtector");
        }

        public string Protect(string value) =>
            value == null ? null : _protector.Protect(value);

        public string Unprotect(string value) =>
            value == null ? null : _protector.Unprotect(value);
    }
}
