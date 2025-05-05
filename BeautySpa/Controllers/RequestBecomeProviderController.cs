using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.RequestBecomeProviderModelView;
using BeautySpa.Services.Service;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Create([FromBody] POSTRequestBecomeProviderModelView model)
        {
            return Ok(await _service.CreateRequestAsync(model));
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetAllAsync([FromQuery] string? status, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var result = await _service.GetAllAsync(status, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPost("approve/{id}")]
        public async Task<IActionResult> Approve(Guid id)
        {
            return Ok(await _service.ApproveRequestAsync(id));
        }

        [HttpPost("reject/{id}")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] string reason)
        {
            return Ok(await _service.RejectRequestAsync(id, reason));
        }
    }
}