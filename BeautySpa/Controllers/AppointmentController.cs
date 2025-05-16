using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.AppointmentModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Manage appointment booking")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _service;

        public AppointmentController(IAppointmentService service)
        {
            _service = service;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create new appointment")]
        public async Task<IActionResult> Create([FromBody] POSTAppointmentModelView model)
        {
            return Ok(await _service.CreateAsync(model));
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Update appointment")]
        public async Task<IActionResult> Update([FromBody] PUTAppointmentModelView model)
        {
            return Ok(await _service.UpdateAsync(model));
        }

        [HttpPatch("status/{id}")]
        [SwaggerOperation(Summary = "Update appointment status (confirmed, checked_in, completed, canceled, no_show)")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] string status)
        {
            return Ok(await _service.UpdateStatusAsync(id, status));
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Soft delete appointment")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _service.DeleteAsync(id));
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all appointments (paginated)")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            return Ok(await _service.GetAllAsync(pageNumber, pageSize));
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get appointment details by ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }
    }
}
