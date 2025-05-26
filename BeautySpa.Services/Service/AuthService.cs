using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.AuthModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BeautySpa.Services.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly RoleManager<ApplicationRoles> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConnectionMultiplexer _redis;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        public AuthService(
            UserManager<ApplicationUsers> userManager,
            RoleManager<ApplicationRoles> roleManager,
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IConnectionMultiplexer redis,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _redis = redis;
            _emailService = emailService;
            _configuration = configuration;
        }

        private string CurrentIp => _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "UnknownIP";
        private string CurrentDevice => _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString() ?? "UnknownDevice";
        //================================================================================================================================
        public async Task<BaseResponseModel<string>> SignUpAsync(SignUpAuthModelView model)
        {
            var db = _redis.GetDatabase();
            var verified = await db.StringGetAsync($"otp:verified:{model.Email}");
            if (verified.IsNullOrEmpty || verified != "true")
                throw new BadRequestException(ErrorCode.InvalidInput, "Email has not been verified.");

            var user = _mapper.Map<ApplicationUsers>(model);
            user.UserName = model.Email;
            user.Status = "active";

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.BadRequest, string.Join("; ", result.Errors.Select(x => x.Description)));

            var customerRole = _configuration["DefaultRoles:Customer"];
            if (string.IsNullOrEmpty(customerRole))
            {
                await _userManager.DeleteAsync(user);
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.InternalServerError, "Default Customer role is not configured.");
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, customerRole);
            if (!addToRoleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.BadRequest, string.Join("; ", addToRoleResult.Errors.Select(x => x.Description)));
            }

            var userInfor = new UserInfor
            {
                UserId = user.Id,
                FullName = model.FullName
            };
            await _unitOfWork.GetRepository<UserInfor>().InsertAsync(userInfor);

            var rankQuery = _unitOfWork.GetRepository<Rank>().Entities
                .Where(r => r.DeletedTime == null)
                .OrderBy(r => r.MinPoints);

            var defaultRank = await rankQuery.FirstOrDefaultAsync();
            if (defaultRank == null)
            {
                await _userManager.DeleteAsync(user);
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.InternalServerError, "Default rank not found.");
            }

            var membership = new MemberShip
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RankId = defaultRank.Id,
                AccumulatedPoints = 0,
                CreatedBy = user.Id.ToString(),
                CreatedTime = CoreHelper.SystemTimeNow
            };
            await _unitOfWork.GetRepository<MemberShip>().InsertAsync(membership);

            await _unitOfWork.SaveAsync();
            await db.KeyDeleteAsync($"otp:{model.Email}");
            await db.KeyDeleteAsync($"otp:verified:{model.Email}");

            return BaseResponseModel<string>.Success("Sign up successfully.");
        }

        //==============================================================================================================================
        public async Task<BaseResponseModel<TokenResponseModelView>> SignInAsync(SignInAuthModelView model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                throw new BadRequestException(ErrorCode.UnAuthorized, "Invalid email or password.");

            var roles = await _userManager.GetRolesAsync(user);
            var token = await _tokenService.GenerateTokenAsync(user, roles, CurrentIp, CurrentDevice);

            return BaseResponseModel<TokenResponseModelView>.Success(token);
        }
        //==============================================================================================================================
        public async Task<BaseResponseModel<TokenResponseModelView>> SignInWithGoogleAsync(SignInWithGoogleModelView model)
        {
            var mockUser = new ApplicationUsers { Id = Guid.NewGuid(), Email = "google@example.com" };
            var token = await _tokenService.GenerateTokenAsync(mockUser, new List<string> { "Customer" }, CurrentIp, CurrentDevice);
            return BaseResponseModel<TokenResponseModelView>.Success(token);
        }
        //==============================================================================================================================
        public async Task<BaseResponseModel<string>> RequestOtpAsync(string email)
        {

            if (string.IsNullOrWhiteSpace(email))
                throw new BadRequestException(ErrorCode.InvalidInput, "Email cannot be empty.");

            var otp = new Random().Next(100000, 999999).ToString();
            var payload = new OtpVerifyModelView
            {
                OtpCode = otp,
                IpAddress = CurrentIp,
                DeviceInfo = CurrentDevice
            };

            var db = _redis.GetDatabase();
            await db.StringSetAsync($"otp:{email}", JsonConvert.SerializeObject(payload), TimeSpan.FromMinutes(5));

            await _emailService.SendEmailAsync(email, "Your OTP Code", $"Your verification OTP is: <b>{otp}</b>");

            return BaseResponseModel<string>.Success("OTP sent successfully.");
        }
        //==============================================================================================================================
        public async Task<BaseResponseModel<string>> ForgotPasswordAsync(ForgotPasswordAuthModelView model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                throw new BadRequestException(ErrorCode.NotFound, "Email not found.");

            var otp = new Random().Next(100000, 999999).ToString();
            var payload = new OtpVerifyModelView
            {
                OtpCode = otp,
                IpAddress = CurrentIp,
                DeviceInfo = CurrentDevice
            };

            var db = _redis.GetDatabase();
            await db.StringSetAsync($"otp:reset-password:{model.Email}", JsonConvert.SerializeObject(payload), TimeSpan.FromMinutes(5));

            await _emailService.SendEmailAsync(model.Email, "Reset Password OTP", $"Your password reset OTP is: <b>{otp}</b>");

            return BaseResponseModel<string>.Success("OTP sent for password reset.");
        }
        //==============================================================================================================================
        public async Task<BaseResponseModel<string>> ResetPasswordAsync(ResetPasswordAuthModelView model)
        {
            var db = _redis.GetDatabase();

            // Kiểm tra OTP đã được xác thực trước đó
            var isVerified = await db.StringGetAsync($"otp:verified:reset-password:{model.Email}");
            if (isVerified.IsNullOrEmpty || isVerified != "true")
                throw new BadRequestException(ErrorCode.InvalidInput, "OTP not verified or expired.");

            // Kiểm tra xác nhận mật khẩu trùng khớp
            if (model.NewPassword != model.ConfirmNewPassword)
                throw new BadRequestException(ErrorCode.InvalidInput, "Passwords do not match.");

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                throw new BadRequestException(ErrorCode.NotFound, "User not found.");

            // Reset mật khẩu
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!result.Succeeded)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.BadRequest, string.Join(", ", result.Errors.Select(x => x.Description)));

            // Xoá OTP sau khi thành công
            await db.KeyDeleteAsync($"otp:reset-password:{model.Email}");
            await db.KeyDeleteAsync($"otp:verified:reset-password:{model.Email}");

            return BaseResponseModel<string>.Success("Password reset successfully.");
        }

        //================================================================================================================================================================
        public async Task<BaseResponseModel<string>> VerifyResetPasswordOtpAsync(OtpVerifyModelView model)
        {
            var db = _redis.GetDatabase();
            var cache = await db.StringGetAsync($"otp:reset-password:{model.Email}");
            if (cache.IsNullOrEmpty)
                throw new BadRequestException(ErrorCode.InvalidInput, "OTP expired or not found.");

            var payload = JsonConvert.DeserializeObject<OtpVerifyModelView>(cache!);
            if (payload == null || payload.OtpCode != model.OtpCode || payload.IpAddress != CurrentIp || payload.DeviceInfo != CurrentDevice)
                throw new BadRequestException(ErrorCode.InvalidInput, "OTP validation failed.");

            await db.StringSetAsync($"otp:verified:reset-password:{model.Email}", "true", TimeSpan.FromMinutes(10));
            return BaseResponseModel<string>.Success("Reset password OTP verified successfully.");
        }
        //==================================================================================================================================================================
        public async Task<BaseResponseModel<string>> VerifyOtpAsync(OtpVerifyModelView model)
        {
            var db = _redis.GetDatabase();
            var cache = await db.StringGetAsync($"otp:{model.Email}");
            if (cache.IsNullOrEmpty)
                throw new BadRequestException(ErrorCode.InvalidInput, "OTP expired or not found.");

            var payload = JsonConvert.DeserializeObject<OtpVerifyModelView>(cache!);
            if (payload == null || payload.OtpCode != model.OtpCode || payload.IpAddress != CurrentIp || payload.DeviceInfo != CurrentDevice)
                throw new BadRequestException(ErrorCode.InvalidInput, "OTP validation failed.");

            var isVerified = await db.StringGetAsync($"otp:verified:{model.Email}");
            if (isVerified == "true") return BaseResponseModel<string>.Success("OTP already verified.");

            await db.StringSetAsync($"otp:verified:{model.Email}", "true", TimeSpan.FromMinutes(10));
            return BaseResponseModel<string>.Success("OTP verified successfully.");
        }

        //==============================================================================================================================
        public async Task<BaseResponseModel<string>> ChangePasswordAsync(ChangePasswordAuthModelView model)
        {
            var userId = Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new BadRequestException(ErrorCode.NotFound, "User not found.");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.BadRequest, string.Join(", ", result.Errors.Select(x => x.Description)));

            return BaseResponseModel<string>.Success("Password changed successfully.");
        }
        //=================================================================================================================================
        public async Task<BaseResponseModel<string>> ResendConfirmEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new BadRequestException(ErrorCode.NotFound, "User not found.");

            if (user.EmailConfirmed)
                throw new BadRequestException(ErrorCode.BadRequest, "Email already confirmed.");

            var otp = new Random().Next(100000, 999999).ToString();

            var payload = new OtpVerifyModelView
            {
                OtpCode = otp,
                IpAddress = CurrentIp,
                DeviceInfo = CurrentDevice
            };

            var db = _redis.GetDatabase();
            await db.StringSetAsync($"otp:confirm-email:{email}", JsonConvert.SerializeObject(payload), TimeSpan.FromMinutes(10));

            await _emailService.SendEmailAsync(email, "Your Confirm Email OTP", $"Your email confirmation OTP is: <b>{otp}</b>");

            return BaseResponseModel<string>.Success("Confirmation OTP sent successfully.");
        }
        //==================================================================================================================================
        public async Task<BaseResponseModel<string>> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new BadRequestException(ErrorCode.NotFound, "User not found.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.BadRequest, "Confirm email failed.");

            return BaseResponseModel<string>.Success("Email confirmed successfully.");
        }
        //==============================================================================================================================
        public async Task<BaseResponseModel<TokenResponseModelView>> RefreshTokenAsync(RefreshTokenRequestModelView model)
        {
            var db = _redis.GetDatabase();
            var userId = Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
            var savedToken = await db.StringGetAsync($"refresh-token:{userId}");

            if (savedToken.IsNullOrEmpty || savedToken != model.RefreshToken)
                throw new BadRequestException(ErrorCode.UnAuthorized, "Invalid refresh token.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new BadRequestException(ErrorCode.NotFound, "User not found.");

            var roles = await _userManager.GetRolesAsync(user);
            var token = await _tokenService.GenerateTokenAsync(user, roles, CurrentIp, CurrentDevice);

            return BaseResponseModel<TokenResponseModelView>.Success(token);
        }
    }
}
