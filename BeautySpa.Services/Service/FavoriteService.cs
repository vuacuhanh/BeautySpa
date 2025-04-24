using AutoMapper;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.FavoriteModelViews;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Repositories.Context;

namespace BeautySpa.Services.Service
{
    public class FavoriteService : IFavoriteService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public FavoriteService(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BasePaginatedList<GETFavoriteModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than 0.");

            var query = _context.Favorites
                .AsNoTracking()
                .Where(f => f.DeletedTime == null)
                .OrderByDescending(f => f.CreatedTime);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedItems = _mapper.Map<IReadOnlyCollection<GETFavoriteModelViews>>(items);
            return new BasePaginatedList<GETFavoriteModelViews>(mappedItems, totalCount, pageNumber, pageSize);
        }

        public async Task<GETFavoriteModelViews> GetByIdAsync(Guid id)
        {
            var favorite = await _context.Favorites
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id && f.DeletedTime == null);

            if (favorite == null)
                throw new Exception("Favorite not found.");

            return _mapper.Map<GETFavoriteModelViews>(favorite);
        }

        public async Task<Guid> CreateAsync(POSTFavoriteModelViews model)
        {
            if (model.CustomerId == Guid.Empty || model.ProviderId == Guid.Empty)
                throw new ArgumentException("CustomerId and ProviderId are required.");

            var exists = await _context.Favorites.AnyAsync(f =>
                f.CustomerId == model.CustomerId &&
                f.ProviderId == model.ProviderId &&
                f.DeletedTime == null);

            if (exists)
                throw new Exception("Favorite already exists.");

            var favorite = _mapper.Map<Favorite>(model);
            favorite.Id = Guid.NewGuid();
            favorite.CreatedTime = DateTimeOffset.UtcNow;

            // Tránh lỗi navigation nếu AutoMapper chưa ignore
            favorite.Customer = null;
            favorite.Provider = null;

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return favorite.Id;
        }

        public async Task UpdateAsync(PUTFavoriteModelViews model)
        {
            var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.Id == model.Id && f.DeletedTime == null);
            if (favorite == null)
                throw new Exception("Favorite not found.");

            // Không cho update navigation trực tiếp
            _mapper.Map(model, favorite);
            favorite.LastUpdatedTime = DateTimeOffset.UtcNow;
            favorite.Customer = null;
            favorite.Provider = null;

            _context.Favorites.Update(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.Id == id && f.DeletedTime == null);
            if (favorite == null)
                throw new Exception("Favorite not found.");

            favorite.DeletedTime = DateTimeOffset.UtcNow;

            _context.Favorites.Update(favorite);
            await _context.SaveChangesAsync();
        }
    }
}
