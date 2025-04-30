using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.ServiceCategoryModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Quản lý danh mục dịch vụ")]
    public class ServiceCategoryController : ControllerBase
    {
        private readonly IServiceCategory _categoryService;

        public ServiceCategoryController(IServiceCategory categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lấy danh sách danh mục dịch vụ (phân trang)")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _categoryService.GetAllAsync(pageNumber, pageSize));
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Lấy chi tiết danh mục dịch vụ theo ID")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            return Ok(await _categoryService.GetByIdAsync(id));
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Tạo mới danh mục dịch vụ")]
        public async Task<IActionResult> Create([FromBody] POSTServiceCategoryModelViews model)
        {
            return Ok(await _categoryService.CreateAsync(model));
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Cập nhật danh mục dịch vụ")]
        public async Task<IActionResult> Update([FromBody] PUTServiceCategoryModelViews model)
        {
            return Ok(await _categoryService.UpdateAsync(model));
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation(Summary = "Xóa mềm danh mục dịch vụ")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            return Ok(await _categoryService.DeleteAsync(id));
        }
    }
}
