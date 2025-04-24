using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.PromotionModelViews;
using BeautySpa.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class PromotionService : IPromotionService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public PromotionService(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BasePaginatedList<GETPromotionModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Promotions
                .AsNoTracking()
                .Where(p => p.DeletedTime == null)
                .OrderByDescending(p => p.CreatedTime);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = _mapper.Map<IReadOnlyCollection<GETPromotionModelViews>>(items);

            return new BasePaginatedList<GETPromotionModelViews>(result, totalCount, pageNumber, pageSize);
        }

        public async Task<GETPromotionModelViews> GetByIdAsync(Guid id)
        {
            var entity = await _context.Promotions
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedTime == null);

            if (entity == null) throw new Exception("Promotion not found");
            return _mapper.Map<GETPromotionModelViews>(entity);
        }

        public async Task<Guid> CreateAsync(POSTPromotionModelViews model)
        {
            var entity = _mapper.Map<Promotion>(model);
            entity.Id = Guid.NewGuid();
            entity.CreatedTime = DateTimeOffset.UtcNow;

            _context.Promotions.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task UpdateAsync(PUTPromotionModelViews model)
        {
            var entity = await _context.Promotions.FirstOrDefaultAsync(p => p.Id == model.Id && p.DeletedTime == null);
            if (entity == null) throw new Exception("Promotion not found");

            _mapper.Map(model, entity);
            entity.LastUpdatedTime = DateTimeOffset.UtcNow;

            _context.Promotions.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Promotions.FirstOrDefaultAsync(p => p.Id == id && p.DeletedTime == null);
            if (entity == null) throw new Exception("Promotion not found");

            entity.DeletedTime = DateTimeOffset.UtcNow;
            _context.Promotions.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
