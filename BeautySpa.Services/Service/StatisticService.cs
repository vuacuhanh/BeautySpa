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
            var allAppointments = _unitOfWork.GetRepository<Appointment>().Entities
                .Where(a => a.DeletedTime == null);

            if (filter.FromDate.HasValue)
                allAppointments = allAppointments.Where(a => a.AppointmentDate >= filter.FromDate);
            if (filter.ToDate.HasValue)
                allAppointments = allAppointments.Where(a => a.AppointmentDate <= filter.ToDate);

            var completedAppointments = allAppointments.Where(a => a.BookingStatus == "completed");

            var totalAppointments = await allAppointments.CountAsync();
            var completedCount = await allAppointments.CountAsync(a => a.BookingStatus == "completed");
            var cancelledCount = await allAppointments.CountAsync(a => a.BookingStatus == "canceled");
            var noShowCount = await allAppointments.CountAsync(a => a.BookingStatus == "no_show");

            var totalRevenue = await completedAppointments.SumAsync(a => (decimal?)a.FinalPrice) ?? 0;
            var totalCommission = await completedAppointments
                .Include(a => a.Payment)
                .SumAsync(a => (decimal?)a.Payment.PlatformFee) ?? 0;

            var revenueByMonth = await completedAppointments
                .GroupBy(a => new { a.AppointmentDate.Year, a.AppointmentDate.Month })
                .Select(g => new MonthlyRevenueModel
                {
                    Month = $"{g.Key.Year:D4}-{g.Key.Month:D2}",
                    Revenue = g.Sum(x => x.FinalPrice)
                }).ToListAsync();

            var topProviders = await completedAppointments
                .GroupBy(a => a.Provider!.ServiceProvider!.BusinessName)
                .Select(g => new TopProviderModel
                {
                    ProviderName = g.Key,
                    TotalAppointments = g.Count()
                })
                .OrderByDescending(x => x.TotalAppointments)
                .Take(5)
                .ToListAsync();

            var approvedProviders = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.CountAsync(p => p.IsApproved);

            return BaseResponseModel<StatisticResultModelView>.Success(new StatisticResultModelView
            {
                TotalAppointments = totalAppointments,
                CompletedCount = completedCount,
                CancelledCount = cancelledCount,
                NoShowCount = noShowCount,
                TotalRevenue = totalRevenue,
                TotalCommissionRevenue = totalCommission,
                RevenueByMonth = revenueByMonth,
                TopBookedProviders = topProviders,
                ApprovedProviderCount = approvedProviders,
                TopServicesToday = new(),
                TopServicesWeek = new(),
                TopServicesYear = new()
            });
        }

        public async Task<BaseResponseModel<StatisticResultModelView>> GetProviderStatisticsByTokenAsync(StatisticFilterModelView filter)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var providerIdClaim = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(providerIdClaim) || !Guid.TryParse(providerIdClaim, out var providerId))
                throw new ErrorException(StatusCodes.Status401Unauthorized, ErrorCode.UnAuthorized, "Provider identity not found.");

            return await GetStatisticsForProvider(filter, providerId);
        }

        private async Task<BaseResponseModel<StatisticResultModelView>> GetStatisticsForProvider(StatisticFilterModelView filter, Guid providerId)
        {
            var query = _unitOfWork.GetRepository<Appointment>().Entities
                .Where(a => a.DeletedTime == null && a.ProviderId == providerId);

            if (filter.FromDate.HasValue)
                query = query.Where(a => a.AppointmentDate >= filter.FromDate);
            if (filter.ToDate.HasValue)
                query = query.Where(a => a.AppointmentDate <= filter.ToDate);

            var today = DateTime.Today;
            var firstDayOfWeek = today.AddDays(-(int)today.DayOfWeek + 1); // Monday
            var firstDayOfYear = new DateTime(today.Year, 1, 1);

            var topToday = await query
                .Where(a => a.AppointmentDate == today)
                .SelectMany(a => a.AppointmentServices)
                .GroupBy(s => s.Service.ServiceName)
                .Select(g => new TopServiceModel { ServiceName = g.Key, TotalBooked = g.Sum(s => s.Quantity) })
                .OrderByDescending(x => x.TotalBooked).Take(5).ToListAsync();

            var topWeek = await query
                .Where(a => a.AppointmentDate >= firstDayOfWeek)
                .SelectMany(a => a.AppointmentServices)
                .GroupBy(s => s.Service.ServiceName)
                .Select(g => new TopServiceModel { ServiceName = g.Key, TotalBooked = g.Sum(s => s.Quantity) })
                .OrderByDescending(x => x.TotalBooked).Take(5).ToListAsync();

            var topYear = await query
                .Where(a => a.AppointmentDate >= firstDayOfYear)
                .SelectMany(a => a.AppointmentServices)
                .GroupBy(s => s.Service.ServiceName)
                .Select(g => new TopServiceModel { ServiceName = g.Key, TotalBooked = g.Sum(s => s.Quantity) })
                .OrderByDescending(x => x.TotalBooked).Take(5).ToListAsync();

            var revenueByMonth = await query
                .Where(a => a.BookingStatus == "completed")
                .GroupBy(a => new { a.AppointmentDate.Year, a.AppointmentDate.Month })
                .Select(g => new MonthlyRevenueModel
                {
                    Month = $"{g.Key.Year:D4}-{g.Key.Month:D2}",
                    Revenue = g.Sum(x => x.FinalPrice)
                }).ToListAsync();

            return BaseResponseModel<StatisticResultModelView>.Success(new StatisticResultModelView
            {
                TotalAppointments = await query.CountAsync(),
                CompletedCount = await query.CountAsync(x => x.BookingStatus == "completed"),
                CancelledCount = await query.CountAsync(x => x.BookingStatus == "cancelled"),
                NoShowCount = await query.CountAsync(x => x.BookingStatus == "no_show"),
                TotalRevenue = await query.Where(x => x.BookingStatus == "completed").SumAsync(x => (decimal?)x.FinalPrice) ?? 0,
                TopServicesToday = topToday,
                TopServicesWeek = topWeek,
                TopServicesYear = topYear,
                RevenueByMonth = revenueByMonth
            });
        }
    }
}
