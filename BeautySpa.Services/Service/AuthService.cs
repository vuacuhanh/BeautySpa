﻿using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.AuthModelViews;
using Microsoft.AspNetCore.Identity;
using BeautySpa.Contract.Repositories.IUOW;
using Microsoft.Extensions.Configuration;
using BeautySpa.Core.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeautySpa.Services.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly RoleManager<ApplicationRoles> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthService(
            UserManager<ApplicationUsers> userManager,
            RoleManager<ApplicationRoles> roleManager,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordAuthModelView changepass, Guid UserId)
        {
            if (string.IsNullOrWhiteSpace(changepass.CurrentPassword))
                throw new BadRequestException(ErrorCode.InvalidInput, "Current password cannot be empty.");

            if (string.IsNullOrWhiteSpace(changepass.NewPassword))
                throw new BadRequestException(ErrorCode.InvalidInput, "New password cannot be empty.");

            if (changepass.NewPassword.Length < 6)
                throw new BadRequestException(ErrorCode.InvalidInput, "New password must be at least 6 characters long.");

            if (changepass.NewPassword != changepass.ConfirmPassword)
                throw new BadRequestException(ErrorCode.InvalidInput, "New password and confirm password do not match.");

            var user = await _userManager.FindByIdAsync(UserId.ToString());
            if (user == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found.");

            var result = await _userManager.ChangePasswordAsync(user, changepass.CurrentPassword, changepass.NewPassword);
            if (!result.Succeeded)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Error occurred while changing password: " + string.Join(", ", result.Errors.Select(e => e.Description)));

            return true;
        }

        public async Task<IList<string>> GetUserRolesAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found.");

            return await _userManager.GetRolesAsync(user);
        }

        public async Task<string?> SignInAsync(SignInAuthModelView signin)
        {
            if (signin.Email == null || !signin.Email.Contains("@"))
                throw new BadRequestException(ErrorCode.InvalidInput, "Email is required and must be valid.");

            if (string.IsNullOrWhiteSpace(signin.Password))
                throw new BadRequestException(ErrorCode.InvalidInput, "Password cannot be empty.");

            var user = await _userManager.Users
                         .Include(u => u.UserInfor)
                         .FirstOrDefaultAsync(u => u.Email == signin.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, signin.Password))
                throw new BadRequestException(ErrorCode.InvalidInput, "Invalid email or password.");

            return await GenerateJwtToken(user);
        }

        public async Task<string?> SignUpAsync(SignUpAuthModelView signup, Guid roleId)
        {
            if (string.IsNullOrWhiteSpace(signup.FullName))
                throw new BadRequestException(ErrorCode.BadRequest, "Full name is required.");

            if (string.IsNullOrWhiteSpace(signup.Email) || !signup.Email.Contains("@"))
                throw new BadRequestException(ErrorCode.InvalidInput, "Email is required and must be valid.");

            if (string.IsNullOrWhiteSpace(signup.Password) || signup.Password.Length < 6)
                throw new BadRequestException(ErrorCode.InvalidInput, "Password must be at least 6 characters long.");

            // Kiểm tra vai trò tồn tại
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Role not found.");
            }

            var user = _mapper.Map<ApplicationUsers>(signup);
            user.UserName = signup.Email;

            var result = await _userManager.CreateAsync(user, signup.Password);

            if (result.Succeeded)
            {
                result = await _userManager.AddToRoleAsync(user, role.Name);
                if (!result.Succeeded)
                    throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Error adding role: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                return await GenerateJwtToken(user);
            }

            throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Error occurred during signup: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        private async Task<string?> GenerateJwtToken(ApplicationUsers user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = _configuration["Jwt:Secret"];

            if (string.IsNullOrEmpty(secret))
                throw new ArgumentNullException("Jwt:Secret", "Jwt secret key is missing in configuration.");

            var key = Encoding.ASCII.GetBytes(secret);

            // Lấy danh sách vai trò của người dùng
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim("fullName", user.UserInfor?.FullName ?? "Unknown")
            };

            claims.AddRange(roleClaims);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}