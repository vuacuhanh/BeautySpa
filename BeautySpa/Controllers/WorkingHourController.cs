using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.WorkingHourModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Quản lý giờ làm việc của chi nhánh spa")]
    public class WorkingHourController : ControllerBase
    {
        private readonly IWorkingHourService _service;

        public WorkingHourController(IWorkingHourService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lấy danh sách giờ làm việc (có phân trang)")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            return Ok(await _service.GetAllAsync(pageNumber, pageSize));
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy chi tiết giờ làm việc theo ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Tạo giờ làm việc mới cho chi nhánh")]
        public async Task<IActionResult> Create([FromBody] POSTWorkingHourModelViews model)
        {
            return Ok(await _service.CreateAsync(model));
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Cập nhật giờ làm việc của chi nhánh")]
        public async Task<IActionResult> Update([FromBody] PUTWorkingHourModelViews model)
        {
            return Ok(await _service.UpdateAsync(model));
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa giờ làm việc")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _service.DeleteAsync(id));
        }
    }
}
