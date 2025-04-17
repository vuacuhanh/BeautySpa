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
    [SwaggerTag("Vai trò")]
    public class RoleController : ControllerBase
    {
        private readonly IRoles _roleService;

        public RoleController(IRoles roleService)
        {
            _roleService = roleService;
        }

        // POST: api/role
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Tạo role mới")]
        public async Task<IActionResult> Create([FromBody] POSTRoleModelViews model)
        {
            var roleId = await _roleService.CreateAsync(model);
            return Ok(new BaseResponseModel<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: "Add role successful"
            ));
        }

        [HttpGet("all")]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Get All Role")]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize)
        {
           var roles = await _roleService.GetAllAsync(pageNumber, pageSize);
           return Ok(new BaseResponseModel<BasePaginatedList<GETRoleModelViews>>(
              statusCode: StatusCodes.Status200OK,
              code: ResponseCodeConstants.SUCCESS,
              data: roles
           ));
        }

        [HttpGet("{id}")]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Get Role By ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
           var role = await _roleService.GetByIdAsync(id);
           return Ok(new BaseResponseModel<GETRoleModelViews>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: role
           ));
        }

        // PUT: api/role
        [HttpPut]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Update Role")]
        public async Task<IActionResult> Update([FromBody] PUTRoleModelViews model)
        {

           await _roleService.UpdateAsync(model);
           return Ok(new BaseResponseModel<string>(
              statusCode: StatusCodes.Status200OK,
              code: ResponseCodeConstants.SUCCESS,
              data: "Update role successful"
           ));
        }
    }
}