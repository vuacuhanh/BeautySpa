using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.AuthModelViews;
using Microsoft.AspNetCore.Identity;
using BeautySpa.Contract.Repositories.IUOW;
using Microsoft.Extensions.Configuration;
using BeautySpa.Core.Base;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Contract.Services.Interface;
using StackExchange.Redis;

namespace BeautySpa.Services.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly RoleManager<ApplicationRoles> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConnectionMultiplexer _redis;
        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

        public AuthService(
            UserManager<ApplicationUsers> userManager,
            RoleManager<ApplicationRoles> roleManager,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IMapper mapper,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor,
            IConnectionMultiplexer redis)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _emailService = emailService;
            _roleManager = roleManager;
            _redis = redis;
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

            var userRepo = _unitOfWork.GetRepository<ApplicationUsers>();
            var user = await userRepo.FindAsync(u => u.Email == signin.Email && u.UserInfor != null)
                .ContinueWith(task => task.Result.FirstOrDefault());

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

            var roleRepo = _unitOfWork.GetRepository<ApplicationRoles>();
            var role = await roleRepo.GetByIdAsync(roleId);
            if (role == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Role not found.");

            var user = _mapper.Map<ApplicationUsers>(signup);
            user.UserName = signup.Email;

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var result = await _userManager.CreateAsync(user, signup.Password);
                    if (!result.Succeeded)
                        throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Error occurred during signup: " + string.Join(", ", result.Errors.Select(e => e.Description)));

                    result = await _userManager.AddToRoleAsync(user, role.Name);
                    if (!result.Succeeded)
                        throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Error adding role: " + string.Join(", ", result.Errors.Select(e => e.Description)));

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return await GenerateJwtToken(user);
        }

        private async Task<string?> GenerateJwtToken(ApplicationUsers user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = _configuration["Jwt:Secret"];

            if (string.IsNullOrEmpty(secret))
                throw new ArgumentNullException("Jwt:Secret", "Jwt secret key is missing in configuration.");

            var key = Encoding.ASCII.GetBytes(secret);

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

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Email confirmation failed.");

            return true;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var userRepo = _unitOfWork.GetRepository<ApplicationUsers>();
            var user = await userRepo.FindAsync(u => u.Email == email)
                .ContinueWith(task => task.Result.FirstOrDefault());

            if (user == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = System.Web.HttpUtility.UrlEncode(token);
            var resetLink = $"{_configuration["Frontend:ResetPasswordUrl"]}?email={user.Email}&token={encodedToken}";

            await _emailService.SendEmailAsync(user.Email, "Reset Password",
                $"Click <a href='{resetLink}'>here</a> to reset your password.");

            return true;
        }

        public async Task<bool> ResendConfirmationEmailAsync(string email)
        {
            var userRepo = _unitOfWork.GetRepository<ApplicationUsers>();
            var user = await userRepo.FindAsync(u => u.Email == email)
                .ContinueWith(task => task.Result.FirstOrDefault());

            if (user == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found.");

            if (user.EmailConfirmed)
                throw new BadRequestException(ErrorCode.InvalidInput, "Email already confirmed.");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = System.Web.HttpUtility.UrlEncode(token);
            var confirmationLink = $"{_configuration["Frontend:ConfirmEmailUrl"]}?userId={user.Id}&token={encodedToken}";

            await _emailService.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by clicking <a href='{confirmationLink}'>here</a>.");

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordAuthModelView model)
        {
            var userRepo = _unitOfWork.GetRepository<ApplicationUsers>();
            var user = await userRepo.FindAsync(u => u.Email == model.Email)
                .ContinueWith(task => task.Result.FirstOrDefault());

            if (user == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found.");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput,
                    string.Join(", ", result.Errors.Select(e => e.Description)));

            return true;
        }

        public async Task<bool> RequestOtpAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                throw new BadRequestException(ErrorCode.InvalidInput, "Email is required and must be valid.");

            var userRepo = _unitOfWork.GetRepository<ApplicationUsers>();
            var existingUser = await userRepo.FindAsync(u => u.Email == email)
                .ContinueWith(task => task.Result.FirstOrDefault());

            if (existingUser != null)
                throw new BadRequestException(ErrorCode.InvalidInput, "Email already registered.");

            var db = _redis.GetDatabase();
            var rateKey = $"otp:rate:{email}";
            var count = await db.StringIncrementAsync(rateKey);
            await db.KeyExpireAsync(rateKey, TimeSpan.FromMinutes(1));
            if (count > 5)
                throw new BadRequestException(ErrorCode.InvalidInput, "Too many requests. Please try again later.");

            var otp = new Random().Next(100000, 999999).ToString();
            await db.StringSetAsync($"otp:{email}", otp, TimeSpan.FromMinutes(10));

            var emailBody = $"Your OTP for registration is <b>{otp}</b>. It expires in 10 minutes.";
            await _emailService.SendEmailAsync(email, "BeautySpa Registration OTP", emailBody);

            return true;
        }

        public async Task<string?> SignUpWithOtpAsync(SignUpAuthModelView signup, string otp)
        {
            if (string.IsNullOrWhiteSpace(signup.FullName))
                throw new BadRequestException(ErrorCode.BadRequest, "Full name is required.");

            if (string.IsNullOrWhiteSpace(signup.Email) || !signup.Email.Contains("@"))
                throw new BadRequestException(ErrorCode.InvalidInput, "Email is required and must be valid.");

            if (string.IsNullOrWhiteSpace(signup.Password) || signup.Password.Length < 6)
                throw new BadRequestException(ErrorCode.InvalidInput, "Password must be at least 6 characters long.");

            if (string.IsNullOrWhiteSpace(otp))
                throw new BadRequestException(ErrorCode.InvalidInput, "OTP is required.");

            var db = _redis.GetDatabase();
            var storedOtp = await db.StringGetAsync($"otp:{signup.Email}");

            if (storedOtp.IsNull)
                throw new BadRequestException(ErrorCode.InvalidInput, "OTP is invalid or expired.");

            if (storedOtp != otp)
                throw new BadRequestException(ErrorCode.InvalidInput, "Invalid OTP.");

            await db.KeyDeleteAsync($"otp:{signup.Email}");
            await db.KeyDeleteAsync($"otp:rate:{signup.Email}");

            var customerRoleName = _configuration["DefaultRoles:Customer"];
            var roleRepo = _unitOfWork.GetRepository<ApplicationRoles>();
            var role = await roleRepo.FindAsync(r => r.Name == customerRoleName)
                .ContinueWith(task => task.Result.FirstOrDefault());

            if (role == null)
            {
                role = new ApplicationRoles { Name = customerRoleName, NormalizedName = customerRoleName.ToUpper() };
                await roleRepo.InsertAsync(role);
                await roleRepo.SaveAsync();
            }

            var user = _mapper.Map<ApplicationUsers>(signup);
            user.UserName = signup.Email;

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var result = await _userManager.CreateAsync(user, signup.Password);
                    if (!result.Succeeded)
                        throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Error occurred during signup: " + string.Join(", ", result.Errors.Select(e => e.Description)));

                    result = await _userManager.AddToRoleAsync(user, role.Name);
                    if (!result.Succeeded)
                        throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Error adding role: " + string.Join(", ", result.Errors.Select(e => e.Description)));

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = System.Web.HttpUtility.UrlEncode(token);
            var confirmationLink = $"{_configuration["Frontend:ConfirmEmailUrl"]}?userId={user.Id}&token={encodedToken}";
            await _emailService.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by clicking <a href='{confirmationLink}'>here</a>.");

            return await GenerateJwtToken(user);
        }
    }
}