using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.RoleModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoles _roleService;

        public RoleController(IRoles roleService)
        {
            _roleService = roleService;
        }

        // POST: api/role
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Tạo role mới")]
        public async Task<IActionResult> Create([FromBody] POSTRoleModelViews model)
        {
            try
            {
                var roleId = await _roleService.CreateAsync(model);
                return Ok(new BaseResponseModel<Guid>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: roleId,
                    message: "Role created successfully."
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: ResponseCodeConstants.FAILED,
                    data: null,
                    message: ex.Message
                ));
            }
        }

        // GET: api/role?pageNumber=1&pageSize=10
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Get All Role")]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize)
        {
            try
            {
                var roles = await _roleService.GetAllAsync(pageNumber, pageSize);
                return Ok(new BaseResponseModel<BasePaginatedList<GETRoleModelViews>>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: roles
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: ResponseCodeConstants.FAILED,
                    data: null,
                    message: ex.Message
                ));
            }
        }

        // GET: api/role/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Get Role By ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var role = await _roleService.GetByIdAsync(id);
                return Ok(new BaseResponseModel<GETRoleModelViews>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: role
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: ResponseCodeConstants.FAILED,
                    data: null,
                    message: ex.Message
                ));
            }
        }

        // PUT: api/role
        [HttpPut]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Update Role")]
        public async Task<IActionResult> Update([FromBody] PUTRoleModelViews model)
        {
            try
            {
                await _roleService.UpdateAsync(model);
                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: "Update role successful"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: ResponseCodeConstants.FAILED,
                    data: null,
                    message: ex.Message
                ));
            }
        }
    }
}