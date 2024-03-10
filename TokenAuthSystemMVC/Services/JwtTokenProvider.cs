using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TokenAuthSystemMVC.Areas.Identity.Data;

namespace TokenAuthSystemMVC.Services
{
    public class JwtTokenProvider : IJwtTokenProvider
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public JwtTokenProvider(IConfiguration configuration, IHttpContextAccessor context, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _context = context;
            _userManager = userManager;
        }

        public async Task<string> GenerateToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, user.UserName ?? "Unknown"),
                new (ClaimTypes.NameIdentifier, user.Id),
            };

            // Add user roles to security claims
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"] ??
                throw new InvalidOperationException("No key for JWT token provided.")));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(5);

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

        public bool IsTokenValid()
        {
            return !string.IsNullOrEmpty(_context?.HttpContext?.Session.GetString("Token"));
        }

        public string GetTokenExpirationTime()
        {
            DateTime expiresUtc = DateTime.MinValue;

            var user = _context?.HttpContext?.User;

            if (user != null)
            {
                // Find expiration time value in claims.
                string? expValue = user.FindFirstValue("exp");

                // Converts a Unix timestamp and returns its equivalent DateTime representation in UTC.
                if (expValue != null && long.TryParse(expValue, out long expUnixTime))
                {
                    expiresUtc = DateTimeOffset.FromUnixTimeSeconds(expUnixTime).UtcDateTime;
                }
            }      

            // Calculate the remaining time until expiration
            TimeSpan timeUntilExpiration = expiresUtc - DateTime.UtcNow;

            return timeUntilExpiration.TotalMinutes.ToString();
        }
    }
}
