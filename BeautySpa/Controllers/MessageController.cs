using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.MessageModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Quản lý tin nhắn giữa người dùng")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("conversations/{userId}")]
        [SwaggerOperation(Summary = "Lấy danh sách các đoạn hội thoại đã nhắn với user này")]
        public async Task<IActionResult> GetConversations(Guid userId)
        {
            return Ok(await _messageService.GetConversationsAsync(userId));
        }

        [HttpGet("thread")]
        [SwaggerOperation(Summary = "Lấy đoạn chat 2 chiều giữa 2 người")]
        public async Task<IActionResult> GetThread([FromQuery] Guid user1Id, [FromQuery] Guid user2Id)
        {
            return Ok(await _messageService.GetThreadAsync(user1Id, user2Id));
        }

        [HttpPost("send")]
        [SwaggerOperation(Summary = "Gửi tin nhắn mới từ người dùng hiện tại tới người nhận")]
        public async Task<IActionResult> Send([FromBody] CreateMessageRequest model)
        {
            return Ok(await _messageService.SendMessageAsync(model));
        }

        [HttpPatch("read/{messageId}")]
        [SwaggerOperation(Summary = "Đánh dấu tin nhắn đã đọc")]
        public async Task<IActionResult> MarkAsRead(Guid messageId)
        {
            return Ok(await _messageService.MarkAsReadAsync(messageId));
        }

        [HttpDelete("{messageId}")]
        [SwaggerOperation(Summary = "Xóa tin nhắn (soft delete)")]
        public async Task<IActionResult> Delete(Guid messageId)
        {
            return Ok(await _messageService.DeleteMessageAsync(messageId));
        }

    }
}
