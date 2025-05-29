using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.ServiceCategoryModelViews;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("get-all")]
        [SwaggerOperation(Summary = "Lấy danh sách danh mục dịch vụ (phân trang)")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            return Ok(await _categoryService.GetAllAsync(pageNumber, pageSize));
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Lấy chi tiết danh mục dịch vụ theo ID")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            return Ok(await _categoryService.GetByIdAsync(id));
        }

        [HttpPost("create")]
        [SwaggerOperation(Summary = "Tạo mới danh mục dịch vụ")]
        public async Task<IActionResult> Create([FromBody] POSTServiceCategoryModelViews model)
        {
            return Ok(await _categoryService.CreateAsync(model));
        }

        [HttpPut("update")]
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

        //[HttpDelete("hard-delete/{id:guid}")]
        ////[Authorize(Roles = "Admin")]
        //[SwaggerOperation(Summary = "Xóa cứng danh mục dịch vụ và tất cả liên quan")]
        //public async Task<IActionResult> DeleteHard([FromRoute] Guid id)
        //{
        //    return Ok(await _categoryService.DeleteHardAsync(id));
        //}
    }
}