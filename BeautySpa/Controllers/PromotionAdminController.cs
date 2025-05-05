using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.PromotionAdminModelView;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Quản lý khuyến mãi hệ thống (PromotionAdmin)")]
    public class PromotionAdminController : ControllerBase
    {
        private readonly IPromotionAdminService _promotionAdminService;

        public PromotionAdminController(IPromotionAdminService promotionAdminService)
        {
            _promotionAdminService = promotionAdminService;
        }

        [HttpGet("getall")]
        [SwaggerOperation(Summary = "Lấy danh sách khuyến mãi hệ thống")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _promotionAdminService.GetAllAsync());
        }

        [HttpGet("get-by-id/{id}")]
        [SwaggerOperation(Summary = "Lấy thông tin khuyến mãi hệ thống theo ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _promotionAdminService.GetByIdAsync(id));
        }

        [HttpPost("create")]
        [SwaggerOperation(Summary = "Tạo khuyến mãi hệ thống mới")]
        public async Task<IActionResult> Create([FromBody] POSTPromotionAdminModelView model)
        {
            return Ok(await _promotionAdminService.CreateAsync(model));
        }

        [HttpPut("update")]
        [SwaggerOperation(Summary = "Cập nhật khuyến mãi hệ thống")]
        public async Task<IActionResult> Update([FromBody] PUTPromotionAdminModelView model)
        {
            return Ok(await _promotionAdminService.UpdateAsync(model));
        }
    }
}
