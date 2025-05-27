// File: Controllers/AuthController.cs
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.AuthModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Đăng nhập / Đăng ký")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [SwaggerOperation(Summary = "Đăng ký tài khoản với OTP xác thực bắt buộc")]
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] SignUpAuthModelView model)
        {
            var result = await _authService.SignUpAsync(model);
            return Ok(result);
        }

        [SwaggerOperation(Summary = "Đăng nhập bằng email và mật khẩu")]
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SignInAuthModelView model)
        {
            var result = await _authService.SignInAsync(model);
            return Ok(result);
        }

        [SwaggerOperation(Summary = "Xác minh OTP trước khi đăng ký")]
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyModelView model)
        {
            var result = await _authService.VerifyOtpAsync(model);
            return Ok(result);
        }

        [SwaggerOperation(Summary = "Đăng nhập bằng Google")]
        [HttpPost("sign-in-google")]
        public async Task<IActionResult> SignInWithGoogle([FromBody] SignInWithGoogleModelView model)
        {
            var result = await _authService.SignInWithGoogleAsync(model);
            return Ok(result);
        }

        [SwaggerOperation(Summary = "Yêu cầu gửi OTP tới Email để xác thực hoặc đăng ký")]
        [HttpPost("request-otp")]
        public async Task<IActionResult> RequestOtp([FromBody] ResendOtpRequestModelView model)
        {
            var result = await _authService.RequestOtpAsync(model.Email);
            return Ok(result);
        }

        [SwaggerOperation(Summary = "Gửi lại email xác thực tài khoản")]
        [HttpPost("resend-confirm-email")]
        public async Task<IActionResult> ResendConfirmEmail([FromBody] ResendOtpRequestModelView model)
        {
            var result = await _authService.ResendConfirmEmailAsync(model.Email);
            return Ok(result);
        }

        [SwaggerOperation(Summary = "Xác nhận email bằng token")]
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var result = await _authService.ConfirmEmailAsync(userId, token);
            return Ok(result);
        }

        [SwaggerOperation(Summary = "Quên mật khẩu - gửi OTP reset")]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordAuthModelView model)
        {
            var result = await _authService.ForgotPasswordAsync(model);
            return Ok(result);
        }

        [SwaggerOperation(Summary = "Xác minh OTP trước khi đặt lại mật khẩu")]
        [HttpPost("verify-reset-password-otp")]
        public async Task<IActionResult> VerifyResetPasswordOtp([FromBody] OtpVerifyModelView model)
        {
            var result = await _authService.VerifyResetPasswordOtpAsync(model);
            return Ok(result);
        }

        [SwaggerOperation(Summary = "Đặt lại mật khẩu bằng OTP")]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordAuthModelView model)
        {
            var result = await _authService.ResetPasswordAsync(model);
            return Ok(result);
        }

        [Authorize]
        [SwaggerOperation(Summary = "Đổi mật khẩu sau khi đăng nhập")]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordAuthModelView model)
        {
            var result = await _authService.ChangePasswordAsync(model);
            return Ok(result);
        }

        [Authorize]
        [SwaggerOperation(Summary = "Làm mới AccessToken bằng RefreshToken")]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestModelView model)
        {
            var result = await _authService.RefreshTokenAsync(model);
            return Ok(result);
        }
    }
}
