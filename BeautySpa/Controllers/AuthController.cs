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
            try
            {
                await _authService.SignUpAsync(model, model.RoleId);
                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: "User registered successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: ResponseCodeConstants.FAILED,
                    data: null,
                    message: ex.Message));
            }
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SignInAuthModelView model)
        {
            try
            {
                var token = await _authService.SignInAsync(model);
                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: token));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: ResponseCodeConstants.FAILED,
                    data: null,
                    message: ex.Message));
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordAuthModelView model)
        {
            try
            {
                var userId = Guid.Parse(Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor));
                await _authService.ChangePasswordAsync(model, userId);
                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: "Password changed successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: ResponseCodeConstants.FAILED,
                    data: null,
                    message: ex.Message));
            }
        }
    }
}