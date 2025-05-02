using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.FavoriteModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Chức năng Like/Unlike nhà cung cấp")]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpPost("like")]
        [SwaggerOperation(Summary = "Like hoặc Unlike nhà cung cấp")]
        public async Task<IActionResult> LikeOrUnlike([FromBody] ToggleFavoriteRequest model)
        {
            return Ok(await _favoriteService.LikeOrUnlikeAsync(model.CustomerId, model.ProviderId));
        }

        [HttpGet("check")]
        [SwaggerOperation(Summary = "Kiểm tra người dùng đã like nhà cung cấp chưa")]
        public async Task<IActionResult> IsLiked([FromQuery] Guid customerId, [FromQuery] Guid providerId)
        {
            return Ok(await _favoriteService.IsFavoriteAsync(customerId, providerId));
        }

        [HttpGet("list/{providerId}")]
        [SwaggerOperation(Summary = "Lấy danh sách các lượt like")]
        //FE dùng .length để đếm nha nhắc trước rồi đó :))
        public async Task<IActionResult> GetFavorites(Guid providerId)
        {
            return Ok(await _favoriteService.GetFavoritesByProviderAsync(providerId));
        }
    }
}
