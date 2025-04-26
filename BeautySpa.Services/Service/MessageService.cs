using AutoMapper;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.MessageModelViews;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Repositories.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using BeautySpa.Core.Utils;
using BeautySpa.Core.Infrastructure;

namespace BeautySpa.Services.Service
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        private string currentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public MessageService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<BasePaginatedList<GETMessageModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");
            }

            IQueryable<Message> messages = _unitOfWork.GetRepository<Message>()
                .Entities.Where(m => !m.DeletedTime.HasValue)
                .OrderByDescending(m => m.CreatedTime)
                .AsQueryable();

            var paginatedMessages = await messages
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new BasePaginatedList<GETMessageModelViews>(
                _mapper.Map<List<GETMessageModelViews>>(paginatedMessages),
                await messages.CountAsync(),
                pageNumber,
                pageSize
            );
        }

        public async Task<GETMessageModelViews> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid message ID.");
            }

            var message = await _unitOfWork.GetRepository<Message>()
                .Entities.FirstOrDefaultAsync(m => m.Id == id && !m.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Message not found.");

            return _mapper.Map<GETMessageModelViews>(message);
        }

        public async Task<Guid> CreateAsync(POSTMessageModelViews model)
        {
            if (string.IsNullOrWhiteSpace(model.Content))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Message content cannot be empty.");
            }

            var message = _mapper.Map<Message>(model);
            message.Id = Guid.NewGuid();
            message.CreatedBy = currentUserId;
            message.CreatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Message>().InsertAsync(message);
            await _unitOfWork.SaveAsync();

            return message.Id;
        }

        public async Task UpdateAsync(PUTMessageModelViews model)
        {
            if (string.IsNullOrWhiteSpace(model.Content))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Message content cannot be empty.");
            }

            var genericRepository = _unitOfWork.GetRepository<Message>();

            var message = await genericRepository.Entities
                .FirstOrDefaultAsync(m => m.Id == model.Id && !m.DeletedTime.HasValue);

            if (message == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Not found Message with id = {model.Id}");
            }

            _mapper.Map(model, message);
            message.LastUpdatedBy = currentUserId;
            message.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await genericRepository.UpdateAsync(message);
            await genericRepository.SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid message ID.");
            }

            var genericRepository = _unitOfWork.GetRepository<Message>();

            var message = await genericRepository.Entities
                .FirstOrDefaultAsync(m => m.Id == id && !m.DeletedTime.HasValue);

            if (message == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Not found Message with id = {id}");
            }

            message.DeletedTime = CoreHelper.SystemTimeNow;
            message.DeletedBy = currentUserId;

            await genericRepository.UpdateAsync(message);
            await genericRepository.SaveAsync();
        }
    }
}
