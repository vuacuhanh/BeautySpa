using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.FavoriteModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Yêu thích nhà cung cấp")]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        // POST: api/favorite
        [HttpPost]
        //[Authorize]
        [SwaggerOperation(Summary = "Thêm yêu thích mới")]
        public async Task<IActionResult> Create([FromBody] POSTFavoriteModelViews model)
        {
            var favoriteId = await _favoriteService.CreateAsync(model);
            return Ok(new BaseResponseModel<Guid>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: favoriteId
            ));
        }

        // GET: api/favorite/all
        [HttpGet("all")]
        //[Authorize]
        [SwaggerOperation(Summary = "Lấy danh sách yêu thích (phân trang)")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var favorites = await _favoriteService.GetAllAsync(pageNumber, pageSize);
            return Ok(new BaseResponseModel<BasePaginatedList<GETFavoriteModelViews>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: favorites
            ));
        }

        // GET: api/favorite/{id}
        [HttpGet("{id}")]
        //[Authorize]
        [SwaggerOperation(Summary = "Lấy yêu thích theo ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var favorite = await _favoriteService.GetByIdAsync(id);
            return Ok(new BaseResponseModel<GETFavoriteModelViews>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: favorite
            ));
        }

        // PUT: api/favorite
        [HttpPut]
        //[Authorize]
        [SwaggerOperation(Summary = "Cập nhật yêu thích")]
        public async Task<IActionResult> Update([FromBody] PUTFavoriteModelViews model)
        {
            await _favoriteService.UpdateAsync(model);
            return Ok(new BaseResponseModel<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: "Cập nhật thành công"
            ));
        }

        // DELETE: api/favorite/{id}
        [HttpDelete("{id}")]
        //[Authorize]
        [SwaggerOperation(Summary = "Xoá yêu thích (soft delete)")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _favoriteService.DeleteAsync(id);
            return Ok(new BaseResponseModel<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: "Xoá thành công"
            ));
        }
    }
}
