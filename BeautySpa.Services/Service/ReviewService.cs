using AutoMapper;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ReviewModelViews;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Repositories.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using BeautySpa.Core.Utils;
using BeautySpa.Core.Infrastructure;

namespace BeautySpa.Services.Service
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        // Lấy userId hiện tại từ HttpContext (thường dùng để lưu CreatedBy, UpdatedBy, DeletedBy)
        private string currentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        // Constructor tiêm các dependency cần thiết: UnitOfWork, AutoMapper, HttpContextAccessor
        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        // Lấy danh sách review phân trang
        public async Task<BasePaginatedList<GETReviewModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            // Validate tham số phân trang
            if (pageNumber <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");
            }

            // Query lấy các review chưa bị xóa (DeletedTime == null) và sắp xếp mới nhất trước
            IQueryable<Review> reviews = _unitOfWork.GetRepository<Review>()
                .Entities.Where(r => !r.DeletedTime.HasValue)
                .OrderByDescending(r => r.CreatedTime)
                .AsQueryable();

            // Thực hiện phân trang
            var paginatedReviews = await reviews
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Trả về danh sách phân trang, đã map sang model view
            return new BasePaginatedList<GETReviewModelViews>(
                _mapper.Map<List<GETReviewModelViews>>(paginatedReviews),
                await reviews.CountAsync(),
                pageNumber,
                pageSize
            );
        }

        // Lấy chi tiết 1 review theo id
        public async Task<GETReviewModelViews> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid review ID.");
            }

            var review = await _unitOfWork.GetRepository<Review>()
                .Entities.FirstOrDefaultAsync(r => r.Id == id && !r.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Review not found.");

            return _mapper.Map<GETReviewModelViews>(review);
        }

        // Tạo mới review
        public async Task<BaseResponseModel<Guid>> CreateAsync(POSTReviewModelViews model)
        {
            if (model.AppointmentId == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "AppointmentId không hợp lệ.");

            if (string.IsNullOrWhiteSpace(model.Comment))
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Nội dung đánh giá không được để trống.");

            var appointment = await _unitOfWork.GetRepository<Appointment>()
                .Entities
                .FirstOrDefaultAsync(a => a.Id == model.AppointmentId && !a.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Không tìm thấy lịch hẹn.");

            // Chỉ cho phép đánh giá nếu đã hoàn thành
            if (!string.Equals(appointment.BookingStatus, "completed", StringComparison.OrdinalIgnoreCase))
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.Failed, "Chỉ được đánh giá lịch đã hoàn tất.");

            // Kiểm tra trùng đánh giá
            var existedReview = await _unitOfWork.GetRepository<Review>()
                .Entities.AnyAsync(r => r.AppointmentId == model.AppointmentId && !r.DeletedTime.HasValue);
            if (existedReview)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.Failed, "Bạn đã đánh giá lịch này rồi.");

            var review = new Review
            {
                Id = Guid.NewGuid(),
                AppointmentId = model.AppointmentId,
                Rating = model.Rating,
                Comment = model.Comment,
                CustomerId = appointment.CustomerId,
                ProviderId = appointment.ProviderId,
                CreatedBy = appointment.CustomerId.ToString(),
                CreatedTime = CoreHelper.SystemTimeNow
            };

            await _unitOfWork.GetRepository<Review>().InsertAsync(review);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<Guid>.Success(review.Id, "Đánh giá thành công.");
        }


        // Cập nhật review
        public async Task UpdateAsync(PUTReviewModelViews model)
        {
            // Validate nội dung
            if (string.IsNullOrWhiteSpace(model.Comment))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Review content cannot be empty.");
            }

            var genericRepository = _unitOfWork.GetRepository<Review>();

            // Tìm review chưa bị xóa
            var review = await genericRepository.Entities
                .FirstOrDefaultAsync(r => r.Id == model.Id && !r.DeletedTime.HasValue);

            if (review == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Not found Review with id = {model.Id}");
            }

            // Map các giá trị mới từ model vào entity
            _mapper.Map(model, review);
            review.LastUpdatedBy = currentUserId;
            review.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await genericRepository.UpdateAsync(review);
            await genericRepository.SaveAsync();
        }

        // Xóa mềm review (set DeletedTime, DeletedBy)
        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid review ID.");
            }

            var genericRepository = _unitOfWork.GetRepository<Review>();

            var review = await genericRepository.Entities
                .FirstOrDefaultAsync(r => r.Id == id && !r.DeletedTime.HasValue);

            if (review == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Not found Review with id = {id}");
            }

            review.DeletedTime = CoreHelper.SystemTimeNow;
            review.DeletedBy = currentUserId;

            await genericRepository.UpdateAsync(review);
            await genericRepository.SaveAsync();
        }
    }
}