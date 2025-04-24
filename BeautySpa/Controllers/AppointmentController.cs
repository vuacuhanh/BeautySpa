using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.AppointmentModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Lịch hẹn dịch vụ")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _service;

        public AppointmentController(IAppointmentService service)
        {
            _service = service;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Tạo lịch hẹn")]
        public async Task<IActionResult> Create([FromBody] POSTAppointmentModelViews model)
        {
            var id = await _service.CreateAsync(model);
            return Ok(new BaseResponseModel<Guid>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, id));
        }

        [HttpGet("all")]
        [SwaggerOperation(Summary = "Danh sách lịch hẹn (phân trang)")]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize)
        {
            var result = await _service.GetAllAsync(pageNumber, pageSize);
            return Ok(new BaseResponseModel<BasePaginatedList<GETAppointmentModelViews>>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, result));
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy lịch hẹn theo ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(new BaseResponseModel<GETAppointmentModelViews>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, result));
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Cập nhật lịch hẹn")]
        public async Task<IActionResult> Update([FromBody] PUTAppointmentModelViews model)
        {
            await _service.UpdateAsync(model);
            return Ok(new BaseResponseModel<string>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, "Update successful"));
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xoá lịch hẹn (soft delete)")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok(new BaseResponseModel<string>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, "Delete successful"));
        }
    }
}
