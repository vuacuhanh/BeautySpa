using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.PaymentModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Manage user payments (MoMo, VNPAY)")]
    public class PaymentController : ControllerBase
    {   
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        [HttpPost("create-deposit")]
        [SwaggerOperation(Summary = "Create deposit payment (MoMo/VNPAY)")]
        public async Task<IActionResult> CreateDeposit([FromBody] POSTPaymentModelView model)
        {
            return Ok(await _service.CreateDepositAsync(model));
        }

        [HttpPost("refund-deposit")]
        [SwaggerOperation(Summary = "Refund deposit with optional platform fee")]
        public async Task<IActionResult> RefundDeposit([FromBody] RefundPaymentModelView model)
        {
            return Ok(await _service.RefundDepositAsync(model));
        }

        [HttpGet("by-appointment/{appointmentId}")]
        [SwaggerOperation(Summary = "Get payment by appointment ID")]
        public async Task<IActionResult> GetByAppointmentId(Guid appointmentId)
        {
            return Ok(await _service.GetByAppointmentIdAsync(appointmentId));
        }
    }
}
