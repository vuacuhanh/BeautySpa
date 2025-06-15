using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.StatisticModelViews;
using BeautySpa.Contract.Repositories.IUOW;
using Microsoft.EntityFrameworkCore;
using BeautySpa.Contract.Repositories.Entity;
using Microsoft.AspNetCore.Http;
using System.Globalization;
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
            var appointments = _unitOfWork.GetRepository<Appointment>().Entities
                .Where(a => a.DeletedTime == null && a.BookingStatus == "completed");

            if (filter.FromDate.HasValue)
                appointments = appointments.Where(a => a.AppointmentDate >= filter.FromDate);
            if (filter.ToDate.HasValue)
                appointments = appointments.Where(a => a.AppointmentDate <= filter.ToDate);

            var totalCommission = await appointments
                .Include(a => a.Payment)
                .SumAsync(a => (decimal?)a.Payment.PlatformFee) ?? 0;

            var topProviders = await appointments
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

            var revenueByMonth = await appointments
                .GroupBy(a => a.AppointmentDate.ToString("yyyy-MM"))
                .Select(g => new MonthlyRevenueModel
                {
                    Month = g.Key,
                    Revenue = g.Sum(x => x.FinalPrice)
                }).ToListAsync();

            return BaseResponseModel<StatisticResultModelView>.Success(new StatisticResultModelView
            {
                TotalCommissionRevenue = totalCommission,
                TopBookedProviders = topProviders,
                ApprovedProviderCount = approvedProviders,
                RevenueByMonth = revenueByMonth
            });
        }

        public async Task<BaseResponseModel<StatisticResultModelView>> GetProviderStatisticsAsync(StatisticFilterModelView filter, Guid providerId)
        {
            return await GetStatisticsForProvider(filter, providerId);
        }

        public async Task<BaseResponseModel<StatisticResultModelView>> GetProviderStatisticsByTokenAsync(StatisticFilterModelView filter)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var providerId))
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
                .GroupBy(a => a.AppointmentDate.ToString("yyyy-MM"))
                .Select(g => new MonthlyRevenueModel
                {
                    Month = g.Key,
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
