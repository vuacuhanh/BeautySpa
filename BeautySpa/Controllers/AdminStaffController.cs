using BeautySpa.ModelViews.StaffAdminModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/admin-staff")]
[SwaggerTag("Nhân viên bên hệ thống làm đẹp")]
public class AdminStaffController : ControllerBase
{
    private readonly AdminStaffService _service;

    public AdminStaffController(AdminStaffService service)
    {
        _service = service;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách AdminStaff (phân trang)")]
    public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int size)
    {
        return Ok(await _service.GetAllAsync(page, size));
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Lấy chi tiết một AdminStaff theo ID")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Ok(await _service.GetByIdAsync(id));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo mới một AdminStaff")]
    public async Task<IActionResult> Create([FromBody] POSTAdminStaffModelView model)
    {
        return Ok(await _service.CreateAsync(model));
    }

    [HttpPut]
    [SwaggerOperation(Summary = "Cập nhật thông tin AdminStaff")]
    public async Task<IActionResult> Update([FromBody] PUTAdminStaffModelView model)
    {
        return Ok(await _service.UpdateAsync(model));
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Xóa mềm một AdminStaff theo ID")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Ok(await _service.DeleteAsync(id));
    }
}
