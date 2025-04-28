using BeautySpa.ModelViews.AuthModelViews;
using BeautySpa.Contract.Repositories.Entity;
using Microsoft.AspNetCore.Http;

namespace BeautySpa.Contract.Services.Interface
{
    public interface ITokenService
    {
        Task<TokenResponseModelView> GenerateTokenAsync(ApplicationUsers user, IList<string> roles, string ipAddress, string deviceInfo);
        Task<string> GenerateRefreshTokenAsync(Guid userId);
        Task<bool> ValidateTokenClaimsAsync(HttpContext httpContext);
        Task<string> GetUserIdFromTokenAsync(string accessToken);
        Task<IList<string>> GetRolesFromTokenAsync(string accessToken);
        Task<(string Ip, string Device)> GetIpDeviceFromTokenAsync(string accessToken);
    }
}
