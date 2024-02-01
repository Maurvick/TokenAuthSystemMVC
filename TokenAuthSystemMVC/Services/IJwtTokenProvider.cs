using TokenAuthSystemMVC.Areas.Identity.Data;

namespace TokenAuthSystemMVC.Services
{
    public interface IJwtTokenProvider
    {
        string GenerateToken(ApplicationUser user);

        string GetToken(string stringToSplit, int chunkSize);

        bool IsTokenValid();

        bool IsTokenExists { get; }
    }
}
