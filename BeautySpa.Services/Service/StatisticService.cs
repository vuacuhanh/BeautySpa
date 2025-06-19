using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.StatisticModelViews;
using BeautySpa.Contract.Repositories.IUOW;
using Microsoft.EntityFrameworkCore;
using BeautySpa.Contract.Repositories.Entity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

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

        public async Task<BaseResponseModel<StatisticResultAdminModelView>> GetAdminStatisticsAsync(StatisticFilterModelView filter)
        {
            var appointmentRepo = _unitOfWork.GetRepository<Appointment>();
            var allAppointments = appointmentRepo.Entities
                .Include(a => a.Provider)!.ThenInclude(p => p.ServiceProvider)
                .Include(a => a.Payment)
                .Where(a => a.DeletedTime == null);

            if (filter.FromDate.HasValue)
                allAppointments = allAppointments.Where(a => a.AppointmentDate >= filter.FromDate);

            if (filter.ToDate.HasValue)
                allAppointments = allAppointments.Where(a => a.AppointmentDate <= filter.ToDate);

            var completedAppointments = allAppointments.Where(a => a.BookingStatus == "completed");

            // ✅ Top 5 spa theo số lượng lịch hoàn thành
            var topSpaByBooking = await completedAppointments
                .GroupBy(a => a.Provider!.ServiceProvider!.BusinessName)
                .Select(g => new TopProviderModel
                {
                    ProviderName = g.Key,
                    TotalAppointments = g.Count()
                })
                .OrderByDescending(x => x.TotalAppointments)
                .Take(5)
                .ToListAsync();

            // ✅ Top 5 spa theo doanh thu
            var topSpaByRevenue = await completedAppointments
                .GroupBy(a => a.Provider!.ServiceProvider!.BusinessName)
                .Select(g => new TopProviderModel
                {
                    ProviderName = g.Key,
                    TotalAppointments = 0, // tránh nhầm, không dùng field này
                    TotalRevenue = g.Sum(x => x.FinalPrice)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .Take(5)
                .ToListAsync();

            // ✅ Tổng hoa hồng đã nhận (PlatformFee)
            var totalCommission = await completedAppointments
                .Where(a => a.Payment != null && a.Payment.PlatformFee > 0)
                .SumAsync(a => (decimal?)a.Payment!.PlatformFee) ?? 0;

            // ✅ Tổng tiền cọc đã thanh toán
            var totalDeposit = await completedAppointments
                .Where(a => a.Payment != null && a.Payment.Status == "completed")
                .SumAsync(a => (decimal?)a.Payment!.Amount) ?? 0;

            // ✅ Tổng tài khoản từ UserManager
            var userManager = _httpContextAccessor.HttpContext!.RequestServices.GetRequiredService<UserManager<ApplicationUsers>>();
            var userCount = (await userManager.GetUsersInRoleAsync("Customer")).Count;
            var providerCount = (await userManager.GetUsersInRoleAsync("Provider")).Count;

            return BaseResponseModel<StatisticResultAdminModelView>.Success(new StatisticResultAdminModelView
            {
                TopBookedProviders = topSpaByBooking,
                TopRevenueProviders = topSpaByRevenue,
                TotalCommissionRevenue = totalCommission,
                TotalDepositAmount = totalDeposit,
                TotalUserCount = userCount,
                ApprovedProviderCount = providerCount
            });
        }


        public async Task<BaseResponseModel<StatisticResultProviderModelView>> GetProviderStatisticsByTokenAsync(StatisticFilterModelView filter)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var providerIdClaim = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(providerIdClaim) || !Guid.TryParse(providerIdClaim, out var providerId))
                throw new ErrorException(StatusCodes.Status401Unauthorized, ErrorCode.UnAuthorized, "Provider identity not found.");

            return await GetStatisticsForProvider(filter, providerId);
        }


        private async Task<BaseResponseModel<StatisticResultProviderModelView>> GetStatisticsForProvider(StatisticFilterModelView filter, Guid providerId)
        {
            var query = _unitOfWork.GetRepository<Appointment>().Entities
                .Where(a => a.DeletedTime == null && a.ProviderId == providerId);

            if (filter.FromDate.HasValue)
                query = query.Where(a => a.AppointmentDate >= filter.FromDate);
            if (filter.ToDate.HasValue)
                query = query.Where(a => a.AppointmentDate <= filter.ToDate);

            var today = DateTime.Today;
            var firstDayOfWeek = today.AddDays(-(int)today.DayOfWeek + 1);
            var firstDayOfYear = new DateTime(today.Year, 1, 1);

            var topToday = await query
                .Where(a => a.AppointmentDate == today)
                .SelectMany(a => a.AppointmentServices)
                .GroupBy(s => s.Service!.ServiceName)
                .Select(g => new TopServiceModel { ServiceName = g.Key, TotalBooked = g.Sum(s => s.Quantity) })
                .OrderByDescending(x => x.TotalBooked).Take(5).ToListAsync();

            var topWeek = await query
                .Where(a => a.AppointmentDate >= firstDayOfWeek)
                .SelectMany(a => a.AppointmentServices)
                .GroupBy(s => s.Service!.ServiceName)
                .Select(g => new TopServiceModel { ServiceName = g.Key, TotalBooked = g.Sum(s => s.Quantity) })
                .OrderByDescending(x => x.TotalBooked).Take(5).ToListAsync();

            var topYear = await query
                .Where(a => a.AppointmentDate >= firstDayOfYear)
                .SelectMany(a => a.AppointmentServices)
                .GroupBy(s => s.Service!.ServiceName)
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

            return BaseResponseModel<StatisticResultProviderModelView>.Success(new StatisticResultProviderModelView
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
