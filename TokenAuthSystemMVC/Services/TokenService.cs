using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TokenAuthSystemMVC.Areas.Identity.Data;
using TokenAuthSystemMVC.Interfaces;

namespace TokenAuthSystemMVC.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, user.Id),
                // new (Claim(ClaimTypes.Role, user.Role),
                new (ClaimTypes.Name, user.UserName!),
                // Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"] ??
                throw new InvalidOperationException("No key for JWT token provided.")));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddDays(7);

            var token = new JwtSecurityToken(
                _configuration["JWT:ValidIssuer"],
                _configuration["JWT:ValidAudience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetToken(string stringToSplit, int chunkSize)
        {
            var parts = Enumerable.Range(0, stringToSplit.Length / chunkSize).
                Select(i => stringToSplit.Substring(i * chunkSize, chunkSize));

            string token = "";

            foreach (string str in parts)
            {
                token += Environment.NewLine + str;
            }

            return token;
        }
    }
}
