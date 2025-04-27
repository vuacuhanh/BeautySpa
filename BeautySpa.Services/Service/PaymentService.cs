using AutoMapper;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.PaymentModelViews;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Repositories.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using BeautySpa.Core.Utils;
using BeautySpa.Core.Infrastructure;

namespace BeautySpa.Services.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        // Lấy UserId từ HttpContext (dùng để ghi CreatedBy, LastUpdatedBy, DeletedBy)
        private string currentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        // Constructor Dependency Injection: UnitOfWork, AutoMapper, HttpContextAccessor
        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        // Lấy danh sách payment phân trang
        public async Task<BasePaginatedList<GETPaymentModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");
            }

            // Query lấy Payment chưa xóa mềm, mới nhất trước
            IQueryable<Payment> payments = _unitOfWork.GetRepository<Payment>()
                .Entities.Where(p => !p.DeletedTime.HasValue)
                .OrderByDescending(p => p.CreatedTime)
                .AsQueryable();

            // Phân trang
            var paginatedPayments = await payments
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new BasePaginatedList<GETPaymentModelViews>(
                _mapper.Map<List<GETPaymentModelViews>>(paginatedPayments),
                await payments.CountAsync(),
                pageNumber,
                pageSize
            );
        }

        // Lấy chi tiết 1 Payment theo ID
        public async Task<GETPaymentModelViews> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid payment ID.");
            }

            var payment = await _unitOfWork.GetRepository<Payment>()
                .Entities.FirstOrDefaultAsync(p => p.Id == id && !p.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Payment not found.");

            return _mapper.Map<GETPaymentModelViews>(payment);
        }

        // Tạo mới Payment
        public async Task<Guid> CreateAsync(POSTPaymentModelViews model)
        {
            if (model.Amount <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Payment amount must be greater than 0.");
            }

            var payment = _mapper.Map<Payment>(model);
            payment.Id = Guid.NewGuid();
            payment.CreatedBy = currentUserId;
            payment.CreatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Payment>().InsertAsync(payment);
            await _unitOfWork.SaveAsync();

            return payment.Id;
        }

        // Cập nhật Payment
        public async Task UpdateAsync(PUTPaymentModelViews model)
        {
            if (model.Amount <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Payment amount must be greater than 0.");
            }

            var genericRepository = _unitOfWork.GetRepository<Payment>();

            var payment = await genericRepository.Entities
                .FirstOrDefaultAsync(p => p.Id == model.Id && !p.DeletedTime.HasValue);

            if (payment == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Not found Payment with id = {model.Id}");
            }

            _mapper.Map(model, payment);
            payment.LastUpdatedBy = currentUserId;
            payment.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await genericRepository.UpdateAsync(payment);
            await genericRepository.SaveAsync();
        }

        // Xóa mềm Payment (cập nhật DeletedTime và DeletedBy)
        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid payment ID.");
            }

            var genericRepository = _unitOfWork.GetRepository<Payment>();

            var payment = await genericRepository.Entities
                .FirstOrDefaultAsync(p => p.Id == id && !p.DeletedTime.HasValue);

            if (payment == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Not found Payment with id = {id}");
            }

            payment.DeletedTime = CoreHelper.SystemTimeNow;
            payment.DeletedBy = currentUserId;

            await genericRepository.UpdateAsync(payment);
            await genericRepository.SaveAsync();
        }
    }
}
