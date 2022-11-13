using System.Security.Claims;
using TMS.Core.Domain.Entities;

namespace TMS.Web.Server.Services
{
    public interface ITokenService
    {
        string GenerateRefreshToken(User user);
        string GenerateJwtToken(User user);
        string GenerateRandomCode();
        ClaimsPrincipal ValidateToken(string jwtToken);
        ClaimsPrincipal ValidateExpiredToken(string token);
    }
}
