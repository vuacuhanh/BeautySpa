// ========================
// 6. CONTROLLER (ServiceImageController.cs)
// ========================
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

        [HttpGet]
        [SwaggerOperation(Summary = "Lấy danh sách ảnh nhà cung cấp (phân trang)")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            return Ok(await _imageService.GetAllAsync(pageNumber, pageSize));
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Lấy ảnh nhà cung cấp theo ID")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            return Ok(await _imageService.GetByIdAsync(id));
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Tạo mới ảnh cho nhà cung cấp")]
        public async Task<IActionResult> Create([FromBody] POSTServiceImageModelViews model)
        {
            return Ok(await _imageService.CreateAsync(model));
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Cập nhật ảnh nhà cung cấp")]
        public async Task<IActionResult> Update([FromBody] PUTServiceImageModelViews model)
        {
            return Ok(await _imageService.UpdateAsync(model));
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation(Summary = "Xóa mềm ảnh nhà cung cấp")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            return Ok(await _imageService.DeleteAsync(id));
        }

        [HttpPut("set-primary/{imageId:guid}")]
        [SwaggerOperation(Summary = "Chọn ảnh làm ảnh chính cho nhà cung cấp")]
        public async Task<IActionResult> SetPrimaryImage([FromRoute] Guid imageId)
        {
            return Ok(await _imageService.SetPrimaryImageAsync(imageId));
        }
    }
}