// File: Controllers/AuthController.cs
using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.AuthModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [SwaggerOperation(Summary = "Đăng ký tài khoản bằng email + mật khẩu + xác thực OTP")]
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] SignUpAuthModelView model)
        {
            var result = await _authService.SignUpAsync(model);
            return Ok(result);
        }

        [SwaggerOperation(Summary = "Đăng ký tài khoản với OTP xác minh")]
        [HttpPost("sign-up-otp")]
        public async Task<IActionResult> SignUpWithOtp([FromBody] SignUpAuthModelView model, [FromQuery] string otp)
        {
            var result = await _authService.SignUpWithOtpAsync(model, otp);
            return Ok(result);
        }

        [SwaggerOperation(Summary = "Đăng nhập bằng email + mật khẩu")]
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SignInAuthModelView model)
        {
            var result = await _authService.SignInAsync(model);
            return Ok(result);
        }

        [SwaggerOperation(Summary = "Đăng nhập bằng Google")] 
        [HttpPost("sign-in-google")]
        public async Task<IActionResult> SignInWithGoogle([FromBody] SignInWithGoogleModelView model)
        {
            var result = await _authService.SignInWithGoogleAsync(model);
            return Ok(result);
        }

        [SwaggerOperation(Summary = "Đăng nhập bằng Facebook")]
        [HttpPost("sign-in-facebook")]
        public async Task<IActionResult> SignInWithFacebook([FromBody] SignInWithFacebookModelView model)
        {
            var result = await _authService.SignInWithFacebookAsync(model);
            return Ok(result);
        }

        [SwaggerOperation(Summary = "Yêu cầu gửi OTP về email")]
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

        /// <summary>
        /// Xác thực email với token
        /// </summary>
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var result = await _authService.ConfirmEmailAsync(userId, token);
            return Ok(result);
        }

        /// <summary>
        /// Quên mật khẩu - gửi OTP
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordAuthModelView model)
        {
            var result = await _authService.ForgotPasswordAsync(model);
            return Ok(result);
        }

        /// <summary>
        /// Đặt lại mật khẩu bằng OTP
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordAuthModelView model)
        {
            var result = await _authService.ResetPasswordAsync(model);
            return Ok(result);
        }

        /// <summary>
        /// Đổi mật khẩu khi đã đăng nhập
        /// </summary>
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordAuthModelView model)
        {
            var result = await _authService.ChangePasswordAsync(model);
            return Ok(result);
        }

        /// <summary>
        /// Làm mới AccessToken bằng RefreshToken
        /// </summary>
        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestModelView model)
        {
            var result = await _authService.RefreshTokenAsync(model);
            return Ok(result);
        }
    }
}
