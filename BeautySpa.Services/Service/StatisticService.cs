using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.StatisticModelViews;
using BeautySpa.Contract.Repositories.IUOW;
using Microsoft.EntityFrameworkCore;
using BeautySpa.Contract.Repositories.Entity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BeautySpa.Services.Service
{
    public class StatisticService : IStatisticService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StatisticService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BaseResponseModel<StatisticResultModelView>> GetAdminStatisticsAsync(StatisticFilterModelView filter)
        {
            return await GetStatistics(filter, null);
        }

        public async Task<BaseResponseModel<StatisticResultModelView>> GetProviderStatisticsAsync(StatisticFilterModelView filter, Guid providerId)
        {
            return await GetStatistics(filter, providerId);
        }

        public async Task<BaseResponseModel<StatisticResultModelView>> GetProviderStatisticsByTokenAsync(StatisticFilterModelView filter)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var providerIdClaim = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(providerIdClaim) || !Guid.TryParse(providerIdClaim, out var providerId))
                throw new ErrorException(StatusCodes.Status401Unauthorized, ErrorCode.UnAuthorized, "Provider identity not found.");

            return await GetStatistics(filter, providerId);
        }

        private async Task<BaseResponseModel<StatisticResultModelView>> GetStatistics(StatisticFilterModelView filter, Guid? providerId)
        {
            var appointmentRepo = _unitOfWork.GetRepository<Appointment>();

            var query = appointmentRepo.Entities
                .Where(a => a.DeletedTime == null);

            if (filter.FromDate.HasValue)
                query = query.Where(a => a.AppointmentDate >= filter.FromDate);
            if (filter.ToDate.HasValue)
                query = query.Where(a => a.AppointmentDate <= filter.ToDate);
            if (providerId.HasValue)
                query = query.Where(a => a.ProviderId == providerId);

            var totalAppointments = await query.CountAsync();
            var totalRevenue = await query.Where(a => a.BookingStatus == "completed").SumAsync(a => (decimal?)a.FinalPrice) ?? 0;

            var completed = await query.CountAsync(a => a.BookingStatus == "completed");
            var cancelled = await query.CountAsync(a => a.BookingStatus == "cancelled");
            var noShow = await query.CountAsync(a => a.BookingStatus == "no_show");

            var topServices = await query
                .SelectMany(a => a.AppointmentServices)
                .GroupBy(s => s.Service.ServiceName)
                .Select(g => new TopServiceModel
                {
                    ServiceName = g.Key,
                    TotalBooked = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalBooked)
                .Take(5)
                .ToListAsync();

            var result = new StatisticResultModelView
            {
                TotalAppointments = totalAppointments,
                TotalRevenue = totalRevenue,
                CompletedCount = completed,
                CancelledCount = cancelled,
                NoShowCount = noShow,
                TopServices = topServices
            };

            return BaseResponseModel<StatisticResultModelView>.Success(result);
        }
    }
}
