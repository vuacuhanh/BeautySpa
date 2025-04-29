using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.RoleModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")] // Chỉ Admin mới quản lý role
    public class RoleController : ControllerBase
    {
        private readonly IRoles _roleService;

        public RoleController(IRoles roleService)
        {
            _roleService = roleService;
        }

        [SwaggerOperation(Summary = "Lấy danh sách role (phân trang)")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var result = await _roleService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [SwaggerOperation(Summary = "Lấy chi tiết role theo Id")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _roleService.GetByIdAsync(id);
            return Ok(result);
        }

        [SwaggerOperation(Summary = "Tạo mới role")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] POSTRoleModelViews model)
        {
            var roleId = await _roleService.CreateAsync(model);
            return Ok(new { id = roleId, message = "Role created successfully." });
        }

        [SwaggerOperation(Summary = "Cập nhật role")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PUTRoleModelViews model)
        {
            await _roleService.UpdateAsync(model);
            return Ok(new { message = "Role updated successfully." });
        }

        [SwaggerOperation(Summary = "Xóa mềm role")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _roleService.DeleteAsync(id);
            return Ok(new { message = "Role deleted successfully." });
        }
    }
}
