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
    public class ServiceController : ControllerBase
    {
        private readonly IServices _serviceService;

        public ServiceController(IServices serviceService)
        {
            _serviceService = serviceService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Create a new service")]
        public async Task<IActionResult> Create([FromBody] POSTServiceModelViews model)
        {
            try
            {
                var serviceId = await _serviceService.CreateAsync(model);
                return Ok(new BaseResponseModel<Guid>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: serviceId,
                    message: "Service created successfully."
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: ResponseCodeConstants.FAILED,
                    data: null,
                    message: ex.Message
                ));
            }
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get a paginated list of services")]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var services = await _serviceService.GetAllAsync(pageNumber, pageSize);
                return Ok(new BaseResponseModel<BasePaginatedList<GETServiceModelViews>>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: services
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: ResponseCodeConstants.FAILED,
                    data: null,
                    message: ex.Message
                ));
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a service by ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var service = await _serviceService.GetByIdAsync(id);
                return Ok(new BaseResponseModel<GETServiceModelViews>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: service
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: ResponseCodeConstants.FAILED,
                    data: null,
                    message: ex.Message
                ));
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Update an existing service")]
        public async Task<IActionResult> Update([FromBody] PUTServiceModelViews model)
        {
            try
            {
                await _serviceService.UpdateAsync(model);
                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: "Service updated successfully."
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: ResponseCodeConstants.FAILED,
                    data: null,
                    message: ex.Message
                ));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete a service (soft delete)")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _serviceService.DeleteAsync(id);
                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: "Service deleted successfully."
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: ResponseCodeConstants.FAILED,
                    data: null,
                    message: ex.Message
                ));
            }
        }
    }
}