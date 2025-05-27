using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServicePromotionModelView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Quản lý Flash Sale cho Dịch vụ (Service Promotion)")]
    public class ServicePromotionController : ControllerBase
    {
        private readonly IServicePromotionService _servicePromotionService;

        public ServicePromotionController(IServicePromotionService servicePromotionService)
        {
            _servicePromotionService = servicePromotionService;
        }

        [HttpGet("get-all")]
        [SwaggerOperation(Summary = "Lấy danh sách flash sale (phân trang)")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var result = await _servicePromotionService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Lấy chi tiết flash sale theo ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _servicePromotionService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        //[Authorize(Roles = "Provider,Admin")]
        [SwaggerOperation(Summary = "Tạo mới một flash sale")]
        public async Task<IActionResult> Create([FromBody] POSTServicePromotionModelView model)
        {
            var result = await _servicePromotionService.CreateAsync(model);
            return Ok(result);
        }

        [HttpPut]
        //[Authorize(Roles = "Provider,Admin")]
        [SwaggerOperation(Summary = "Cập nhật thông tin flash sale")]
        public async Task<IActionResult> Update([FromBody] PUTServicePromotionModelView model)
        {
            var result = await _servicePromotionService.UpdateAsync(model);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        //[Authorize(Roles = "Provider,Admin")]
        [SwaggerOperation(Summary = "Xóa (mềm) một flash sale")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _servicePromotionService.DeleteAsync(id);
            return Ok(result);
        }

        [HttpGet("my-promotions")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> GetMyPromotions()
        {
            return Ok(await _servicePromotionService.GetAllByCurrentUserAsync());
        }

    }
}
