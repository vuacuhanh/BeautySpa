using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.MemberShipModelViews;
using BeautySpa.Core.Base;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberShipController : ControllerBase
    {
        private readonly IMemberShipService _memberShipService;

        public MemberShipController(IMemberShipService memberShipService)
        {
            _memberShipService = memberShipService;
        }

        [HttpGet("getall/membership")]
        [SwaggerOperation(Summary = "Lấy danh sách thành viên (phân trang)")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _memberShipService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("get/membership/by/{id}")]
        [SwaggerOperation(Summary = "Lấy thành viên theo ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _memberShipService.GetByUserIdAsync(id);
            return Ok(result);
        }

        [HttpPatch("add-points")]
        [SwaggerOperation(Summary = "Cộng điểm và cập nhật rank cho thành viên")]
        public async Task<IActionResult> AddPoints([FromBody] PATCHMemberShipAddPointsModel model)
        {
            var result = await _memberShipService.AddPointsAsync(model);
            return Ok(result);
        }

        [HttpDelete("delete/membership/{id}")]
        [SwaggerOperation(Summary = "Xóa mềm thành viên")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _memberShipService.DeleteAsync(id);
            return Ok(result);
        }
    }
}
