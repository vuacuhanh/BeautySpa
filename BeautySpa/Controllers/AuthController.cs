using BeautySpa.Core.Base;
using BeautySpa.ModelViews.AuthModelViews;
using BeautySpa.Services.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BeautySpa.Core.Infrastructure;

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
        public async Task<IActionResult> SignUp([FromBody] SignUpAuthModelView model, string roleName)
        {
            await _authService.SignUpAsync(model, roleName);
            return Ok(new BaseResponseModel<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
            data: "User registered successfully."));
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SignInAuthModelView model)
        {
            var token = await _authService.SignInAsync(model);
            return Ok(new BaseResponseModel<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: token));
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
                data: "Password changed successfully."));
        }
    }
}
