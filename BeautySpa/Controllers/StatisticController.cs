using BeautySpa.Contract.Services.Interface;
using BeautySpa.ModelViews.StatisticModelViews;
using BeautySpa.Services.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeautySpa.API.Controllers
{
    [Route("api/statistic")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private readonly IStatisticService _statisticService;
        private readonly IHttpContextAccessor _contextAccessor;

        public StatisticController(IStatisticService statisticService, IHttpContextAccessor contextAccessor)
        {
            _statisticService = statisticService;
            _contextAccessor = contextAccessor;
        }

        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminStats([FromBody] StatisticFilterModelView filter)
        {
            var result = await _statisticService.GetAdminStatisticsAsync(filter);
            return Ok(result);
        }

        [HttpPost("provider")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> GetProviderStats([FromBody] StatisticFilterModelView filter)
        {
            var providerId = Guid.Parse(User.Claims.First(c => c.Type.Contains("nameidentifier")).Value);
            var result = await _statisticService.GetProviderStatisticsAsync(filter, providerId);
            return Ok(result);
        }
    }
}
