using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServiceModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Quản lý dịch vụ (Service)")]
    public class ServiceController : ControllerBase
    {
        private readonly IServices _serviceService;

        public ServiceController(IServices serviceService)
        {
            _serviceService = serviceService;
        }

        [HttpGet("get-all")]
        [SwaggerOperation(Summary = "Lấy danh sách dịch vụ (phân trang)")]
        public async Task<IActionResult> GetAllAsync(int pageNumber, int pageSize, Guid? providerId = null)
        {
            var result = await _serviceService.GetAllAsync(pageNumber, pageSize, providerId);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Lấy chi tiết dịch vụ theo ID")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            return Ok(await _serviceService.GetByIdAsync(id));
        }

        [HttpGet("by-provider/{providerId:guid}")]
        [SwaggerOperation(Summary = "Lấy tất cả dịch vụ theo ProviderId")]
        public async Task<IActionResult> GetByProviderId([FromRoute] Guid providerId)
        {
            var result = await _serviceService.GetByProviderIdAsync(providerId);
            return Ok(result);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Provider")]
        [SwaggerOperation(Summary = "Tạo mới dịch vụ")]
        public async Task<IActionResult> Create([FromBody] POSTServiceModelViews model)
        {
            return Ok(await _serviceService.CreateAsync(model));
        }

        [HttpPut("update")]
        [Authorize(Roles = "Provider")]
        [SwaggerOperation(Summary = "Cập nhật thông tin dịch vụ")]
        public async Task<IActionResult> Update([FromBody] PUTServiceModelViews model)
        {
            return Ok(await _serviceService.UpdateAsync(model));
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Provider")]
        [SwaggerOperation(Summary = "Xóa mềm dịch vụ theo ID")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            return Ok(await _serviceService.DeleteAsync(id));
        }
        [HttpGet("my-services")]
        [Authorize(Roles = "Provider")]
        [SwaggerOperation(Summary = "Lấy tất cả dịch vụ do tài khoản hiện tại tạo")]
        public async Task<IActionResult> GetMyServices()
        {
            var result = await _serviceService.GetMyServicesAsync();
            return Ok(result);
        }

    }
}