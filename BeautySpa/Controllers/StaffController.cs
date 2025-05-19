using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.StaffModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Nhân viên bên nhà đăng ký dịch vụ")]
    public class StaffController : ControllerBase
    {
        private readonly IStaff _staffService;

        public StaffController(IStaff staffService)
        {
            _staffService = staffService;
        }

        [HttpPost("create")]
        [SwaggerOperation(Summary = "Tạo mới nhân viên")]
        public async Task<IActionResult> Create([FromBody] POSTStaffModelView model)
        {
            var result = await _staffService.CreateAsync(model);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("update")]
        [SwaggerOperation(Summary = "Cập nhật thông tin nhân viên")]
        public async Task<IActionResult> Update([FromBody] PUTStaffModelView model)
        {
            var result = await _staffService.UpdateAsync(model);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation(Summary = "Xóa mềm nhân viên theo ID")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _staffService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Lấy thông tin nhân viên theo ID")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _staffService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("get-all")]
        [SwaggerOperation(Summary = "Lấy danh sách nhân viên (phân trang)")]
        public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int size, [FromQuery] Guid? providerId = null)
        {
            var result = await _staffService.GetAllAsync(page, size, providerId);
            return StatusCode(result.StatusCode, result);
        }
    }
}