using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ReviewModelViews;
using BeautySpa.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class ReviewService : IReviewService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public ReviewService(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BasePaginatedList<GETReviewModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Reviews
                .AsNoTracking()
                .Where(r => r.DeletedTime == null)
                .OrderByDescending(r => r.CreatedTime);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = _mapper.Map<IReadOnlyCollection<GETReviewModelViews>>(items);

            return new BasePaginatedList<GETReviewModelViews>(result, totalCount, pageNumber, pageSize);
        }

        public async Task<GETReviewModelViews> GetByIdAsync(Guid id)
        {
            var entity = await _context.Reviews
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id && r.DeletedTime == null);

            if (entity == null) throw new Exception("Review not found");
            return _mapper.Map<GETReviewModelViews>(entity);
        }

        public async Task<Guid> CreateAsync(POSTReviewModelViews model)
        {
            var entity = _mapper.Map<Review>(model);
            entity.Id = Guid.NewGuid();
            entity.CreatedTime = DateTimeOffset.UtcNow;

            _context.Reviews.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task UpdateAsync(PUTReviewModelViews model)
        {
            var entity = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == model.Id && r.DeletedTime == null);
            if (entity == null) throw new Exception("Review not found");

            _mapper.Map(model, entity);
            entity.LastUpdatedTime = DateTimeOffset.UtcNow;

            _context.Reviews.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id && r.DeletedTime == null);
            if (entity == null) throw new Exception("Review not found");

            entity.DeletedTime = DateTimeOffset.UtcNow;
            _context.Reviews.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
