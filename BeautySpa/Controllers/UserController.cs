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
        [HttpGet]
        [SwaggerOperation(Summary = "Lấy danh sách tất cả người dùng (phân trang)")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _userService.GetAllAsync(pageNumber, pageSize));
        }

        // GET: api/user/customer
        [HttpGet("customer")]
        [SwaggerOperation(Summary = "Lấy danh sách người dùng có vai trò Customer (phân trang)")]
        [Authorize(Roles = "Admin, Customer")]
        public async Task<IActionResult> GetCustomer([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _userService.GetCustomerAsync(pageNumber, pageSize));
        }

        // PUT: api/user
        [HttpPut]
        [SwaggerOperation(Summary = "Cập nhật thông tin người dùng")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] PUTUserModelViews model)
        {
            return Ok(await _userService.UpdateAsync(model));
        }

        // PUT: api/user/customer
        [HttpPut("customer")]
        [SwaggerOperation(Summary = "Cập nhật thông tin khách hàng")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateCustomer([FromBody] PUTuserforcustomer model)
        {
            return Ok(await _userService.UpdateCustomerAsync(model));
        }

        // DELETE: api/user/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa mềm người dùng theo ID")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _userService.DeleteAsync(id));
        }
    }
}
