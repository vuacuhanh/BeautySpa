using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.LocationModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Quản lý chi nhánh và địa chỉ spa")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        // ===== Branch APIs =====

        [HttpGet("spa")]
        [SwaggerOperation(Summary = "Lấy tất cả spa")]
        public async Task<IActionResult> GetAllSpa()
        {
            return Ok(await _locationService.GetAllBranchesAsync());
        }

        [HttpGet("get/spa/{id}")]
        [SwaggerOperation(Summary = "Lấy chi tiết spa theo ID")]
        public async Task<IActionResult> GetSpaById(Guid id)
        {
            return Ok(await _locationService.GetBranchByIdAsync(id));
        }

        [HttpPost("create/spa")]
        [SwaggerOperation(Summary = "Tạo mới chi spa")]
        public async Task<IActionResult> CreateSpa([FromBody] POSTBranchLocationModelView model)
        {
            return Ok(await _locationService.CreateBranchAsync(model));
        }

        [HttpPut("upadate/spa")]
        [SwaggerOperation(Summary = "Cập nhật spa")]
        public async Task<IActionResult> UpdateSpa([FromBody] PUTBranchLocationModelView model)
        {
            return Ok(await _locationService.UpdateBranchAsync(model));
        }

        [HttpDelete("delete/spa/{id}")]
        [SwaggerOperation(Summary = "Xóa mềm spa")]
        public async Task<IActionResult> DeleteSpa(Guid id)
        {
            return Ok(await _locationService.DeleteBranchAsync(id));
        }

        // ===== Location APIs =====

        [HttpGet("getall/locations")]
        [SwaggerOperation(Summary = "Lấy tất cả địa chỉ chi nhánh spa")]
        public async Task<IActionResult> GetAllLocationBranches()
        {
            return Ok(await _locationService.GetAllLocationsAsync());
        }

        [HttpGet("get/locations/branches/{id}")]
        [SwaggerOperation(Summary = "Lấy địa chỉ chi nhánh spa theo ID")]
        public async Task<IActionResult> GetLocationBranchesById(Guid id)
        {
            return Ok(await _locationService.GetLocationByIdAsync(id));
        }

        [HttpPost("create/locations/branches")]
        [SwaggerOperation(Summary = "Tạo mới địa chỉ địa chỉ spa")]
        public async Task<IActionResult> CreateLocationBranches([FromBody] POSTLocationSpaModelView model)
        {
            return Ok(await _locationService.CreateLocationAsync(model));
        }

        [HttpPut("update/locations/branches")]
        [SwaggerOperation(Summary = "Cập nhật địa chỉ chi nhánh spa")]
        public async Task<IActionResult> UpdateLocationBranches([FromBody] PUTLocationSpaModelView model)
        {
            return Ok(await _locationService.UpdateLocationAsync(model));
        }

        [HttpDelete("delete/locations/branches{id}")]
        [SwaggerOperation(Summary = "Xóa mềm địa chỉ chi nhánh spa")]
        public async Task<IActionResult> DeleteLocationBranches(Guid id)
        {
            return Ok(await _locationService.DeleteLocationAsync(id));
        }

        [HttpGet("get/locations/branches/by-spa/{branchId}")]
        [SwaggerOperation(Summary = "Lấy danh sách địa chỉ theo spa")]
        public async Task<IActionResult> GetLocationsBySpaId(Guid branchId)
        {
            return Ok(await _locationService.GetLocationsByBranchIdAsync(branchId));
        }
    }
}
