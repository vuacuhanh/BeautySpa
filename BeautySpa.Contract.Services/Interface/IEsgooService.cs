using BeautySpa.ModelViews.LocationModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IEsgooService
    {
        Task<ProvinceModel?> GetProvinceByIdAsync(string id);
        Task<DistrictModel?> GetDistrictByIdAsync(string id, string provinceId);
    }
}