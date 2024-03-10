using TokenAuthSystemMVC.Areas.Identity.Data;

namespace TokenAuthSystemMVC.Services
{
    public interface IJwtTokenProvider
    {
        Task<string> GenerateToken(ApplicationUser user);

        string GetToken(string stringToSplit, int chunkSize);

        string GetTokenExpirationTime();

        bool IsTokenValid();
    }
}
