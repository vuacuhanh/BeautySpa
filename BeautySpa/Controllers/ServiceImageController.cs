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

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Create a new service image")]
        public async Task<IActionResult> Create([FromBody] POSTServiceImageModelViews model)
        {
            var imageId = await _imageService.CreateAsync(model);
            return Ok(new BaseResponseModel<Guid>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: imageId,
                message: "Service image created successfully."
            ));
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get a paginated list of service images")]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
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