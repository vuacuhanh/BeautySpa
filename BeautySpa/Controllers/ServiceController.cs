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
    [SwaggerTag("Dịch vụ")]
    public class ServiceController : ControllerBase
    {
        private readonly IServices _serviceService;

        public ServiceController(IServices serviceService)
        {
            _serviceService = serviceService;
        }
        [HttpGet]
        [SwaggerOperation(Summary = "Get a paginated list of services")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _serviceService.GetAllAsync(pageNumber, pageSize));
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Get a service by ID")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            return Ok(await _serviceService.GetByIdAsync(id));
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new service")]
        public async Task<IActionResult> Create([FromBody] POSTServiceModelViews model)
        {
            return Ok(await _serviceService.CreateAsync(model));
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Update an existing service")]
        public async Task<IActionResult> Update([FromBody] PUTServiceModelViews model)
        {
            return Ok(await _serviceService.UpdateAsync(model));
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation(Summary = "Delete a service (soft delete)")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            return Ok(await _serviceService.DeleteAsync(id));
        }

    }
}