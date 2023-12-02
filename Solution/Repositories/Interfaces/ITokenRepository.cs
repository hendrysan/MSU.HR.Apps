using Models.Entities;
using System.Security.Claims;

namespace Repositories.Interfaces
{
    public interface ITokenRepository
    {
        DateTime GetRefreshTokenExpiryTime();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
        string GenerateRefreshToken();
        string CreateToken(DateTime expiryTime);
        List<Claim> CreateClaims();
    }
}
