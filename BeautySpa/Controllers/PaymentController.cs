using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.PaymentModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Thanh toán dịch vụ")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Tạo giao dịch thanh toán")]
        public async Task<IActionResult> Create([FromBody] POSTPaymentModelViews model)
        {
            var id = await _service.CreateAsync(model);
            return Ok(new BaseResponseModel<Guid>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, id));
        }

        [HttpGet("all")]
        [SwaggerOperation(Summary = "Danh sách thanh toán (phân trang)")]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize)
        {
            var result = await _service.GetAllAsync(pageNumber, pageSize);
            return Ok(new BaseResponseModel<BasePaginatedList<GETPaymentModelViews>>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, result));
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy thông tin thanh toán theo ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(new BaseResponseModel<GETPaymentModelViews>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, result));
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Cập nhật trạng thái/thông tin thanh toán")]
        public async Task<IActionResult> Update([FromBody] PUTPaymentModelViews model)
        {
            await _service.UpdateAsync(model);
            return Ok(new BaseResponseModel<string>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, "Update successful"));
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xoá thanh toán (soft delete)")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok(new BaseResponseModel<string>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, "Delete successful"));
        }
    }
}
