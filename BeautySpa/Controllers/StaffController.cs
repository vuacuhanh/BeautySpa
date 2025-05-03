using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.StaffModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Nhân viên bên nhà đăng ký dịch vụ")]
    public class StaffController : ControllerBase
    {
        private readonly IStaff _staffService;

        public StaffController(IStaff staffService)
        {
            _staffService = staffService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] POSTStaffModelView model)
        {
            var result = await _staffService.CreateAsync(model);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PUTStaffModelView model)
        {
            var result = await _staffService.UpdateAsync(model);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _staffService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _staffService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int size, [FromQuery] Guid? providerId = null)
        {
            var result = await _staffService.GetAllAsync(page, size, providerId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
