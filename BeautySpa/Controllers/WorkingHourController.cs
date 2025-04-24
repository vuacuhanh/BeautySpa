using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.WorkingHourModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Giờ làm việc của Provider")]
    public class WorkingHourController : ControllerBase
    {
        private readonly IWorkingHourService _service;

        public WorkingHourController(IWorkingHourService service)
        {
            _service = service;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Tạo giờ làm việc")]
        public async Task<IActionResult> Create([FromBody] POSTWorkingHourModelViews model)
        {
            var id = await _service.CreateAsync(model);
            return Ok(new BaseResponseModel<Guid>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, id));
        }

        [HttpGet("all")]
        [SwaggerOperation(Summary = "Danh sách giờ làm việc (phân trang)")]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize)
        {
            var result = await _service.GetAllAsync(pageNumber, pageSize);
            return Ok(new BaseResponseModel<BasePaginatedList<GETWorkingHourModelViews>>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, result));
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy giờ làm việc theo ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(new BaseResponseModel<GETWorkingHourModelViews>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, result));
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Cập nhật giờ làm việc")]
        public async Task<IActionResult> Update([FromBody] PUTWorkingHourModelViews model)
        {
            await _service.UpdateAsync(model);
            return Ok(new BaseResponseModel<string>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, "Update successful"));
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xoá giờ làm việc (soft delete)")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok(new BaseResponseModel<string>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, "Delete successful"));
        }
    }
}
