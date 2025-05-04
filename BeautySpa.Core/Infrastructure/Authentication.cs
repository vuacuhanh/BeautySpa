using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace BeautySpa.Core.Infrastructure
{
    public static class Authentication
    {
        public static string GetUserIdFromHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor?.HttpContext == null)
                throw new UnauthorizedException("HttpContext is null");

            var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("User Id not found in the token.");

            return userId;
        }

        public static string GetUserIdFromHttpContext(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext), "HttpContext cannot be null");

            return ExtractClaimFromAuthorizationHeader(httpContext.Request.Headers["Authorization"].ToString(), "id");
        }

        public static string GetUserRoleFromHttpContext(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext), "HttpContext cannot be null");

            if (!httpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader) || string.IsNullOrWhiteSpace(authorizationHeader))
                throw new UnauthorizedException("Authorization header is missing or empty.");

            return ExtractClaimFromAuthorizationHeader(authorizationHeader.ToString(), ClaimTypes.Role);
        }

        public static string GetFullNameFromClaims(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor?.HttpContext?.User == null)
                return string.Empty;

            var claimsIdentity = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            return claimsIdentity?.FindFirst("fullName")?.Value ?? string.Empty;
        }

        private static string ExtractClaimFromAuthorizationHeader(string authorizationHeader, string claimType)
        {
            if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedException("Authorization header is missing or not a Bearer token.");

            string jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();
            var tokenHandler = new JwtSecurityTokenHandler();

            if (!tokenHandler.CanReadToken(jwtToken))
                throw new UnauthorizedException("Invalid token format.");

            var token = tokenHandler.ReadJwtToken(jwtToken);
            var claim = token.Claims.FirstOrDefault(c => c.Type == claimType);

            if (claim == null)
                throw new UnauthorizedException($"Claim '{claimType}' not found in the token.");

            return claim.Value;
        }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}
