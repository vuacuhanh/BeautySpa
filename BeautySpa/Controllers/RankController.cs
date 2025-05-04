using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.RankModelViews;
using BeautySpa.Core.Base;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RankController : ControllerBase
    {
        private readonly IRankService _rankService;

        public RankController(IRankService rankService)
        {
            _rankService = rankService;
        }

        [HttpGet("getall/ranks")]
        [SwaggerOperation(Summary = "Lấy danh sách rank có phân trang")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _rankService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("get/{id}")]
        [SwaggerOperation(Summary = "Lấy rank theo ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _rankService.GetByIdAsync(id));
        }

        [HttpPost("create/rank")]
        [SwaggerOperation(Summary = "Tạo mới một rank")]
        public async Task<IActionResult> Create([FromBody] POSTRankModelView model)
        {
            return Ok(await _rankService.CreateAsync(model));
        }

        [HttpPut("update/rank")]
        [SwaggerOperation(Summary = "Cập nhật thông tin rank")]
        public async Task<IActionResult> Update([FromBody] PUTRankModelView model)
        {
            return Ok(await _rankService.UpdateAsync(model));
        }

        [HttpDelete("delete/rank/{id}")]
        [SwaggerOperation(Summary = "Xóa mềm rank")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _rankService.DeleteAsync(id));
        }
    }
}
