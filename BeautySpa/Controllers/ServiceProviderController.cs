using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServiceProviderModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Nhà cung cấp dịch vụ")]
    public class ServiceProviderController : ControllerBase
    {
        private readonly IServiceProviders _providerService;

        public ServiceProviderController(IServiceProviders providerService)
        {
            _providerService = providerService;
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Create a new service provider")]
        public async Task<IActionResult> Create([FromBody] POSTServiceProviderModelViews model)
        {
            var providerId = await _providerService.CreateAsync(model);
            return Ok(new BaseResponseModel<Guid>(
               statusCode: StatusCodes.Status200OK,
               code: ResponseCodeConstants.SUCCESS,
               data: providerId,
               message: "Service provider created successfully."
            ));
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get a paginated list of service providers")]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            var providers = await _providerService.GetAllAsync(pageNumber, pageSize);
            return Ok(new BaseResponseModel<BasePaginatedList<GETServiceProviderModelViews>>(
                 statusCode: StatusCodes.Status200OK,
                 code: ResponseCodeConstants.SUCCESS,
                 data: providers
             ));
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a service provider by ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var provider = await _providerService.GetByIdAsync(id);
            return Ok(new BaseResponseModel<GETServiceProviderModelViews>(
                 statusCode: StatusCodes.Status200OK,
                 code: ResponseCodeConstants.SUCCESS,
                 data: provider
             ));
        }

        [HttpPut]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Update an existing service provider")]
        public async Task<IActionResult> Update([FromBody] PUTServiceProviderModelViews model)
        {
            await _providerService.UpdateAsync(model);
            return Ok(new BaseResponseModel<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: "Service provider updated successfully."
            ));
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete a service provider (soft delete)")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _providerService.DeleteAsync(id);
            return Ok(new BaseResponseModel<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: "Service provider deleted successfully."
             ));
        }
    }
}