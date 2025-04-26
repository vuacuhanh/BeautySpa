using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ReviewModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;

namespace BeautySpa.Services.Service
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private string currentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<BasePaginatedList<GETReviewModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");
            }

            IQueryable<Review> reviews = _unitOfWork.GetRepository<Review>()
                .Entities.Where(r => !r.DeletedTime.HasValue)
                .OrderByDescending(r => r.CreatedTime)
                .AsQueryable();

            var paginatedReviews = await reviews
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new BasePaginatedList<GETReviewModelViews>(_mapper.Map<List<GETReviewModelViews>>(paginatedReviews),
                await reviews.CountAsync(), pageNumber, pageSize);
        }

        public async Task<GETReviewModelViews> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid review ID.");
            }

            var review = await _unitOfWork.GetRepository<Review>().Entities
                .FirstOrDefaultAsync(r => r.Id == id && !r.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Review not found.");

            return _mapper.Map<GETReviewModelViews>(review);
        }

        public async Task<Guid> CreateAsync(POSTReviewModelViews model)
        {
            // Kiểm tra đầu vào
            if (model.Rating < 1 || model.Rating > 5)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Rating must be between 1 and 5.");
            }

            if (model.AppointmentId == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid appointment ID.");
            }

            if (model.CustomerId == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid customer ID.");
            }

            if (model.ProviderId == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid provider ID.");
            }

            // Kiểm tra AppointmentId có tồn tại không
            var appointmentExists = await _unitOfWork.GetRepository<Appointment>().Entities
                .FirstOrDefaultAsync(a => a.Id == model.AppointmentId && !a.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Appointment not found.");

            // Kiểm tra CustomerId có tồn tại không
            var customerExists = await _unitOfWork.GetRepository<ApplicationUsers>().Entities
                .FirstOrDefaultAsync(u => u.Id == model.CustomerId && !u.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Customer not found.");

            // Kiểm tra ProviderId có tồn tại không
            var providerExists = await _unitOfWork.GetRepository<ApplicationUsers>().Entities
                .FirstOrDefaultAsync(u => u.Id == model.ProviderId && !u.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Provider not found.");

            // Kiểm tra xem đã có review cho Appointment này chưa
            var reviewExists = await _unitOfWork.GetRepository<Review>().Entities
                .FirstOrDefaultAsync(r => r.AppointmentId == model.AppointmentId && !r.DeletedTime.HasValue);

            if (reviewExists != null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "A review already exists for this appointment.");
            }

            // Map từ model sang entity
            var review = _mapper.Map<Review>(model);
            review.Id = Guid.NewGuid();
            review.CreatedBy = currentUserId;
            review.CreatedTime = CoreHelper.SystemTimeNow;
            review.LastUpdatedBy = currentUserId;
            review.LastUpdatedTime = CoreHelper.SystemTimeNow;

            // Thêm vào DB
            await _unitOfWork.GetRepository<Review>().InsertAsync(review);
            await _unitOfWork.SaveAsync();

            return review.Id;
        }

        public async Task UpdateAsync(PUTReviewModelViews model)
        {
            if (model.Id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid review ID.");
            }

            if (model.Rating < 1 || model.Rating > 5)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Rating must be between 1 and 5.");
            }

            var genericRepository = _unitOfWork.GetRepository<Review>();

            var review = await genericRepository.Entities
                .FirstOrDefaultAsync(r => r.Id == model.Id && !r.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Review with id = {model.Id} not found.");

            _mapper.Map(model, review);
            review.LastUpdatedBy = currentUserId;
            review.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await genericRepository.UpdateAsync(review);
            await genericRepository.SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid review ID.");
            }

            var genericRepository = _unitOfWork.GetRepository<Review>();

            var review = await genericRepository.Entities
                .FirstOrDefaultAsync(r => r.Id == id && !r.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Review with id = {id} not found.");

            review.DeletedTime = CoreHelper.SystemTimeNow;
            review.DeletedBy = currentUserId;

            await genericRepository.UpdateAsync(review);
            await genericRepository.SaveAsync();
        }
    }
}