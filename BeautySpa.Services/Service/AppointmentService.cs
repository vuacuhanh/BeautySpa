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
using BeautySpa.Repositories.UOW;
using BeautySpa.Services.Validations.AppoitmentValidator;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Entity = BeautySpa.Contract.Repositories.Entity;

namespace BeautySpa.Services.Service
{
    public partial class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _context;
        private readonly IPaymentService _paymentService;
        private readonly INotificationService _notificationService;
        private readonly IStaff _staffService;

        public AppointmentService(
            IUnitOfWork unitOfWork,
            IStaff staffService,
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
            _staffService = staffService;
        }

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_context);

        //tạo đặt lịch
        public async Task<BaseResponseModel<AppointmentCreatedResult>> CreateAsync(POSTAppointmentModelView model)
        {
            var unitOfWorkImpl = _unitOfWork as UnitOfWork
                ?? throw new InvalidCastException("IUnitOfWork must be UnitOfWork");

            var strategy = unitOfWorkImpl.Context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await unitOfWorkImpl.Context.Database.BeginTransactionAsync();
                var result = await CreateAppointmentInternalAsync(model);
                await transaction.CommitAsync();
                return result;
            });
        }

        private async Task<BaseResponseModel<AppointmentCreatedResult>> CreateAppointmentInternalAsync(POSTAppointmentModelView model)
        {
            await new POSTAppointmentModelViewValidator().ValidateAndThrowAsync(model);

            var userId = Guid.Parse(CurrentUserId);
            var now = CoreHelper.SystemTimeNow;

            var provider = await GetProviderAsync(model.SpaBranchLocationId);

            await ValidateWorkingHoursAsync(model.SpaBranchLocationId, model.AppointmentDate, model.StartTime);
            await ValidateSlotAvailabilityAsync(provider, model);

            if (model.StaffId.HasValue)
            {
                var staff = await _unitOfWork.GetRepository<Staff>()
                    .Entities.FirstOrDefaultAsync(s =>
                        s.Id == model.StaffId.Value &&
                        s.BranchId == model.SpaBranchLocationId &&
                        s.DeletedTime == null);

                if (staff == null)
                    throw new ErrorException(400, ErrorCode.Failed, "Nhân viên không hợp lệ cho chi nhánh này.");
            }

            var (appointmentServices, originalTotal) = await BuildAppointmentServicesAsync(model, now);
            var discount = await CalculateTotalDiscountAsync(model, userId, originalTotal, now);

            var finalPrice = originalTotal - discount;
            var depositAmount = CalculateDepositAmount(finalPrice);

            var appointment = await CreateAppointmentEntityAsync(
                model,
                userId,
                provider,
                appointmentServices,
                originalTotal,
                discount,
                finalPrice,
                now
            );

            var paymentMethod = model.PaymentMethod?.ToLower() ?? "momo";

            var paymentResult = await _paymentService.CreateDepositAsync(new POSTPaymentModelView
            {
                AppointmentId = appointment.Id,
                Amount = (int)depositAmount,
                PaymentMethod = paymentMethod
            });

            if (paymentResult.StatusCode != 200 ||
            (paymentMethod != "cash" && string.IsNullOrEmpty(paymentResult.Data?.PayUrl)))
            {
                throw new ErrorException(400, ErrorCode.Failed, "Tạo giao dịch thanh toán thất bại.");
            }

            var startDateTime = model.AppointmentDate.Date + model.StartTime;

            await _notificationService.CreateAsync(new POSTNotificationModelView
            {
                UserId = userId,
                Title = "Đặt lịch thành công",
                Message = $"Bạn đã đặt lịch lúc {startDateTime:HH\\:mm dd/MM/yyyy}. Vui lòng thanh toán tiền cọc.",
                NotificationType = "appointment"
            });

            return BaseResponseModel<AppointmentCreatedResult>.Success(new AppointmentCreatedResult
            {
                AppointmentId = appointment.Id,
                PaymentMethod = paymentMethod,
                PayUrl = paymentResult.Data?.PayUrl,
                QrCodeUrl = paymentResult.Data?.QrCodeUrl
            });
        }

        // ---- Các phương thức phụ trợ ----

        private async Task<ServiceProvider> GetProviderAsync(Guid spaBranchLocationId)
        {
            var branch = await _unitOfWork.GetRepository<SpaBranchLocation>()
                .Entities.Include(x => x.Provider)
                .FirstOrDefaultAsync(x => x.Id == spaBranchLocationId)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Branch not found.");

            return branch.Provider
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Provider not found.");
        }

        private void ValidateWorkingHours(ServiceProvider provider, TimeSpan startTime)
        {
            if (!provider.OpenTime.HasValue || !provider.CloseTime.HasValue)
                throw new ErrorException(400, ErrorCode.Failed, "Provider working hours not configured.");

            if (startTime < provider.OpenTime || startTime >= provider.CloseTime)
                throw new ErrorException(400, ErrorCode.Failed, $"Outside working hours. Open: {provider.OpenTime:hh\\:mm}, Close: {provider.CloseTime:hh\\:mm}");
        }

        private async Task ValidateSlotAvailabilityAsync(ServiceProvider provider, POSTAppointmentModelView model)
        {
            int maxDuration = await _unitOfWork.GetRepository<Entity.Service>()
                .Entities.Where(s => model.Services.Select(x => x.ServiceId).Contains(s.Id))
                .MaxAsync(s => s.DurationMinutes);

            TimeSpan endTime = model.StartTime + TimeSpan.FromMinutes(maxDuration);

            var slotUsed = await _unitOfWork.GetRepository<Appointment>()
                .Entities.Where(a =>
                    a.ProviderId == provider.ProviderId &&
                    a.AppointmentDate == model.AppointmentDate &&
                    new[] { "pending", "confirmed", "checked_in" }.Contains(a.BookingStatus) &&
                    a.DeletedTime == null &&
                    a.StartTime < endTime)
                .CountAsync();

            if (slotUsed >= provider.MaxAppointmentsPerSlot)
                throw new ErrorException(400, ErrorCode.Failed, $"Time slot fully booked. Used: {slotUsed}, Max: {provider.MaxAppointmentsPerSlot}");
        }

        private async Task<(List<Entity.AppointmentService>, decimal)> BuildAppointmentServicesAsync(POSTAppointmentModelView model, DateTimeOffset now)
        {
            decimal originalTotal = 0;
            var appointmentServices = new List<Entity.AppointmentService>();

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
                    flash.Quantity++;
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

            return (appointmentServices, originalTotal);
        }

        private async Task<decimal> CalculateTotalDiscountAsync(POSTAppointmentModelView model, Guid userId, decimal originalTotal, DateTimeOffset now)
        {
            decimal discount = 0;

            if (model.PromotionId.HasValue)
            {
                discount += await CalculatePromotionDiscountAsync(model.PromotionId.Value, originalTotal, now);
            }

            if (model.PromotionAdminId.HasValue)
            {
                discount += await CalculatePromotionAdminDiscountAsync(model.PromotionAdminId.Value, userId, originalTotal, now);
            }

            return discount;
        }

        private async Task<decimal> CalculatePromotionDiscountAsync(Guid promoId, decimal originalTotal, DateTimeOffset now)
        {
            var promo = await _unitOfWork.GetRepository<Promotion>().GetByIdAsync(promoId)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Promotion không tồn tại.");

            if (promo.IsActive && now >= promo.StartDate && now <= promo.EndDate)
            {
                promo.Quantity++;
                return promo.DiscountPercent > 0
                    ? originalTotal * promo.DiscountPercent.Value / 100
                    : promo.DiscountAmount ?? 0;
            }

            return 0;
        }

        private async Task<decimal> CalculatePromotionAdminDiscountAsync(Guid promoAdminId, Guid userId, decimal originalTotal, DateTimeOffset now)
        {
            var promoAdmin = await _unitOfWork.GetRepository<PromotionAdmin>().Entities
                .Include(pa => pa.PromotionAdminRanks)
                .FirstOrDefaultAsync(pa => pa.Id == promoAdminId)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "PromotionAdmin không tồn tại.");

            var rankId = await _unitOfWork.GetRepository<MemberShip>().Entities
                .Where(m => m.UserId == userId)
                .Select(m => m.RankId)
                .FirstOrDefaultAsync();

            if (promoAdmin.IsActive && now >= promoAdmin.StartDate && now <= promoAdmin.EndDate &&
                promoAdmin.PromotionAdminRanks.Any(x => x.RankId == rankId))
            {
                promoAdmin.Quantity++;
                return promoAdmin.DiscountPercent > 0
                    ? originalTotal * promoAdmin.DiscountPercent.Value / 100
                    : promoAdmin.DiscountAmount ?? 0;
            }

            return 0;
        }

        private decimal CalculateDepositAmount(decimal finalPrice)
        {
            decimal depositPercent = finalPrice switch
            {
                < 100_000 => 1.0m,
                < 300_000 => 0.5m,
                < 1_000_000 => 0.3m,
                _ => 0.2m
            };
            return Math.Round(finalPrice * depositPercent, 0);
        }

        private async Task<Appointment> CreateAppointmentEntityAsync(
            POSTAppointmentModelView model,
            Guid userId,
            ServiceProvider provider,
            List<Entity.AppointmentService> appointmentServices,
            decimal originalTotal,
            decimal discount,
            decimal finalPrice,
            DateTimeOffset now)
        {
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                AppointmentDate = model.AppointmentDate,
                StartTime = model.StartTime,
                SpaBranchLocationId = model.SpaBranchLocationId,
                StaffId = model.StaffId,
                Notes = model.Notes,
                CustomerId = userId,
                ProviderId = provider.ProviderId,
                PromotionId = model.PromotionId,
                PromotionAdminId = model.PromotionAdminId,
                OriginalTotalPrice = originalTotal,
                DiscountAmount = discount,
                FinalPrice = finalPrice,
                CreatedBy = CurrentUserId,
                CreatedTime = DateTime.UtcNow,
                AppointmentServices = appointmentServices,
                BookingStatus = model.PaymentMethod?.ToLower() == "cash" ? "waiting_payment" : "pending"
            };

            await _unitOfWork.GetRepository<Appointment>().InsertAsync(appointment);
            await _unitOfWork.SaveAsync();

            return appointment;
        }

        // End tạo đặt lịch

        private async Task ValidateWorkingHoursAsync(Guid branchId, DateTime date, TimeSpan startTime)
        {
            int dayOfWeek = (int)date.DayOfWeek;

            var workingHour = await _unitOfWork.GetRepository<WorkingHour>()
                .Entities
                .FirstOrDefaultAsync(x =>
                    x.SpaBranchLocationId == branchId &&
                    x.DayOfWeek == dayOfWeek &&
                    x.IsWorking &&
                    x.DeletedTime == null);

            if (workingHour == null)
                throw new ErrorException(400, ErrorCode.Failed, "Chi nhánh không hoạt động vào ngày đã chọn.");

            if (startTime < workingHour.OpeningTime || startTime >= workingHour.ClosingTime)
                throw new ErrorException(400, ErrorCode.Failed,
                    $"Chi nhánh chỉ hoạt động từ {workingHour.OpeningTime:hh\\:mm} đến {workingHour.ClosingTime:hh\\:mm}.");
        }

        public async Task<BaseResponseModel<string>> AutoCancelUnpaidAppointmentsAsync()
        {
            var now = CoreHelper.SystemTimeNow;

            var appointments = await _unitOfWork.GetRepository<Appointment>()
                .Entities.Include(x => x.Payment)
                .Include(x => x.AppointmentServices)
                .Where(x => x.BookingStatus == "waiting_payment"
                         && x.CreatedTime <= now.AddMinutes(-10).DateTime
                         && x.DeletedTime == null
                         && x.Payment != null
                         && x.Payment.PaymentMethod != "cash"
                         && x.Payment.Status == "pending")
                .ToListAsync();

            foreach (var appointment in appointments)
            {
                // ✅ XÓA VĨNH VIỄN
                _unitOfWork.GetRepository<Appointment>().HardDelete(appointment);

                if (appointment.Payment != null && appointment.Payment.Status != "refunded")
                {
                    appointment.Payment.RefundAmount = appointment.Payment.Amount;
                    appointment.Payment.PlatformFee = 0;
                    appointment.Payment.Status = "refunded";

                    _unitOfWork.GetRepository<Payment>().Update(appointment.Payment);
                }

                await ReturnPromotionsAsync(appointment);

                await _notificationService.CreateAsync(new POSTNotificationModelView
                {
                    UserId = appointment.CustomerId,
                    Title = "Lịch hẹn bị hủy",
                    Message = "Bạn chưa thanh toán cọc, lịch đã bị hủy sau 10 phút.",
                    NotificationType = "appointment"
                });
            }

            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("Đã xóa các lịch quá hạn chưa thanh toán.");
        }


        public async Task<BaseResponseModel<string>> UpdateStatusAsync(Guid appointmentId, string status)
        {
            var appointment = await _unitOfWork.GetRepository<Appointment>()
                .Entities.Include(x => x.Payment)
                .Include(x => x.AppointmentServices)
                .FirstOrDefaultAsync(x => x.Id == appointmentId && x.DeletedTime == null)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Không tìm thấy lịch.");

            if (appointment.BookingStatus == "waiting_payment")
            {
                throw new ErrorException(400, ErrorCode.Failed, "Lịch hẹn chưa được thanh toán. Không thể cập nhật trạng thái.");
            }

            var now = CoreHelper.SystemTimeNow;
            DateTime appointmentTime = appointment.AppointmentDate.Date + appointment.StartTime;
            double remainingMinutes = (appointmentTime - now).TotalMinutes;
            bool isLate = remainingMinutes < 25;

            if (status.Equals("completed", StringComparison.OrdinalIgnoreCase))
            {
                appointment.BookingStatus = "completed";

                if (appointment.Payment != null)
                {
                    if (appointment.Payment.PaymentMethod?.ToLower() == "cash" && appointment.Payment.Status == "waiting")
                    {
                        appointment.Payment.Status = "completed";
                        appointment.Payment.PaymentDate = now.UtcDateTime;
                    }

                    if (appointment.Payment.Status == "completed")
                    {
                        appointment.Payment.PlatformFee = Math.Round(appointment.FinalPrice * 0.1m, 0);
                    }
                }

                var member = await _unitOfWork.GetRepository<MemberShip>()
                    .Entities.FirstOrDefaultAsync(x => x.UserId == appointment.CustomerId);
                if (member != null)
                {
                    member.AccumulatedPoints += Convert.ToInt32(appointment.FinalPrice / 1000);
                    var ranks = await _unitOfWork.GetRepository<Rank>()
                        .Entities.Where(r => r.DeletedTime == null)
                        .OrderByDescending(r => r.MinPoints)
                        .ToListAsync();

                    var matchedRank = ranks.FirstOrDefault(r => member.AccumulatedPoints >= r.MinPoints);
                    if (matchedRank != null && matchedRank.Id != member.RankId)
                    {
                        member.RankId = matchedRank.Id;
                        member.LastRankUpdate = CoreHelper.SystemTimeNow;
                    }
                }

                await _notificationService.CreateAsync(new POSTNotificationModelView
                {
                    UserId = appointment.CustomerId,
                    Title = "Lịch đã hoàn tất",
                    Message = "Cảm ơn bạn! Spa rất vui khi được phục vụ bạn.",
                    NotificationType = "Appointment"
                });
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

                await _notificationService.CreateAsync(new POSTNotificationModelView
                {
                    UserId = appointment.CustomerId,
                    Title = "Lịch hẹn đã hủy",
                    Message = isLate
                        ? "Bạn đã hủy trễ – hệ thống đã trừ phí và hoàn lại tiền cọc."
                        : "Bạn đã hủy lịch – tiền cọc đã được hoàn lại đầy đủ.",
                    NotificationType = "appointment",
                });
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

                await _notificationService.CreateAsync(new POSTNotificationModelView
                {
                    UserId = appointment.CustomerId,
                    Title = "Bạn đã không đến",
                    Message = "Lịch hẹn của bạn đã bị hủy. Hệ thống đã hoàn lại cọc sau khi trừ phí.",
                    NotificationType = "appointment"
                });
            }
            else if (status.Equals("checked_in", StringComparison.OrdinalIgnoreCase))
            {
                appointment.BookingStatus = "checked_in";

                await _notificationService.CreateAsync(new POSTNotificationModelView
                {
                    UserId = appointment.CustomerId,
                    Title = "Check-in thành công",
                    Message = "Chúc bạn có trải nghiệm làm đẹp tuyệt vời tại Spa.",
                    NotificationType = "appointment"
                });
            }
            else if (status.Equals("confirmed", StringComparison.OrdinalIgnoreCase))
            {
                appointment.BookingStatus = "confirmed";
                appointment.IsConfirmedBySpa = true;
                appointment.ConfirmationTime = now.DateTime;

                await _notificationService.CreateAsync(new POSTNotificationModelView
                {
                    UserId = appointment.CustomerId,
                    Title = "Lịch hẹn được xác nhận",
                    Message = "Lịch hẹn của bạn đã được spa xác nhận thành công.",
                    NotificationType = "appointment"
                });
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

                await _notificationService.CreateAsync(new POSTNotificationModelView
                {
                    UserId = appointment.CustomerId,
                    Title = "Bạn đã không đến",
                    Message = "Hệ thống đã tự động hủy lịch sau 12 giờ kể từ giờ hẹn. Tiền cọc đã được hoàn lại sau khi trừ phí.",
                    NotificationType = "appointment"
                });
            }

            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("Đã tự động xử lý no_show cho các lịch quá hạn.");
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
                .Include(x => x.Staff)
                .Include(x => x.AppointmentServices).ThenInclude(s => s.Service)
                .Include(a => a.AppointmentServices)
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
                .Include(x => x.Staff)
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Không tìm thấy lịch.");

            var result = _mapper.Map<GETAppointmentModelView>(appointment);
            return BaseResponseModel<GETAppointmentModelView>.Success(result);
        }

        private async Task ReturnPromotionsAsync(Appointment appointment)
        {
            if (appointment.PromotionId.HasValue)
            {
                var promo = await _unitOfWork.GetRepository<Promotion>()
                    .GetByIdAsync(appointment.PromotionId.Value);
                if (promo != null) promo.Quantity--;
            }

            if (appointment.PromotionAdminId.HasValue)
            {
                var promoAdmin = await _unitOfWork.GetRepository<PromotionAdmin>()
                    .GetByIdAsync(appointment.PromotionAdminId.Value);
                if (promoAdmin != null) promoAdmin.Quantity--;
            }

            foreach (var aps in appointment.AppointmentServices)
            {
                var flash = await _unitOfWork.GetRepository<ServicePromotion>()
                    .Entities.FirstOrDefaultAsync(f =>
                        f.ServiceId == aps.ServiceId &&
                        f.StartDate <= CoreHelper.SystemTimeNow &&
                        f.EndDate >= CoreHelper.SystemTimeNow);
                if (flash != null) flash.Quantity--;
            }
        }
        public async Task<BaseResponseModel<List<GETAppointmentModelView>>> GetByCurrentUserAsync()
        {
            var currentUserId = Authentication.GetUserIdFromHttpContextAccessor(_context);
            var appointments = await _unitOfWork
            .GetRepository<Appointment>()
            .Entities
            .Where(a => a.CustomerId.ToString() == currentUserId)
            .Include(x => x.BranchLocation)
            .Include(x => x.Staff)
            .Include(a => a.AppointmentServices)
                .ThenInclude(s => s.Service)
                .Include(a => a.Payment)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

            var result = _mapper.Map<List<GETAppointmentModelView>>(appointments);
            for (int i = 0; i < appointments.Count; i++)
            {
                var appointment = appointments[i];
                var payment = appointment.Payment;
                result[i].DepositAmount = payment?.Status == "refunded" ? 0 : payment?.Amount;
                result[i].IsPaid = payment?.Status == "completed" ||
                    (appointment.BookingStatus == "completed" && payment?.PaymentMethod?.ToLower() == "cash");

                var hasReview = await _unitOfWork.GetRepository<Review>()
                .Entities
                .AnyAsync(r => r.AppointmentId == appointment.Id && r.DeletedTime == null);
                result[i].IsReviewed = hasReview;
            }

            return BaseResponseModel<List<GETAppointmentModelView>>.Success(result);
        }

        public async Task<BaseResponseModel<string>> CancelByUserAsync(Guid appointmentId)
        {
            var userId = CurrentUserId;
            var now = CoreHelper.SystemTimeNow;

            var appointment = await _unitOfWork.GetRepository<Appointment>()
                .Entities.Include(x => x.Payment)
                .FirstOrDefaultAsync(x => x.Id == appointmentId
                                       && x.CustomerId.ToString() == userId
                                       && x.DeletedTime == null)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Không tìm thấy lịch hẹn.");

            if (appointment.IsConfirmedBySpa)
                throw new ErrorException(400, ErrorCode.Failed, "Lịch đã được spa xác nhận, không thể hủy.");

            double minutesSinceCreated = (now - appointment.CreatedTime).TotalMinutes;
            if (minutesSinceCreated > 5)
                throw new ErrorException(400, ErrorCode.Failed, "Chỉ được hủy trong vòng 5 phút sau khi đặt.");

            appointment.BookingStatus = "canceled";
            appointment.DeletedTime = now;
            appointment.LastUpdatedTime = now;
            appointment.LastUpdatedBy = userId;

            if (appointment.Payment != null && appointment.Payment.Status != "refunded")
            {
                appointment.Payment.RefundAmount = appointment.Payment.Amount;
                appointment.Payment.PlatformFee = 0;
                appointment.Payment.Status = "refunded";
            }

            await ReturnPromotionsAsync(appointment);

            await _notificationService.CreateAsync(new POSTNotificationModelView
            {
                UserId = appointment.ProviderId,
                Title = "Người dùng đã hủy lịch hẹn",
                Message = $"Khách hàng đã hủy lịch đặt vào lúc {appointment.AppointmentDate:dd/MM/yyyy} - {appointment.StartTime}",
                NotificationType = "Appointment"
            });

            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("Hủy lịch thành công.");
        }
    }
}
