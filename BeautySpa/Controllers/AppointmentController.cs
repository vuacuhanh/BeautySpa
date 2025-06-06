using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.AppointmentModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Quản lý lịch hẹn dịch vụ Spa")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _service;

        public AppointmentController(IAppointmentService service)
        {
            _service = service;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Tạo lịch hẹn mới")]
        public async Task<IActionResult> Create([FromBody] POSTAppointmentModelView model)
        {
            var result = await _service.CreateAsync(model);
            return Ok(result);
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Cập nhật thông tin lịch hẹn")]
        public async Task<IActionResult> Update([FromBody] PUTAppointmentModelView model)
        {
            var result = await _service.UpdateAsync(model);
            return Ok(result);
        }

        [HttpPatch("status/{id}")]
        [SwaggerOperation(Summary = "Cập nhật trạng thái lịch hẹn (confirmed, checked_in, completed, canceled, no_show)")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] string status)
        {
            var result = await _service.UpdateStatusAsync(id, status);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa mềm lịch hẹn")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return Ok(result);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lấy danh sách lịch hẹn (có phân trang)")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var result = await _service.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy chi tiết lịch hẹn theo ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("auto-cancel")]
        [SwaggerOperation(Summary = "Tự động hủy các lịch chưa thanh toán sau 10 phút")]
        public async Task<IActionResult> AutoCancelUnpaid()
        {
            var result = await _service.AutoCancelUnpaidAppointmentsAsync();
            return Ok(result);
        }

        [HttpPost("auto-no-show")]
        [SwaggerOperation(Summary = "Tự động đánh dấu no_show các lịch quá 12 tiếng chưa xử lý")]
        public async Task<IActionResult> AutoNoShow()
        {
            var result = await _service.AutoNoShowAfter12HoursAsync();
            return Ok(result);
        }
        [HttpGet("by-current-user")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetByCurrentUser()
        {
            var result = await _service.GetByCurrentUserAsync();
            return Ok(result);
        }

        [HttpPatch("cancel-by-user/{id}")]
        [Authorize(Roles = "Customer")]
        [SwaggerOperation(Summary = "Người dùng hủy lịch trong 5 phút sau khi đặt (nếu chưa được xác nhận)")]
        public async Task<IActionResult> CancelByUser(Guid id)
        {
            var result = await _service.CancelByUserAsync(id);
            return Ok(result);
        }
    }
}
