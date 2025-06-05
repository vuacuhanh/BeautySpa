using BeautySpa.ModelViews.UserModelViews;
using BeautySpa.Contract.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Quản lý người dùng")]
    public class UserController : ControllerBase
    {
        private readonly IUsers _userService;

        public UserController(IUsers userService)
        {
            _userService = userService;
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy thông tin chi tiết người dùng theo ID")]
        //[Authorize(Roles = "Admin, Customer")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _userService.GetByIdAsync(id));
        }

        // GET: api/user
        [HttpGet ("get-all")]
        [SwaggerOperation(Summary = "Lấy danh sách tất cả người dùng (phân trang)")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            return Ok(await _userService.GetAllAsync(pageNumber, pageSize));
        }

        [HttpGet("role")]
        [SwaggerOperation(Summary = "Lấy danh sách tất cả người dùng theo role (phân trang)")]
        public async Task<IActionResult> GetByRole([FromQuery] string role, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var result = await _userService.GetByRoleAsync(role, pageNumber, pageSize);
            return Ok(result);
        }

        // GET: api/user/customer
        //[HttpGet("customer")]
        //[SwaggerOperation(Summary = "Lấy danh sách người dùng có vai trò Customer (phân trang)")]
        //[Authorize(Roles = "Admin, Customer")]
        //public async Task<IActionResult> GetCustomer([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        //{
        //    return Ok(await _userService.GetCustomerAsync(pageNumber, pageSize));
        //}

        // PUT: api/user
        [HttpPut("update")]
        [SwaggerOperation(Summary = "Cập nhật thông tin người dùng")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] PUTUserModelViews model)
        {
            return Ok(await _userService.UpdateAsync(model));
        }

        // PUT: api/user/customer
        //[HttpPut("customer")]
        //[SwaggerOperation(Summary = "Cập nhật thông tin khách hàng")]
        //[Authorize(Roles = "Customer")]
        //public async Task<IActionResult> UpdateCustomer([FromBody] PUTuserforcustomer model)
        //{
        //    return Ok(await _userService.UpdateCustomerAsync(model));
        //}

        // DELETE: api/user/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa mềm người dùng theo ID")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _userService.DeleteAsync(id));
        }

        [HttpDelete("Delete/{id}")]
        [SwaggerOperation(Summary = "Xóa mềm người dùng theo ID")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> SoftDeleteAsync(Guid id)
        {
            return Ok(await _userService.DeleteAsync(id));
        }

        [HttpDelete("Deletepermanent/{id}")]
        [SwaggerOperation(Summary = "Xóa vĩnh viễn người dùng theo ID")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletepermanentlyAsync(Guid id)
        {
            return Ok(await _userService.DeletepermanentlyAsync(id));
        }
        


        [HttpPut("deactivate-provider/{userId}")]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Hủy quyền và khóa tài khoản provider")]
        public async Task<IActionResult> DeactivateProvider(Guid userId)
        {
            var result = await _userService.DeactivateProviderAccountAsync(userId);
            return Ok(result);
        }

        [HttpPut("reactivate-provider/{userId}")]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Kích hoạt lại tài khoản provider bị khóa")]
        public async Task<IActionResult> ReactivateProvider(Guid userId)
        {
            var result = await _userService.ReactivateProviderAccountAsync(userId);
            return Ok(result);
        }
    }
}
