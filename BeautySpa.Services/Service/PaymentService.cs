using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.PaymentModelViews;
using BeautySpa.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public PaymentService(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BasePaginatedList<GETPaymentModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Payments
                .AsNoTracking()
                .Where(p => p.DeletedTime == null)
                .OrderByDescending(p => p.CreatedTime);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = _mapper.Map<IReadOnlyCollection<GETPaymentModelViews>>(items);

            return new BasePaginatedList<GETPaymentModelViews>(result, totalCount, pageNumber, pageSize);
        }

        public async Task<GETPaymentModelViews> GetByIdAsync(Guid id)
        {
            var entity = await _context.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedTime == null);

            if (entity == null) throw new Exception("Payment not found");
            return _mapper.Map<GETPaymentModelViews>(entity);
        }

        public async Task<Guid> CreateAsync(POSTPaymentModelViews model)
        {
            var entity = _mapper.Map<Payment>(model);
            entity.Id = Guid.NewGuid();
            entity.CreatedTime = DateTimeOffset.UtcNow;

            _context.Payments.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task UpdateAsync(PUTPaymentModelViews model)
        {
            var entity = await _context.Payments.FirstOrDefaultAsync(p => p.Id == model.Id && p.DeletedTime == null);
            if (entity == null) throw new Exception("Payment not found");

            _mapper.Map(model, entity);
            entity.LastUpdatedTime = DateTimeOffset.UtcNow;

            _context.Payments.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Payments.FirstOrDefaultAsync(p => p.Id == id && p.DeletedTime == null);
            if (entity == null) throw new Exception("Payment not found");

            entity.DeletedTime = DateTimeOffset.UtcNow;
            _context.Payments.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
