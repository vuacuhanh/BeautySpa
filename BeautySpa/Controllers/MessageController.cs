using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.MessageModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Tin nhắn trao đổi giữa khách hàng và nhà cung cấp")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _service;

        public MessageController(IMessageService service)
        {
            _service = service;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Gửi tin nhắn")]
        public async Task<IActionResult> Create([FromBody] POSTMessageModelViews model)
        {
            var id = await _service.CreateAsync(model);
            return Ok(new BaseResponseModel<Guid>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, id));
        }

        [HttpGet("all")]
        [SwaggerOperation(Summary = "Danh sách tin nhắn (phân trang)")]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize)
        {
            var result = await _service.GetAllAsync(pageNumber, pageSize);
            return Ok(new BaseResponseModel<BasePaginatedList<GETMessageModelViews>>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, result));
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Xem tin nhắn theo ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(new BaseResponseModel<GETMessageModelViews>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, result));
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Cập nhật nội dung/trạng thái tin nhắn")]
        public async Task<IActionResult> Update([FromBody] PUTMessageModelViews model)
        {
            await _service.UpdateAsync(model);
            return Ok(new BaseResponseModel<string>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, "Update successful"));
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xoá tin nhắn (soft delete)")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok(new BaseResponseModel<string>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, "Delete successful"));
        }
    }
}
