using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.RequestBecomeProviderModelView;
using BeautySpa.Services.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [ApiController]
    [Route("api/request-provider")]
    public class RequestBecomeProviderController : ControllerBase
    {
        private readonly IRequestBecomeProvider _service;

        public RequestBecomeProviderController(IRequestBecomeProvider service)
        {
            _service = service;
        }

        [HttpPost("create")]
        [SwaggerOperation(Summary = "Tạo đơn đăng ký trở thành provider")]
        public async Task<IActionResult> Create([FromBody] POSTRequestBecomeProviderModelView model)
        {
            return Ok(await _service.CreateRequestAsync(model));
        }

        [HttpGet("get")]
        [SwaggerOperation(Summary = "lấy ra những đơn đăng ký theo trạng thái ( pending, approved, rejected")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAsync([FromQuery] string? status, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var result = await _service.GetAllAsync(status, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPost("approve/{id}")]
        [SwaggerOperation(Summary = "Duyệt đơn (Approved)")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(Guid id)
        {
            return Ok(await _service.ApproveRequestAsync(id));
        }

        [HttpPost("reject/{id}")]
        [SwaggerOperation(Summary = "Từ chối đơn (Rejected)")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] string reason)
        {
            return Ok(await _service.RejectRequestAsync(id, reason));
        }
    }
}