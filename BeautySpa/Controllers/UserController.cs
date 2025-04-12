using BeautySpa.ModelViews.UserModelViews;
using BeautySpa.Services.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUsers _userService;

        // Constructor để tiêm IUsers service
        public UserController(IUsers userService)
        {
            _userService = userService;
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        //[Authorize(Roles = "Admin, Customer")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/user
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var users = await _userService.GetAllAsync(pageNumber, pageSize);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/user/customer/{id}
        [HttpGet("customer/{id}")]
        [Authorize(Roles = "Admin, Customer")]
        public async Task<IActionResult> GetCustomerInfo(Guid id)
        {
            try
            {
                var customer = await _userService.GetCustomerInfoAsync(id);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/user
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] PUTUserModelViews model)
        {
            try
            {
                await _userService.UpdateAsync(model);
                return Ok("User updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/user/customer
        [HttpPut("customer")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateCustomer([FromBody] PUTuserforcustomer model)
        {
            try
            {
                await _userService.UpdateCustomerAsync(model);
                return Ok("Customer updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/user/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _userService.DeleteAsync(id);
                return Ok("User deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}