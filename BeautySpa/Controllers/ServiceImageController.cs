using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServiceImageModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceImageController : ControllerBase
    {
        private readonly IServiceImages _imageService;

        public ServiceImageController(IServiceImages imageService)
        {
            _imageService = imageService;
        }
        [HttpGet]
        [SwaggerOperation(Summary = "Get a paginated list of service images")]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize)
        {
            var images = await _imageService.GetAllAsync(pageNumber, pageSize);
            return Ok(new BaseResponseModel<BasePaginatedList<GETServiceImageModelViews>>(
               statusCode: StatusCodes.Status200OK,
               code: ResponseCodeConstants.SUCCESS,
               data: images
            ));
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a service image by ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var image = await _imageService.GetByIdAsync(id);
            return Ok(new BaseResponseModel<GETServiceImageModelViews>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: image
            ));
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Create a new service image")]
        public async Task<IActionResult> Create([FromBody] POSTServiceImageModelViews model)
        {
            var imageId = await _imageService.CreateAsync(model);
            return Ok(new BaseResponseModel<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: "Service image created successfully."
            ));
        }

     
        [HttpPut]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Update an existing service image")]
        public async Task<IActionResult> Update([FromBody] PUTServiceImageModelViews model)
        {
            await _imageService.UpdateAsync(model);
            return Ok(new BaseResponseModel<string>(
               statusCode: StatusCodes.Status200OK,
               code: ResponseCodeConstants.SUCCESS,
               data: "Service image updated successfully."
            ));

        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete a service image (soft delete)")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _imageService.DeleteAsync(id);
            return Ok(new BaseResponseModel<string>(
               statusCode: StatusCodes.Status200OK,
               code: ResponseCodeConstants.SUCCESS,
               data: "Service image deleted successfully."
             ));
        }
    }
}