using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.LocationModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpaBranchLocationController : ControllerBase
    {
        private readonly ISpaBranchLocationService _service;
        public SpaBranchLocationController(ISpaBranchLocationService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        [SwaggerOperation(Summary = "Tạo chi nhánh spa mới")]
        public async Task<IActionResult> Create([FromBody] POSTSpaBranchLocationModelView model)
            => Ok(await _service.CreateAsync(model));

        [HttpPut("update")]
        [SwaggerOperation(Summary = "Cập nhật chi nhánh spa")]
        public async Task<IActionResult> Update([FromBody] PUTSpaBranchLocationModelView model)
            => Ok(await _service.UpdateAsync(model));

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa mềm chi nhánh spa")]
        public async Task<IActionResult> Delete(Guid id)
            => Ok(await _service.DeleteAsync(id));

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy chi tiết chi nhánh spa theo ID")]
        public async Task<IActionResult> GetById(Guid id)
            => Ok(await _service.GetByIdAsync(id));

        [HttpGet("getby-provider")]
        [SwaggerOperation(Summary = "Lấy danh sách các chi nhánh spa theo ID Provider")]
        public async Task<IActionResult> GetAllByProviderQuery([FromQuery] Guid providerId)
            => Ok(await _service.GetByProviderAsync(providerId));

        [HttpGet("by-provider/{providerId}")]
        [SwaggerOperation(Summary = "Lấy danh sách các chi nhánh spa theo ID Provider (route)")]
        public async Task<IActionResult> GetAllByProvider(Guid providerId)
            => Ok(await _service.GetByProviderAsync(providerId));
    }
}