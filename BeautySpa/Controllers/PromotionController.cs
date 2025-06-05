using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.PromotionModelViews;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[Route("api/[controller]")]
[ApiController]
[SwaggerTag("Manage Provider Voucher Promotions")]
public class PromotionController : ControllerBase
{
    private readonly IPromotionService _service;

    public PromotionController(IPromotionService service)
    {
        _service = service;
    }

    [HttpGet("all")]
    [SwaggerOperation(Summary = "Get all promotions (paginated)")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        return Ok(await _service.GetAllAsync(page, size));
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get promotion by ID")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Ok(await _service.GetByIdAsync(id));
    }

    [HttpPost("create")]
    [SwaggerOperation(Summary = "Create new promotion")]
    public async Task<IActionResult> Create([FromBody] POSTPromotionModelView model)
    {
        return Ok(await _service.CreateAsync(model));
    }

    [HttpPut("update")]
    [SwaggerOperation(Summary = "Update promotion")]
    public async Task<IActionResult> Update([FromBody] PUTPromotionModelView model)
    {
        return Ok(await _service.UpdateAsync(model));
    }

    [HttpDelete("delete/{id}")]
    [SwaggerOperation(Summary = "Delete promotion")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Ok(await _service.DeleteAsync(id));
    }

    [HttpGet("by-provider/{providerId}")]
    [SwaggerOperation(Summary = "Get all promotions by ProviderId")]
    public async Task<IActionResult> GetByProvider(Guid providerId)
    {
        return Ok(await _service.GetByProviderIdAsync(providerId));
    }
    [HttpDelete("hard/{id}")]
    [SwaggerOperation(Summary = "Xoá cứng khuyến mãi khỏi hệ thống")]
    public async Task<IActionResult> DeletePermanent(Guid id)
    {
        var result = await _service.DeleteHardAsync(id);
        return Ok(result);
    }
}

