using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.PaymentModelViews;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Quản lý thanh toán người dùng (MoMo, VNPAY)")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        [HttpPost("create-deposit")]
        [SwaggerOperation(Summary = "Tạo giao dịch thanh toán cọc (qua MoMo hoặc VNPAY)")]
        public async Task<IActionResult> CreateDeposit([FromBody] POSTPaymentModelView model)
        {
            return Ok(await _service.CreateDepositAsync(model));
        }

        [HttpPost("refund-deposit")]
        [SwaggerOperation(Summary = "Hoàn tiền giao dịch cọc (có thể giữ lại phí nền tảng)")]
        public async Task<IActionResult> RefundDeposit([FromBody] RefundPaymentModelView model)
        {
            return Ok(await _service.RefundDepositAsync(model));
        }

        [HttpGet("by-appointment/{appointmentId}")]
        [SwaggerOperation(Summary = "Lấy thông tin thanh toán theo mã lịch hẹn")]
        public async Task<IActionResult> GetByAppointmentId(Guid appointmentId)
        {
            return Ok(await _service.GetByAppointmentIdAsync(appointmentId));
        }

        /*[HttpGet("vnpay-ipn")]
        [SwaggerOperation(Summary = "Endpoint IPN nhận callback từ VNPAY khi thanh toán hoàn tất")]
        public async Task<IActionResult> HandleVnpayIpn([FromQuery] Dictionary<string, string> query)
        {
            return Ok(await _service.HandleVnpayIpnAsync(query));
        }

        [HttpPost("momo-ipn")]
        [SwaggerOperation(Summary = "Endpoint IPN nhận callback từ MoMo khi thanh toán hoàn tất")]
        public async Task<IActionResult> HandleMomoIpn([FromBody] JObject payload)
        {
            return Ok(await _service.HandleMomoIpnAsync(payload));
        }

        [HttpPost("query-vnpay")]
        [SwaggerOperation(Summary = "Truy vấn trạng thái giao dịch qua VNPAY (manual query)")]
        public async Task<IActionResult> QueryVnPayStatus([FromBody] QueryVnPayModel model)
        {
            return Ok(await _service.QueryVnPayStatusAsync(model));
        }

        [HttpPost("query-momo")]
        [SwaggerOperation(Summary = "Truy vấn trạng thái giao dịch qua MoMo (manual query)")]
        public async Task<IActionResult> QueryMoMoStatus([FromBody] QueryMoMoModel model)
        {
            return Ok(await _service.QueryMoMoStatusAsync(model));
        }*/
    }
}
