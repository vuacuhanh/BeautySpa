using BeautySpa.Contract.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BeautySpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayPalController : ControllerBase
    {
        private readonly IPayPalService _payPalService;

        public PayPalController(IPayPalService payPalService)
        {
            _payPalService = payPalService;
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string paymentId)
        {
            var result = await _payPalService.ConfirmPayPalAsync(paymentId);
            return Ok(result);
        }
    }
}
