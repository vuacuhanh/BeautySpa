using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.AppointmentModelViews;
using BeautySpa.ModelViews.NotificationModelViews;
using BeautySpa.ModelViews.PaymentModelViews;
using BeautySpa.Services.Validations.AppoitmentValidator;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Entity = BeautySpa.Contract.Repositories.Entity;

namespace BeautySpa.Services.Service
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _context;
        private readonly IPaymentService _paymentService;
        private readonly INotificationService _notificationService;

        public AppointmentService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor context,
            IPaymentService paymentService,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
            _paymentService = paymentService;
            _notificationService = notificationService;
        }

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_context);

        private async Task SendNotificationAsync(Guid userId, string title, string message)
        {
            await _notificationService.CreateAsync(new POSTNotificationModelView
            {
                UserId = userId,
                Title = title,
                Message = message
            });

            await _notificationService.SendSocketAsync(userId, new
            {
                Type = "appointment",
                Title = title,
                Message = message
            });
        }

        public async Task<BaseResponseModel<Guid>> CreateAsync(POSTAppointmentModelView model)
        {
            await new POSTAppointmentModelViewValidator().ValidateAndThrowAsync(model);
            var now = CoreHelper.SystemTimeNow;
            var userId = Guid.Parse(CurrentUserId);

            var firstService = await _unitOfWork.GetRepository<Entity.Service>()
                .Entities.Include(x => x.Provider)
                .FirstOrDefaultAsync(x => x.Id == model.Services[0].ServiceId)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Service not found.");

            var providerId = firstService.ProviderId;
            int dayOfWeek = (int)model.AppointmentDate.DayOfWeek;
            var workingHour = await _unitOfWork.GetRepository<WorkingHour>()
                .Entities.FirstOrDefaultAsync(x =>
                    x.ServiceProviderId == providerId &&
                    x.DayOfWeek == dayOfWeek &&
                    x.IsWorking &&
                    x.OpeningTime <= model.StartTime &&
                    x.ClosingTime >= model.StartTime);
            if (workingHour == null)
                throw new ErrorException(400, ErrorCode.Failed, "Outside of working hours.");

            int maxDuration = await _unitOfWork.GetRepository<Entity.Service>()
                .Entities.Where(s => model.Services.Select(x => x.ServiceId).Contains(s.Id))
                .Select(s => s.DurationMinutes)
                .MaxAsync();

            TimeSpan endTime = model.StartTime + TimeSpan.FromMinutes(maxDuration);

            var slotUsed = await _unitOfWork.GetRepository<Appointment>()
                .Entities.Where(a =>
                    a.ProviderId == providerId &&
                    a.AppointmentDate == model.AppointmentDate &&
                    new[] { "pending", "confirmed", "checked_in" }.Contains(a.BookingStatus) &&
                    a.DeletedTime == null &&
                    a.StartTime < endTime)
                .CountAsync();

            int maxSlot = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.Where(x => x.ProviderId == providerId)
                .Select(x => x.MaxAppointmentsPerSlot)
                .FirstOrDefaultAsync();

            if (slotUsed >= maxSlot)
                throw new ErrorException(400, ErrorCode.Failed, "Time slot is fully booked.");

            decimal originalTotal = 0;
            List<Entity.AppointmentService> appointmentServices = new();

            foreach (var s in model.Services)
            {
                var service = await _unitOfWork.GetRepository<Entity.Service>().GetByIdAsync(s.ServiceId)
                    ?? throw new ErrorException(404, ErrorCode.NotFound, "Service not found.");

                decimal price = service.Price;
                var flash = await _unitOfWork.GetRepository<ServicePromotion>().Entities
                    .FirstOrDefaultAsync(f => f.ServiceId == s.ServiceId && f.StartDate <= now && f.EndDate >= now);
                if (flash != null)
                {
                    price -= flash.DiscountPercent > 0 ? price * flash.DiscountPercent.Value / 100 : flash.DiscountAmount ?? 0;
                    flash.UsedCount++;
                }

                appointmentServices.Add(new Entity.AppointmentService
                {
                    Id = Guid.NewGuid(),
                    ServiceId = s.ServiceId,
                    Quantity = s.Quantity,
                    PriceAtBooking = price
                });

                originalTotal += price * s.Quantity;
            }

            decimal discount = 0;
            if (model.PromotionId.HasValue)
            {
                var promo = await _unitOfWork.GetRepository<Promotion>().GetByIdAsync(model.PromotionId.Value);
                if (promo != null && promo.IsActive && now >= promo.StartDate && now <= promo.EndDate)
                {
                    discount = promo.DiscountPercent > 0
                        ? originalTotal * promo.DiscountPercent.Value / 100
                        : promo.DiscountAmount ?? 0;
                    promo.TotalUsed++;
                }
            }

            if (model.PromotionAdminId.HasValue)
            {
                var promoAdmin = await _unitOfWork.GetRepository<PromotionAdmin>().GetByIdAsync(model.PromotionAdminId.Value);
                var rankId = await _unitOfWork.GetRepository<MemberShip>().Entities
                    .Where(m => m.UserId == userId)
                    .Select(m => m.RankId)
                    .FirstOrDefaultAsync();

                if (promoAdmin != null && promoAdmin.IsActive && now >= promoAdmin.StartDate && now <= promoAdmin.EndDate && promoAdmin.PromotionAdminRanks.Any(x => x.RankId == rankId))
                {
                    discount += promoAdmin.DiscountPercent > 0
                        ? originalTotal * promoAdmin.DiscountPercent.Value / 100
                        : promoAdmin.DiscountAmount ?? 0;
                    promoAdmin.TotalUsed++;
                }
            }

            decimal finalPrice = originalTotal - discount;
            decimal depositPercent = finalPrice switch
            {
                < 100_000 => 1.0m,
                < 300_000 => 0.5m,
                < 1_000_000 => 0.3m,
                _ => 0.2m
            };
            decimal depositAmount = Math.Round(finalPrice * depositPercent, 0);

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                AppointmentDate = model.AppointmentDate,
                StartTime = model.StartTime,
                SpaBranchLocationId = model.SpaBranchLocationId,
                Notes = model.Notes,
                CustomerId = userId,
                ProviderId = providerId,
                PromotionId = model.PromotionId,
                PromotionAdminId = model.PromotionAdminId,
                OriginalTotalPrice = originalTotal,
                DiscountAmount = discount,
                FinalPrice = finalPrice,
                CreatedBy = CurrentUserId,
                CreatedTime = now.DateTime,
                AppointmentServices = appointmentServices
            };

            await _unitOfWork.GetRepository<Appointment>().InsertAsync(appointment);
            await _unitOfWork.SaveAsync();

            await _paymentService.CreateDepositAsync(new POSTPaymentModelView
            {
                AppointmentId = appointment.Id,
                Amount = depositAmount,
                PaymentMethod = model.PaymentMethod ?? "momo"
            });

            await SendNotificationAsync(userId, "Đặt lịch thành công",
                $"Bạn đã đặt lịch lúc {model.StartTime:hh\\:mm dd/MM/yyyy}. Vui lòng thanh toán tiền cọc.");

            return BaseResponseModel<Guid>.Success(appointment.Id);
        }
        public async Task<BaseResponseModel<string>> AutoCancelUnpaidAppointmentsAsync()
        {
            var now = CoreHelper.SystemTimeNow;

            var appointments = await _unitOfWork.GetRepository<Appointment>()
                .Entities.Include(x => x.Payment)
                .Include(x => x.AppointmentServices)
                .Where(x => x.BookingStatus == "pending"
                         && x.CreatedTime <= now.AddMinutes(-10).DateTime
                         && x.DeletedTime == null)
                .ToListAsync();

            foreach (var appointment in appointments)
            {
                appointment.BookingStatus = "canceled";
                appointment.DeletedTime = now;
                appointment.LastUpdatedTime = now;

                if (appointment.Payment != null && appointment.Payment.Status != "refunded")
                {
                    appointment.Payment.RefundAmount = appointment.Payment.Amount;
                    appointment.Payment.PlatformFee = 0;
                    appointment.Payment.Status = "refunded";
                }

                await ReturnPromotionsAsync(appointment);

                await SendNotificationAsync(appointment.CustomerId, "Lịch hẹn bị hủy",
                    "Bạn chưa thanh toán cọc, lịch đã bị hủy sau 10 phút.");
            }

            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("Đã hủy các lịch quá hạn chưa thanh toán.");
        }
        public async Task<BaseResponseModel<string>> UpdateStatusAsync(Guid appointmentId, string status)
        {
            var appointment = await _unitOfWork.GetRepository<Appointment>()
                .Entities.Include(x => x.Payment)
                .Include(x => x.AppointmentServices)
                .FirstOrDefaultAsync(x => x.Id == appointmentId && x.DeletedTime == null)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Không tìm thấy lịch.");

            var now = CoreHelper.SystemTimeNow;
            DateTime appointmentTime = appointment.AppointmentDate.Date + appointment.StartTime;
            double remainingMinutes = (appointmentTime - now).TotalMinutes;
            bool isLate = remainingMinutes < 25;

            if (status.Equals("completed", StringComparison.OrdinalIgnoreCase))
            {
                appointment.BookingStatus = "completed";

                if (appointment.Payment != null)
                {
                    appointment.Payment.Status = "completed";
                    appointment.Payment.PlatformFee = Math.Round(appointment.FinalPrice * 0.1m, 0);
                }

                var member = await _unitOfWork.GetRepository<MemberShip>()
                    .Entities.FirstOrDefaultAsync(x => x.UserId == appointment.CustomerId);
                if (member != null)
                {
                    member.AccumulatedPoints += (int)(appointment.FinalPrice / 1000);
                }

                await SendNotificationAsync(appointment.CustomerId, "Lịch đã hoàn tất",
                    "Cảm ơn bạn! Spa rất vui khi được phục vụ bạn.");
            }
            else if (status.Equals("canceled", StringComparison.OrdinalIgnoreCase))
            {
                appointment.BookingStatus = "canceled";

                if (appointment.Payment != null && appointment.Payment.Status != "refunded")
                {
                    decimal fee = isLate ? Math.Round(appointment.FinalPrice * 0.1m, 0) : 0;
                    appointment.Payment.RefundAmount = appointment.Payment.Amount - fee;
                    appointment.Payment.PlatformFee = fee;
                    appointment.Payment.Status = "refunded";
                }

                await ReturnPromotionsAsync(appointment);

                await SendNotificationAsync(appointment.CustomerId, "Lịch hẹn đã hủy",
                    isLate
                        ? "Bạn đã hủy trễ – hệ thống đã trừ phí và hoàn lại tiền cọc."
                        : "Bạn đã hủy lịch – tiền cọc đã được hoàn lại đầy đủ.");
            }
            else if (status.Equals("no_show", StringComparison.OrdinalIgnoreCase))
            {
                appointment.BookingStatus = "no_show";

                if (appointment.Payment != null && appointment.Payment.Status != "refunded")
                {
                    decimal fee = Math.Round(appointment.FinalPrice * 0.1m, 0);
                    appointment.Payment.RefundAmount = appointment.Payment.Amount - fee;
                    appointment.Payment.PlatformFee = fee;
                    appointment.Payment.Status = "refunded";
                }

                await ReturnPromotionsAsync(appointment);

                await SendNotificationAsync(appointment.CustomerId, "Bạn đã không đến",
                    "Lịch hẹn của bạn đã bị hủy. Hệ thống đã hoàn lại cọc sau khi trừ phí.");
            }
            else if (status.Equals("checked_in", StringComparison.OrdinalIgnoreCase))
            {
                appointment.BookingStatus = "checked_in";

                await SendNotificationAsync(appointment.CustomerId, "Check-in thành công",
                    "Chúc bạn có trải nghiệm làm đẹp tuyệt vời tại Spa.");
            }
            else if (status.Equals("confirmed", StringComparison.OrdinalIgnoreCase))
            {
                appointment.BookingStatus = "confirmed";
                appointment.IsConfirmedBySpa = true;
                appointment.ConfirmationTime = now.DateTime;

                await SendNotificationAsync(appointment.CustomerId, "Lịch hẹn được xác nhận",
                    "Lịch hẹn của bạn đã được spa xác nhận thành công.");
            }
            else
            {
                throw new ErrorException(400, ErrorCode.Failed, "Trạng thái không hợp lệ.");
            }

            appointment.LastUpdatedTime = now;
            appointment.LastUpdatedBy = CurrentUserId;
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Cập nhật trạng thái thành công.");
        }
        public async Task<BaseResponseModel<string>> AutoNoShowAfter12HoursAsync()
        {
            var now = CoreHelper.SystemTimeNow;

            var appointments = await _unitOfWork.GetRepository<Appointment>()
                .Entities.Include(x => x.Payment)
                .Include(x => x.AppointmentServices)
                .Where(x =>
                    (x.BookingStatus == "pending" || x.BookingStatus == "confirmed") &&
                    x.AppointmentDate.Add(x.StartTime).AddHours(12) <= now &&
                    x.DeletedTime == null)
                .ToListAsync();

            foreach (var appointment in appointments)
            {
                appointment.BookingStatus = "no_show";

                if (appointment.Payment != null && appointment.Payment.Status != "refunded")
                {
                    decimal fee = Math.Round(appointment.FinalPrice * 0.1m, 0);
                    appointment.Payment.RefundAmount = appointment.Payment.Amount - fee;
                    appointment.Payment.PlatformFee = fee;
                    appointment.Payment.Status = "refunded";
                }

                await ReturnPromotionsAsync(appointment);

                await SendNotificationAsync(appointment.CustomerId, "Bạn đã không đến",
                    "Hệ thống đã tự động hủy lịch sau 12 giờ kể từ giờ hẹn. Tiền cọc đã được hoàn lại sau khi trừ phí.");
            }

            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("Đã tự động xử lý no_show cho các lịch quá hạn.");
        }
        public async Task<BaseResponseModel<string>> UpdateAsync(PUTAppointmentModelView model)
        {
            await new PUTAppointmentModelViewValidator().ValidateAndThrowAsync(model);

            var appointment = await _unitOfWork.GetRepository<Appointment>()
                .Entities.Include(a => a.AppointmentServices)
                .FirstOrDefaultAsync(a => a.Id == model.Id && a.DeletedTime == null)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Không tìm thấy lịch hẹn.");

            if (appointment.BookingStatus is "canceled" or "completed")
                throw new ErrorException(400, ErrorCode.Failed, "Không thể cập nhật lịch đã hủy hoặc hoàn tất.");

            appointment.AppointmentDate = model.AppointmentDate;
            appointment.StartTime = model.StartTime;
            appointment.SpaBranchLocationId = model.SpaBranchLocationId;
            appointment.Notes = model.Notes;
            appointment.LastUpdatedBy = CurrentUserId;
            appointment.LastUpdatedTime = CoreHelper.SystemTimeNow;

            foreach (var s in appointment.AppointmentServices.ToList())
                _unitOfWork.GetRepository<Entity.AppointmentService>().Delete1(s);

            decimal total = 0;
            foreach (var s in model.Services)
            {
                var service = await _unitOfWork.GetRepository<Entity.Service>().GetByIdAsync(s.ServiceId)
                    ?? throw new ErrorException(404, ErrorCode.NotFound, "Dịch vụ không tồn tại.");

                appointment.AppointmentServices.Add(new Entity.AppointmentService
                {
                    Id = Guid.NewGuid(),
                    ServiceId = s.ServiceId,
                    Quantity = s.Quantity,
                    PriceAtBooking = service.Price
                });

                total += service.Price * s.Quantity;
            }

            appointment.OriginalTotalPrice = total;
            appointment.FinalPrice = total; // Nếu muốn tính lại khuyến mãi có thể thêm logic ở đây

            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("Cập nhật lịch hẹn thành công.");
        }
        public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
        {
            var appointment = await _unitOfWork.GetRepository<Appointment>()
                .GetByIdAsync(id)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Không tìm thấy lịch.");

            appointment.DeletedBy = CurrentUserId;
            appointment.DeletedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("Đã xóa lịch hẹn.");
        }
        public async Task<BaseResponseModel<BasePaginatedList<GETAppointmentModelView>>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _unitOfWork.GetRepository<Appointment>()
                .Entities.AsNoTracking()
                .Include(x => x.AppointmentServices).ThenInclude(s => s.Service)
                .Where(x => x.DeletedTime == null)
                .OrderByDescending(x => x.CreatedTime);

            var result = await _unitOfWork.GetRepository<Appointment>().GetPagging(query, pageNumber, pageSize);
            var mapped = result.Items.Select(x => _mapper.Map<GETAppointmentModelView>(x)).ToList();

            var paged = new BasePaginatedList<GETAppointmentModelView>(mapped, result.TotalItems, pageNumber, pageSize);
            return BaseResponseModel<BasePaginatedList<GETAppointmentModelView>>.Success(paged);
        }
        public async Task<BaseResponseModel<GETAppointmentModelView>> GetByIdAsync(Guid id)
        {
            var appointment = await _unitOfWork.GetRepository<Appointment>()
                .Entities.AsNoTracking()
                .Include(x => x.AppointmentServices).ThenInclude(s => s.Service)
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Không tìm thấy lịch.");

            var result = _mapper.Map<GETAppointmentModelView>(appointment);
            return BaseResponseModel<GETAppointmentModelView>.Success(result);
        }
    }
}