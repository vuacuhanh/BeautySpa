using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.StatisticModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeautySpa.API.Controllers
{
    [Route("api/statistic")]
    [ApiController]
    [SwaggerTag("Thống kê Provider/ Admin")]
    public class StatisticController : ControllerBase
    {
        private readonly IStatisticService _statisticService;

        public StatisticController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        [HttpPost("admin")]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Thống kê dành cho Admin")]
        public async Task<IActionResult> GetAdminStats([FromBody] StatisticFilterModelView filter)
        {
            var result = await _statisticService.GetAdminStatisticsAsync(filter);
            return Ok(result);
        }

        [HttpPost("provider")]
        //[Authorize(Roles = "Provider")]
        [SwaggerOperation(Summary = "Thống kê dành cho Provider hiện tại")]
        public async Task<IActionResult> GetProviderStats([FromBody] StatisticFilterModelView filter)
        {
            var result = await _statisticService.GetProviderStatisticsByTokenAsync(filter);
            return Ok(result);
        }
    }
}
