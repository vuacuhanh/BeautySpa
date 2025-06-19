using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ReviewModelViews;
using BeautySpa.Services.Service;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Đánh giá khách hàng")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _service;

        public ReviewController(IReviewService service)
        {
            _service = service;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Tạo đánh giá")]
        public async Task<IActionResult> Create([FromBody] POSTReviewModelViews model)
        {
            var result = await _service.CreateAsync(model);
            return Ok(result);
        }

        [HttpGet("all")]
        [SwaggerOperation(Summary = "Danh sách đánh giá (phân trang)")]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize)
        {
            var result = await _service.GetAllAsync(pageNumber, pageSize);
            return Ok(new BaseResponseModel<BasePaginatedList<GETReviewModelViews>>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, result));
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy đánh giá theo ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(new BaseResponseModel<GETReviewModelViews>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, result));
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Cập nhật đánh giá")]
        public async Task<IActionResult> Update([FromBody] PUTReviewModelViews model)
        {
            await _service.UpdateAsync(model);
            return Ok(new BaseResponseModel<string>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, "Update successful"));
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xoá đánh giá (soft delete)")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok(new BaseResponseModel<string>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, "Delete successful"));
        }

        [HttpGet("by-provider/{providerId}")]
        [SwaggerOperation(Summary = "Lấy danh sách đánh giá theo ProviderId")]
        public async Task<IActionResult> GetByProviderId(Guid providerId)
        {
            var result = await _service.GetByProviderIdAsync(providerId);
            return Ok(new BaseResponseModel<List<GETReviewModelViews>>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, result));
        }

    }
}
