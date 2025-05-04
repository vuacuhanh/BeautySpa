using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServiceImageModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Quản lý hình ảnh cơ sở nhà cung cấp")]
    public class ServiceImageController : ControllerBase
    {
        private readonly IServiceImages _imageService;

        public ServiceImageController(IServiceImages imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("multi-create")]
        [SwaggerOperation(Summary = "Tạo nhiều ảnh mô tả cho nhà cung cấp")]
        public async Task<IActionResult> CreateMultiple([FromBody] POSTServiceImageModelViews model)
        {
            var result = await _imageService.CreateMultipleAsync(model);
            return Ok(result);
        }

        [HttpGet("by-provider/{providerId}/paged")]
        [SwaggerOperation(Summary = "Lấy danh sách ảnh của nhà cung cấp (phân trang)")]
        public async Task<IActionResult> GetPaged(Guid providerId, [FromQuery] int page, [FromQuery] int size)
        {
            var result = await _imageService.GetPagedByProviderIdAsync(providerId, page, size);
            return Ok(result);
        }

        [HttpGet("by-provider/{providerId}")]
        [SwaggerOperation(Summary = "Lấy tất cả ảnh của nhà cung cấp")]
        public async Task<IActionResult> GetByProvider(Guid providerId)
        {
            var result = await _imageService.GetByProviderIdAsync(providerId);
            return Ok(result);
        }

        [HttpDelete("{imageId}")]
        [SwaggerOperation(Summary = "Xóa mềm ảnh theo ID")]
        public async Task<IActionResult> Delete(Guid imageId)
        {
<<<<<<< HEAD
            return Ok(await _imageService.UpdateAsync(model));
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation(Summary = "Xóa mềm ảnh nhà cung cấp")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            return Ok(await _imageService.DeleteAsync(id));
        }

        /*[HttpPut("set-primary/{imageId:guid}")]
        [SwaggerOperation(Summary = "Chọn ảnh làm ảnh chính cho nhà cung cấp")]
        public async Task<IActionResult> SetPrimaryImage([FromRoute] Guid imageId)
        {
            return Ok(await _imageService.SetPrimaryImageAsync(imageId));
        }*/
=======
            var result = await _imageService.DeleteAsync(imageId);
            return Ok(result);
        }
>>>>>>> f10d2edc9c521bf9ffcf36161b33af05f888b4d8
    }
}