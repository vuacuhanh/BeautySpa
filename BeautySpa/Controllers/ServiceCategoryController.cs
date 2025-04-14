using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServiceCategoryModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;

namespace BeautySpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceCategoryController : ControllerBase
    {
        private readonly IServiceCategory _categoryService;

        public ServiceCategoryController(IServiceCategory categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new service category")]
        public async Task<IActionResult> Create([FromBody] POSTServiceCategoryModelViews model)
        {
            try
            {
                var categoryId = await _categoryService.CreateAsync(model);
                return Ok(new BaseResponseModel<Guid>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: categoryId,
                    message: "Service category created successfully."
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
        [SwaggerOperation(Summary = "Get a paginated list of service categories")]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var categories = await _categoryService.GetAllAsync(pageNumber, pageSize);
                return Ok(new BaseResponseModel<BasePaginatedList<GETServiceCategoryModelViews>>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: categories
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
        [SwaggerOperation(Summary = "Get a service category by ID")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var category = await _categoryService.GetByIdAsync(id);
                return Ok(new BaseResponseModel<GETServiceCategoryModelViews>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: category
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
        [SwaggerOperation(Summary = "Update an existing service category")]
        public async Task<IActionResult> Update([FromBody] PUTServiceCategoryModelViews model)
        {
            try
            {
                await _categoryService.UpdateAsync(model);
                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: "Service category updated successfully."
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
        [SwaggerOperation(Summary = "Delete a service category (soft delete)")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _categoryService.DeleteAsync(id);
            return Ok(new BaseResponseModel<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: "Service category deleted successfully."
             ));   
        }
    }
}