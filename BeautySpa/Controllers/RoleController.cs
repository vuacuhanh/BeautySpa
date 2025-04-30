using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.RoleModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")] // Chỉ Admin mới quản lý role
    [SwaggerTag("Quản lý vai trò người dùng")]
    public class RoleController : ControllerBase
    {
        private readonly IRoles _roleService;

        public RoleController(IRoles roleService)
        {
            _roleService = roleService;
        }

        [SwaggerOperation(Summary = "Lấy danh sách các role (phân trang)")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _roleService.GetAllAsync(pageNumber, pageSize));
        }

        [SwaggerOperation(Summary = "Lấy thông tin chi tiết role theo ID")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            return Ok(await _roleService.GetByIdAsync(id));
        }

        [SwaggerOperation(Summary = "Tạo mới role")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] POSTRoleModelViews model)
        {
            return Ok(await _roleService.CreateAsync(model));
        }

        [SwaggerOperation(Summary = "Cập nhật role")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PUTRoleModelViews model)
        {
            return Ok(await _roleService.UpdateAsync(model));
        }

        [SwaggerOperation(Summary = "Xóa mềm role theo ID")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            return Ok(await _roleService.DeleteAsync(id));
        }
    }
}
