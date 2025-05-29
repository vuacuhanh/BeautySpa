using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.StaffModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Quản lý nhân viên của nhà cung cấp dịch vụ")]
    public class StaffController : ControllerBase
    {
        private readonly IStaff _staffService;

        public StaffController(IStaff staffService)
        {
            _staffService = staffService;
        }

        [HttpGet("get-all")]
        [SwaggerOperation(Summary = "Lấy danh sách nhân viên (provider hiện tại)")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _staffService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy thông tin nhân viên theo ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _staffService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Tạo mới nhân viên")]
        public async Task<IActionResult> Create([FromBody] POSTStaffModelView model)
        {
            var result = await _staffService.CreateAsync(model);
            return Ok(result);
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Cập nhật thông tin nhân viên")]
        public async Task<IActionResult> Update([FromBody] PUTStaffModelView model)
        {
            var result = await _staffService.UpdateAsync(model);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xoá nhân viên (soft delete)")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _staffService.DeleteAsync(id);
            return Ok(result);
        }

    }
}