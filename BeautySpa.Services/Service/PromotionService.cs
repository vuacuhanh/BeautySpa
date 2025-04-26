using AutoMapper;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.PromotionModelViews;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Repositories.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using BeautySpa.Core.Utils;
using BeautySpa.Core.Infrastructure;

namespace BeautySpa.Services.Service
{
    public class PromotionService : IPromotionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        // Lấy UserId hiện tại từ HttpContext
        private string currentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        // Constructor Dependency Injection
        public PromotionService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        // Lấy danh sách promotion phân trang
        public async Task<BasePaginatedList<GETPromotionModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");
            }

            // Query promotions chưa bị xóa mềm, mới nhất trước
            IQueryable<Promotion> promotions = _unitOfWork.GetRepository<Promotion>()
                .Entities.Where(p => !p.DeletedTime.HasValue)
                .OrderByDescending(p => p.CreatedTime)
                .AsQueryable();

            // Phân trang
            var paginatedPromotions = await promotions
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new BasePaginatedList<GETPromotionModelViews>(
                _mapper.Map<List<GETPromotionModelViews>>(paginatedPromotions),
                await promotions.CountAsync(),
                pageNumber,
                pageSize
            );
        }

        // Lấy chi tiết promotion theo ID
        public async Task<GETPromotionModelViews> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid promotion ID.");
            }

            var promotion = await _unitOfWork.GetRepository<Promotion>()
                .Entities.FirstOrDefaultAsync(p => p.Id == id && !p.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Promotion not found.");

            return _mapper.Map<GETPromotionModelViews>(promotion);
        }

        // Tạo mới promotion
        public async Task<Guid> CreateAsync(POSTPromotionModelViews model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Promotion title cannot be empty.");
            }

            var promotion = _mapper.Map<Promotion>(model);
            promotion.Id = Guid.NewGuid();
            promotion.CreatedBy = currentUserId;
            promotion.CreatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Promotion>().InsertAsync(promotion);
            await _unitOfWork.SaveAsync();

            return promotion.Id;
        }

        // Cập nhật promotion
        public async Task UpdateAsync(PUTPromotionModelViews model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Promotion title cannot be empty.");
            }

            var genericRepository = _unitOfWork.GetRepository<Promotion>();

            var promotion = await genericRepository.Entities
                .FirstOrDefaultAsync(p => p.Id == model.Id && !p.DeletedTime.HasValue);

            if (promotion == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Not found Promotion with id = {model.Id}");
            }

            _mapper.Map(model, promotion);
            promotion.LastUpdatedBy = currentUserId;
            promotion.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await genericRepository.UpdateAsync(promotion);
            await genericRepository.SaveAsync();
        }

        // Xóa mềm promotion
        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid promotion ID.");
            }

            var genericRepository = _unitOfWork.GetRepository<Promotion>();

            var promotion = await genericRepository.Entities
                .FirstOrDefaultAsync(p => p.Id == id && !p.DeletedTime.HasValue);

            if (promotion == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Not found Promotion with id = {id}");
            }

            promotion.DeletedTime = CoreHelper.SystemTimeNow;
            promotion.DeletedBy = currentUserId;

            await genericRepository.UpdateAsync(promotion);
            await genericRepository.SaveAsync();
        }
    }
}
