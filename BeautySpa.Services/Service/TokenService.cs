﻿using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.AuthModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BeautySpa.Services.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IConnectionMultiplexer _redis;

        public TokenService(IConfiguration configuration, IConnectionMultiplexer redis)
        {
            _configuration = configuration;
            _redis = redis;
        }

        public async Task<TokenResponseModelView> GenerateTokenAsync(ApplicationUsers user, IList<string> roles, string ipAddress, string deviceInfo)
        {
            var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()), // custom claim
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // standard claim
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim("ip", ipAddress),
                new Claim("device", deviceInfo)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = await GenerateRefreshTokenAsync(user.Id);

            return new TokenResponseModelView
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiration = expiration
            };
        }

        public async Task<string> GenerateRefreshTokenAsync(Guid userId)
        {
            var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var db = _redis.GetDatabase();
            await db.StringSetAsync($"refresh-token:{userId}", refreshToken, TimeSpan.FromDays(7));
            return refreshToken;
        }

        public Task<bool> ValidateTokenClaimsAsync(HttpContext httpContext)
        {
            var ipClaim = httpContext.User.FindFirst("ip")?.Value;
            var deviceClaim = httpContext.User.FindFirst("device")?.Value;
            var currentIp = httpContext.Connection.RemoteIpAddress?.ToString();
            var currentDevice = httpContext.Request.Headers["User-Agent"].FirstOrDefault()?.ToString();

            var isValid = string.Equals(ipClaim, currentIp, StringComparison.OrdinalIgnoreCase) &&
                          string.Equals(deviceClaim, currentDevice, StringComparison.OrdinalIgnoreCase);

            return Task.FromResult(isValid);
        }

        public Task<string> GetUserIdFromTokenAsync(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                return Task.FromResult(string.Empty);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);
            var userId = token.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier || c.Type == "id")?.Value ?? string.Empty;

            return Task.FromResult(userId);
        }

        public Task<IList<string>> GetRolesFromTokenAsync(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                return Task.FromResult<IList<string>>(new List<string>());

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);
            var roles = token.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            return Task.FromResult<IList<string>>(roles);
        }

        public Task<(string Ip, string Device)> GetIpDeviceFromTokenAsync(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                return Task.FromResult(("", ""));

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);

            var ip = token.Claims.FirstOrDefault(c => c.Type == "ip")?.Value ?? string.Empty;
            var device = token.Claims.FirstOrDefault(c => c.Type == "device")?.Value ?? string.Empty;

            return Task.FromResult((ip, device));
        }
    }
}
