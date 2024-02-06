using TokenAuthSystemMVC.Areas.Identity.Data;

namespace TokenAuthSystemMVC.Services
{
    public interface IJwtTokenProvider
    {
        string GenerateToken(ApplicationUser user, IList<string> userRoles);

        string GetToken(string stringToSplit, int chunkSize);

        bool IsTokenValid();
    }
}
