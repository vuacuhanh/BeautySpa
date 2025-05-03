using BeautySpa.Core.Base;
using BeautySpa.ModelViews.LocationModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface ILocationService
    {
        // Branch
        Task<BaseResponseModel<List<GETBranchLocationModelView>>> GetAllBranchesAsync();
        Task<BaseResponseModel<GETBranchLocationModelView>> GetBranchByIdAsync(Guid id);
        Task<BaseResponseModel<Guid>> CreateBranchAsync(POSTBranchLocationModelView model);
        Task<BaseResponseModel<string>> UpdateBranchAsync(PUTBranchLocationModelView model);
        Task<BaseResponseModel<string>> DeleteBranchAsync(Guid id);

        // Location
        Task<BaseResponseModel<List<GETLocationSpaModelView>>> GetAllLocationsAsync();
        Task<BaseResponseModel<GETLocationSpaModelView>> GetLocationByIdAsync(Guid id);
        Task<BaseResponseModel<Guid>> CreateLocationAsync(POSTLocationSpaModelView model);
        Task<BaseResponseModel<string>> UpdateLocationAsync(PUTLocationSpaModelView model);
        Task<BaseResponseModel<string>> DeleteLocationAsync(Guid id);

        // Theo Branch
        Task<BaseResponseModel<List<GETLocationSpaModelView>>> GetLocationsByBranchIdAsync(Guid branchId);
    }
}
