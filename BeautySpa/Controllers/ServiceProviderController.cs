using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.ServiceProviderModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Quản lý nhà cung cấp dịch vụ")]
    public class ServiceProviderController : ControllerBase
    {
        private readonly IServiceProviders _providerService;

        public ServiceProviderController(IServiceProviders providerService)
        {
            _providerService = providerService;
        }

        [HttpGet("get-all")]
        [SwaggerOperation(Summary = "Lấy danh sách nhà cung cấp (phân trang)")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            return Ok(await _providerService.GetAllAsync(pageNumber, pageSize));
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Lấy thông tin nhà cung cấp theo ID")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            return Ok(await _providerService.GetByIdAsync(id));
        }

        [HttpPost("create")]
        [SwaggerOperation(Summary = "Tạo mới nhà cung cấp dịch vụ")]
        public async Task<IActionResult> Create([FromBody] POSTServiceProviderModelViews model)
        {
            return Ok(await _providerService.CreateAsync(model));
        }

        [HttpPut("update")]
        [SwaggerOperation(Summary = "Cập nhật thông tin nhà cung cấp dịch vụ")]
        public async Task<IActionResult> Update([FromBody] PUTServiceProviderModelViews model)
        {
            return Ok(await _providerService.UpdateAsync(model));
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation(Summary = "Xóa mềm nhà cung cấp dịch vụ theo ID")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            return Ok(await _providerService.DeleteAsync(id));
        }
        [HttpGet("by-category/{categoryId}")]
        [SwaggerOperation(Summary = "Lấy danh sách nhà cung cấp theo danh mục")]
        public async Task<IActionResult> GetByCategory([FromRoute] Guid categoryId)
        {
            var result = await _providerService.GetByCategory(categoryId);
            return Ok(result);
        }
    }
}