using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BeautySpa.Core.Infrastructure
{
    public static class Authentication
    {
        public static string GetUserIdFromHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor?.HttpContext == null)
                throw new UnauthorizedException("HttpContext is null");

            var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? httpContextAccessor.HttpContext.User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("User Id not found in the token.");

            return userId;
        }

        public static string GetUserIdFromHttpContext(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext), "HttpContext cannot be null");

            var header = httpContext.Request.Headers["Authorization"].ToString();
            return ExtractUserIdFromAuthorizationHeader(header);
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

        public static string ExtractUserIdFromAuthorizationHeader(string authorizationHeader)
        {
            if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedException("Authorization header is missing or not a Bearer token.");

            string jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();
            var tokenHandler = new JwtSecurityTokenHandler();

            if (!tokenHandler.CanReadToken(jwtToken))
                throw new UnauthorizedException("Invalid token format.");

            var token = tokenHandler.ReadJwtToken(jwtToken);

            var userId = token.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier || c.Type == "sub" || c.Type == "id")?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedException("User ID not found in token.");

            return userId;
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
