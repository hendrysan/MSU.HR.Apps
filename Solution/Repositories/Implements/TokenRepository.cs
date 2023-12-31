using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Models.Entities;
using Repositories.Interfaces;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Repositories.Implements
{
    public class TokenRepository(IConfiguration configuration) : ITokenRepository
    {
        private readonly IConfiguration _configuration = configuration;
        public List<Claim> CreateClaims(MasterUser user, List<GrantAccess> grants)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Sub, "TokenForTheApiWithAuth"),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString(CultureInfo.InvariantCulture)),
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.FullName ?? string.Empty),
                    new(ClaimTypes.Role, user.Role?.Code ?? string.Empty),
                    new(ClaimTypes.Email, user.Email ?? string.Empty),
                    new("IdNumber", user.IdNumber?? string.Empty),
                    new("Email", user.Email?? string.Empty),
                    new("Email", user.Email?? string.Empty),
                    new("PhoneNumber", user.PhoneNumber?? string.Empty),
                    new("LastLogin", DateTime.Now.ToString()),
                    new("GrantAccess", JsonSerializer.Serialize(grants)),
                };

                return claims;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string CreateToken(MasterUser user, List<GrantAccess> grants, DateTime expiryTime)
        {
            var expiration = expiryTime;
            var token = CreateJwtToken(
                CreateClaims(user, grants),
                CreateSigningCredentials(),
                expiration
            );
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenWrite = tokenHandler.WriteToken(token);
            return tokenWrite.ToString();
        }

        private static JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials,
        DateTime expiration) =>
        new(
            "apiWithAuthBackend",
            "apiWithAuthBackend",
            claims,
            expires: expiration,
            signingCredentials: credentials
        );

        private SigningCredentials CreateSigningCredentials()
        {
            var jwtSecret = _configuration.GetSection("JWT:Secret").Value ?? string.Empty;
            return new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSecret)
                ),
                SecurityAlgorithms.HmacSha256
            );
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var jwtSecret = _configuration.GetSection("JWT:Secret").Value ?? string.Empty;
            var encodeSecret = Encoding.UTF8.GetBytes(jwtSecret);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(encodeSecret),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            IdentityModelEventSource.ShowPII = true;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;

        }

        public DateTime GetRefreshTokenExpiryTime()
        {
            int expirationMinutes = Convert.ToInt32(_configuration.GetSection("Jwt:RefreshTokenExpirationMinutes").Value);
            return DateTime.Now.AddMinutes(expirationMinutes);
        }

    }
}
