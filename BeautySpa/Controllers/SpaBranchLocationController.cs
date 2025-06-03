using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.LocationModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Quản lý chi nhánh spa (SpaBranchLocation)")]
    public class SpaBranchLocationController : ControllerBase
    {
        private readonly ISpaBranchLocationService _branchService;

        public SpaBranchLocationController(ISpaBranchLocationService branchService)
        {
            _branchService = branchService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Provider")]
        [SwaggerOperation(Summary = "Tạo chi nhánh spa mới")]
        public async Task<IActionResult> Create([FromBody] POSTSpaBranchLocationModelView model)
        {
            return Ok(await _branchService.CreateAsync(model));
        }

        [HttpPut("update")]
        [Authorize(Roles = "Provider")]
        [SwaggerOperation(Summary = "Cập nhật thông tin chi nhánh spa")]
        public async Task<IActionResult> Update([FromBody] PUTSpaBranchLocationModelView model)
        {
            return Ok(await _branchService.UpdateAsync(model));
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Provider")]
        [SwaggerOperation(Summary = "Xoá (soft delete) chi nhánh spa")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _branchService.DeleteAsync(id));
        }

        [HttpGet("get/{id}")]
        [SwaggerOperation(Summary = "Lấy chi tiết chi nhánh spa theo ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _branchService.GetByIdAsync(id));
        }

        [HttpGet("get-all")]
        [SwaggerOperation(Summary = "Lấy danh sách chi nhánh spa (phân trang)")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _branchService.GetAllAsync(pageNumber, pageSize));
        }

        [HttpGet("by-provider/{providerId}")]
        [SwaggerOperation(Summary = "Lấy danh sách chi nhánh spa theo ProviderId")]
        public async Task<IActionResult> GetByProvider(Guid providerId)
        {
            return Ok(await _branchService.GetByProviderAsync(providerId));
        }
    }
}
