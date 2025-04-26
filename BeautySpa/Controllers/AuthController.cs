using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.ModelViews.AuthModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(IAuthService authService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] SignUpAuthModelView model)
        {
            await _authService.SignUpAsync(model, model.RoleId);
            return Ok(new BaseResponseModel<string>(
                 statusCode: StatusCodes.Status200OK,
                  code: ResponseCodeConstants.SUCCESS,
                  data: "User registered successfully."
            ));
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SignInAuthModelView model)
        {
            var token = await _authService.SignInAsync(model);
            return Ok(new BaseResponseModel<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: token
            ));
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordAuthModelView model)
        {
            var userId = Guid.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
            await _authService.ChangePasswordAsync(model, userId);
            return Ok(new BaseResponseModel<string>(
                  statusCode: StatusCodes.Status200OK,
                  code: ResponseCodeConstants.SUCCESS,
                  data: "Password changed successfully."
            ));
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            await _authService.ConfirmEmailAsync(userId, token);
            return Ok(new BaseResponseModel<string>(
                statusCode: StatusCodes.Status200OK,
                 code: ResponseCodeConstants.SUCCESS,
                data: "Email confirmed successfully."));
        }

        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] string email)
        {
            await _authService.ResendConfirmationEmailAsync(email);
            return Ok(new BaseResponseModel<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: "Confirmation email sent."));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            await _authService.ForgotPasswordAsync(email);
            return Ok(new BaseResponseModel<string>(
               statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: "Reset password email sent."));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordAuthModelView model)
        {
            await _authService.ResetPasswordAsync(model);
            return Ok(new BaseResponseModel<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: "Password reset successful."));
        }
    }
}