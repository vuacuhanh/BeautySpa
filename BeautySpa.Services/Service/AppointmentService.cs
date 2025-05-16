using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.AppointmentModelViews;
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

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
        }

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_context);

        public async Task<BaseResponseModel<Guid>> CreateAsync(POSTAppointmentModelView model)
        {
            await new POSTAppointmentModelViewValidator().ValidateAndThrowAsync(model);
            DateTimeOffset now = CoreHelper.SystemTimeNow;
            Guid userId = Guid.Parse(CurrentUserId);

            Entity.Service? firstService = await _unitOfWork.GetRepository<Entity.Service>()
                .Entities.Include(x => x.Provider)
                .FirstOrDefaultAsync(x => x.Id == model.Services[0].ServiceId)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Service not found.");
            Guid providerId = firstService.ProviderId;

            int dayOfWeek = (int)model.AppointmentDate.DayOfWeek;
            WorkingHour? workingHour = await _unitOfWork.GetRepository<WorkingHour>()
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

            string[] validStatuses = { "pending", "confirmed", "checked_in" };
            IQueryable<Appointment> query = _unitOfWork.GetRepository<Appointment>().Entities;

            int slotUsed = await query
                .Where(a =>
                    a.ProviderId == providerId &&
                    a.AppointmentDate == model.AppointmentDate &&
                    validStatuses.Contains(a.BookingStatus) &&
                    a.DeletedTime == null &&
                    a.StartTime < endTime &&
                    a.AppointmentServices.Any())
                .CountAsync();

            int maxSlot = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.Where(x => x.ProviderId == providerId)
                .Select(x => x.MaxAppointmentsPerSlot)
                .FirstOrDefaultAsync();

            if (slotUsed >= maxSlot)
                throw new ErrorException(400, ErrorCode.Failed, "Time slot is fully booked.");

            decimal originalTotal = 0;
            List<Entity.AppointmentService> appointmentServices = new();

            foreach (AppointmentServiceModel s in model.Services)
            {
                Entity.Service? service = await _unitOfWork.GetRepository<Entity.Service>().GetByIdAsync(s.ServiceId)
                    ?? throw new ErrorException(404, ErrorCode.NotFound, "Service not found.");
                decimal price = service.Price;

                ServicePromotion? flash = await _unitOfWork.GetRepository<ServicePromotion>().Entities
                    .FirstOrDefaultAsync(f => f.ServiceId == s.ServiceId && f.StartDate <= now && f.EndDate >= now);

                if (flash != null)
                {
                    if (flash.DiscountPercent > 0)
                        price -= price * flash.DiscountPercent.Value / 100;
                    else if (flash.DiscountAmount > 0)
                        price -= flash.DiscountAmount.Value;
                }

                appointmentServices.Add(new Entity.AppointmentService
                {
                    ServiceId = s.ServiceId,
                    Quantity = s.Quantity,
                    PriceAtBooking = price
                });

                originalTotal += price * s.Quantity;
            }

            decimal discount = 0;
            if (model.PromotionId.HasValue)
            {
                Promotion? promo = await _unitOfWork.GetRepository<Promotion>().GetByIdAsync(model.PromotionId.Value)
                    ?? throw new ErrorException(404, ErrorCode.NotFound, "Promotion not found.");
                if (promo.IsActive && now >= promo.StartDate && now <= promo.EndDate)
                {
                    discount += promo.DiscountPercent > 0
                        ? originalTotal * promo.DiscountPercent.Value / 100
                        : promo.DiscountAmount ?? 0;
                }
            }

            if (model.PromotionAdminId.HasValue)
            {
                PromotionAdmin? promoAdmin = await _unitOfWork.GetRepository<PromotionAdmin>().GetByIdAsync(model.PromotionAdminId.Value)
                    ?? throw new ErrorException(404, ErrorCode.NotFound, "PromotionAdmin not found.");

                if (promoAdmin.IsActive && now >= promoAdmin.StartDate && now <= promoAdmin.EndDate)
                {
                    Guid? rankId = await _unitOfWork.GetRepository<MemberShip>()
                        .Entities.Where(m => m.UserId == userId)
                        .Select(m => m.RankId)
                        .FirstOrDefaultAsync();

                    bool valid = promoAdmin.PromotionAdminRanks.Any(x => x.RankId == rankId);
                    if (valid)
                    {
                        discount += promoAdmin.DiscountPercent > 0
                            ? originalTotal * promoAdmin.DiscountPercent.Value / 100
                            : promoAdmin.DiscountAmount ?? 0;
                    }
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

            Appointment appointment = new()
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
            return BaseResponseModel<Guid>.Success(appointment.Id);
        }
        public async Task<BaseResponseModel<string>> UpdateAsync(PUTAppointmentModelView model)
        {
            await new PUTAppointmentModelViewValidator().ValidateAndThrowAsync(model);

            Appointment? appointment = await _unitOfWork.GetRepository<Appointment>()
                .Entities.Include(a => a.AppointmentServices)
                .FirstOrDefaultAsync(a => a.Id == model.Id && a.DeletedTime == null)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Appointment not found.");

            if (appointment.BookingStatus is "canceled" or "completed")
                throw new ErrorException(400, ErrorCode.Failed, "Cannot update canceled or completed appointments.");

            appointment.AppointmentDate = model.AppointmentDate;
            appointment.StartTime = model.StartTime;
            appointment.SpaBranchLocationId = model.SpaBranchLocationId;
            appointment.Notes = model.Notes;
            appointment.LastUpdatedBy = CurrentUserId;
            appointment.LastUpdatedTime = CoreHelper.SystemTimeNow;

            foreach (Entity.AppointmentService s in appointment.AppointmentServices.ToList())
                _unitOfWork.GetRepository<Entity.AppointmentService>().Delete1(s);

            foreach (AppointmentServiceModel s in model.Services)
            {
                Entity.Service? service = await _unitOfWork.GetRepository<Entity.Service>().GetByIdAsync(s.ServiceId)
                    ?? throw new ErrorException(404, ErrorCode.NotFound, "Service not found.");

                appointment.AppointmentServices.Add(new Entity.AppointmentService
                {
                    ServiceId = s.ServiceId,
                    Quantity = s.Quantity,
                    PriceAtBooking = service.Price
                });
            }

            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("Appointment updated.");
        }

        public async Task<BaseResponseModel<string>> UpdateStatusAsync(Guid appointmentId, string status)
        {
            Appointment? appointment = await _unitOfWork.GetRepository<Appointment>()
                .Entities.Include(x => x.Payment)
                .FirstOrDefaultAsync(x => x.Id == appointmentId && x.DeletedTime == null)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Appointment not found.");

            DateTimeOffset now = CoreHelper.SystemTimeNow;

            Dictionary<string, Func<Task>> actions = new(StringComparer.OrdinalIgnoreCase)
            {
                ["confirmed"] = async () =>
                {
                    appointment.BookingStatus = "confirmed";
                    appointment.IsConfirmedBySpa = true;
                    appointment.ConfirmationTime = now.DateTime;
                    await Task.CompletedTask;
                },
                ["checked_in"] = async () =>
                {
                    appointment.BookingStatus = "checked_in";
                    await Task.CompletedTask;
                },
                ["completed"] = async () =>
                {
                    appointment.BookingStatus = "completed";
                    if (appointment.Payment != null)
                    {
                        appointment.Payment.Status = "completed";

                        ProviderFeePolicy? policy = await _unitOfWork.GetRepository<ProviderFeePolicy>()
                            .Entities.FirstOrDefaultAsync(x => x.ProviderId == appointment.ProviderId);

                        if (policy != null)
                        {
                            decimal percent = policy.PlatformFeePercentOnCompleted;
                            appointment.Payment.PlatformFee = Math.Round(appointment.FinalPrice * percent, 0);
                        }
                    }

                    MemberShip? membership = await _unitOfWork.GetRepository<MemberShip>()
                        .Entities.FirstOrDefaultAsync(m => m.UserId == appointment.CustomerId);
                    if (membership != null)
                    {
                        membership.AccumulatedPoints += (int)(appointment.FinalPrice / 1000);
                    }

                    await Task.CompletedTask;
                },
                ["no_show"] = async () =>
                {
                    appointment.BookingStatus = "no_show";
                    if (appointment.Payment != null && appointment.Payment.Status != "refunded")
                    {
                        ProviderFeePolicy? policy = await _unitOfWork.GetRepository<ProviderFeePolicy>()
                            .Entities.FirstOrDefaultAsync(x => x.ProviderId == appointment.ProviderId);

                        decimal hold = 0;
                        if (policy != null)
                        {
                            decimal percent = policy.PlatformFeePercentOnCanceled;
                            hold = Math.Min(appointment.FinalPrice * percent, appointment.Payment.Amount);
                        }

                        appointment.Payment.RefundAmount = appointment.Payment.Amount - hold;
                        appointment.Payment.PlatformFee = hold;
                        appointment.Payment.Status = "refunded";
                    }

                    await Task.CompletedTask;
                },
                ["canceled"] = async () =>
                {
                    appointment.BookingStatus = "canceled";
                    if (appointment.Payment != null && appointment.Payment.Status != "refunded")
                    {
                        ProviderFeePolicy? policy = await _unitOfWork.GetRepository<ProviderFeePolicy>()
                            .Entities.FirstOrDefaultAsync(x => x.ProviderId == appointment.ProviderId);

                        decimal fee = 0;
                        if (policy != null)
                        {
                            decimal percent = policy.PlatformFeePercentOnCanceled;
                            fee = Math.Round(appointment.FinalPrice * percent, 0);
                        }

                        appointment.Payment.RefundAmount = appointment.Payment.Amount - fee;
                        appointment.Payment.PlatformFee = fee;
                        appointment.Payment.Status = "refunded";
                    }

                    await Task.CompletedTask;
                }
            };

            if (!actions.ContainsKey(status))
                throw new ErrorException(400, ErrorCode.Failed, "Invalid status.");

            await actions[status]();
            appointment.LastUpdatedBy = CurrentUserId;
            appointment.LastUpdatedTime = now;
            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("Status updated.");
        }
        public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
        {
            Appointment? appointment = await _unitOfWork.GetRepository<Appointment>()
                .GetByIdAsync(id)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Appointment not found.");

            appointment.DeletedBy = CurrentUserId;
            appointment.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Appointment deleted.");
        }

        public async Task<BaseResponseModel<BasePaginatedList<GETAppointmentModelView>>> GetAllAsync(int pageNumber, int pageSize)
        {
            IQueryable<Appointment> query = _unitOfWork.GetRepository<Appointment>()
                .Entities.AsNoTracking()
                .Include(x => x.AppointmentServices).ThenInclude(s => s.Service)
                .Where(x => x.DeletedTime == null)
                .OrderByDescending(x => x.CreatedTime);

            BasePaginatedList<Appointment> result = await _unitOfWork.GetRepository<Appointment>()
                .GetPagging(query, pageNumber, pageSize);

            List<GETAppointmentModelView> mapped = result.Items
                .Select(x => _mapper.Map<GETAppointmentModelView>(x)).ToList();

            BasePaginatedList<GETAppointmentModelView> paged = new(mapped, result.TotalItems, pageNumber, pageSize);
            return BaseResponseModel<BasePaginatedList<GETAppointmentModelView>>.Success(paged);
        }

        public async Task<BaseResponseModel<GETAppointmentModelView>> GetByIdAsync(Guid id)
        {
            Appointment? entity = await _unitOfWork.GetRepository<Appointment>()
                .Entities.AsNoTracking()
                .Include(x => x.AppointmentServices).ThenInclude(s => s.Service)
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Appointment not found.");

            GETAppointmentModelView result = _mapper.Map<GETAppointmentModelView>(entity);
            return BaseResponseModel<GETAppointmentModelView>.Success(result);
        }
    }
}
