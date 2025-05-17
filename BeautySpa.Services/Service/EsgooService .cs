using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Settings;
using BeautySpa.ModelViews.LocationModelViews;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace BeautySpa.Services.Service
{
    public class EsgooService : IEsgooService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly EsgooSettings _settings;

        public EsgooService(IHttpClientFactory factory, IOptions<EsgooSettings> settings)
        {
            _httpClientFactory = factory;
            _settings = settings.Value;
        }

        public async Task<ProvinceModel?> GetProvinceByIdAsync(string id)
        {
            var http = _httpClientFactory.CreateClient();
            var url = $"{_settings.BaseUrl}/1/0.htm";

            EsgooProvinceResponse? response = await http.GetFromJsonAsync<EsgooProvinceResponse>(url);
            return response?.data.FirstOrDefault(p => p.id == id);
        }

        public async Task<DistrictModel?> GetDistrictByIdAsync(string id, string provinceId)
        {
            var http = _httpClientFactory.CreateClient();
            var url = $"{_settings.BaseUrl}/2/{provinceId}.htm";

            EsgooDistrictResponse? response = await http.GetFromJsonAsync<EsgooDistrictResponse>(url);
            return response?.data.FirstOrDefault(d => d.id == id);
        }
    }
}
