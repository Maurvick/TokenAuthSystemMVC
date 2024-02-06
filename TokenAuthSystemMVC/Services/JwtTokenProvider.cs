﻿using Microsoft.IdentityModel.Tokens;
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

        public JwtTokenProvider(IConfiguration configuration, IHttpContextAccessor context)
        {
            _configuration = configuration;
            _context = context;
        }

        public string GenerateToken(ApplicationUser user, IList<string> userRoles)
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, user.UserName!),
                new (ClaimTypes.NameIdentifier, user.Id),
            };

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
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
    }
}
