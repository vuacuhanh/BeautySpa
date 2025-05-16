// Service (sửa hoàn chỉnh dùng cấu hình từ appsettings + chuẩn base/exception)
using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Settings;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.LocationModelViews;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace BeautySpa.Services.Service
{
    public class SpaBranchLocationService : ISpaBranchLocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GoogleMapSettings _mapSettings;

        public SpaBranchLocationService(IUnitOfWork unitOfWork, IMapper mapper, IHttpClientFactory httpClientFactory, IOptions<GoogleMapSettings> mapSettings)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
            _mapSettings = mapSettings.Value;
        }

        private async Task<(double lat, double lng)> GeocodeAddressAsync(string address)
        {
            var http = _httpClientFactory.CreateClient();
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_mapSettings.ApiKey}";
            var response = await http.GetFromJsonAsync<dynamic>(url);

            if (response?.results?[0]?.geometry?.location != null)
            {
                return ((double)response.results[0].geometry.location.lat,
                        (double)response.results[0].geometry.location.lng);
            }

            throw new ErrorException(400, ErrorCode.Failed, "Unable to geocode address.");
        }

        public async Task<BaseResponseModel<Guid>> CreateAsync(POSTSpaBranchLocationModelView model)
        {
            var entity = _mapper.Map<SpaBranchLocation>(model);
            var fullAddress = $"{model.Street}, {model.District}, {model.City}, {model.Country}";
            (entity.Latitude, entity.Longitude) = await GeocodeAddressAsync(fullAddress);

            await _unitOfWork.GetRepository<SpaBranchLocation>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();
            return BaseResponseModel<Guid>.Success(entity.Id);
        }

        public async Task<BaseResponseModel<string>> UpdateAsync(PUTSpaBranchLocationModelView model)
        {
            var entity = await _unitOfWork.GetRepository<SpaBranchLocation>().GetByIdAsync(model.Id)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Branch not found");

            _mapper.Map(model, entity);
            var fullAddress = $"{model.Street}, {model.District}, {model.City}, {model.Country}";
            (entity.Latitude, entity.Longitude) = await GeocodeAddressAsync(fullAddress);

            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("Updated");
        }

        public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.GetRepository<SpaBranchLocation>().GetByIdAsync(id)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Branch not found");

            entity.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("Deleted");
        }

        public async Task<BaseResponseModel<GETSpaBranchLocationModelView>> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.GetRepository<SpaBranchLocation>().GetByIdAsync(id)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Branch not found");

            var result = _mapper.Map<GETSpaBranchLocationModelView>(entity);
            return BaseResponseModel<GETSpaBranchLocationModelView>.Success(result);
        }

        public async Task<BaseResponseModel<List<GETSpaBranchLocationModelView>>> GetByProviderAsync(Guid providerId)
        {
            IQueryable<SpaBranchLocation> query = _unitOfWork.GetRepository<SpaBranchLocation>()
                .Entities.AsNoTracking()
                .Where(x => x.ServiceProviderId == providerId && x.DeletedTime == null);

            var raw = await query.ToListAsync();
            var result = _mapper.Map<List<GETSpaBranchLocationModelView>>(raw);
            return BaseResponseModel<List<GETSpaBranchLocationModelView>>.Success(result);
        }
    }
}