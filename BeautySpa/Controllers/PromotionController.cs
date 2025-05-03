using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.PromotionModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Khuyến mãi nhà cung cấp")]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _service;

        public PromotionController(IPromotionService service)
        {
            _service = service;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Tạo khuyến mãi")]
        public async Task<IActionResult> Create([FromBody] POSTPromotionModelViews model)
        {
            var result = await _service.CreateAsync(model);
            return Ok(result);
        }

        [HttpGet("all")]
        [SwaggerOperation(Summary = "Danh sách khuyến mãi (phân trang)")]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize)
        {
            var result = await _service.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy khuyến mãi theo ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Cập nhật khuyến mãi")]
        public async Task<IActionResult> Update([FromBody] PUTPromotionModelViews model)
        {
            var result = await _service.UpdateAsync(model);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xoá khuyến mãi (soft delete)")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return Ok(result);
        }
    }
}