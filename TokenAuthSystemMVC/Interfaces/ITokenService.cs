using TokenAuthSystemMVC.Areas.Identity.Data;

namespace TokenAuthSystemMVC.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(ApplicationUser user);

        string GetToken(string stringToSplit, int chunkSize);

        bool IsTokenValid(string key, string issuer, string token);
    }
}
