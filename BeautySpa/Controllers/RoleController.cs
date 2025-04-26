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

        // POST: api/role
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Add new Role")]
        public async Task<IActionResult> Create([FromBody] POSTRoleModelViews rolemodel)
        {
            var roleId = await _roleService.CreateAsync(rolemodel);
            return Ok(new BaseResponseModel<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: "Add role successful"
            ));
        }

        

        // PUT: api/role
        [HttpPut]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Update Role")]
        public async Task<IActionResult> Update([FromBody] PUTRoleModelViews rolemodel)
        {

           await _roleService.UpdateAsync(rolemodel);
           return Ok(new BaseResponseModel<string>(
              statusCode: StatusCodes.Status200OK,
              code: ResponseCodeConstants.SUCCESS,
              data: "Update role successful"
           ));
        }
        [HttpDelete]
        [SwaggerOperation(Summary ="Delete Role")]
        public async Task<IActionResult> Delete(Guid roleid)
        {
            await _roleService.DeleteAsync(roleid);
            return Ok(new BaseResponseModel<string>(
                statusCode:StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data:"Delete Role successful"
            ));
        }
    }
}