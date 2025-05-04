using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.NotificationModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Quản lý thông báo người dùng")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("getall/noti/by-user/{userId}")]
        [SwaggerOperation(Summary = "Lấy danh sách thông báo của người dùng")]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            return Ok(await _notificationService.GetAllByUserIdAsync(userId));
        }

        [HttpGet("unread-count/{userId}")]
        [SwaggerOperation(Summary = "Đếm số thông báo chưa đọc của người dùng")]
        public async Task<IActionResult> GetUnreadCount(Guid userId)
        {
            return Ok(await _notificationService.GetUnreadCountAsync(userId));
        }

        [HttpPost("create/noti")]
        [SwaggerOperation(Summary = "Tạo thông báo mới cho người dùng")]
        public async Task<IActionResult> Create([FromBody] POSTNotificationModelView model)
        {
            return Ok(await _notificationService.CreateAsync(model));
        }

        [HttpPatch("read/noti/{id}")]
        [SwaggerOperation(Summary = "Đánh dấu thông báo đã đọc")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            return Ok(await _notificationService.MarkAsReadAsync(id));
        }

        [HttpDelete("delete/noti/{id}")]
        [SwaggerOperation(Summary = "Xóa thông báo (soft delete)")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _notificationService.DeleteAsync(id));
        }

    }
}
