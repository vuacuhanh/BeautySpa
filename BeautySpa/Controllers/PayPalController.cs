using BeautySpa.Contract.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BeautySpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayPalController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PayPalController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string paymentId)
        {
            var result = await _paymentService.ConfirmPayPalAsync(paymentId);
            return Ok(result);
        }
    }
}
