using BeautySpa.Core.Base;
using BeautySpa.ModelViews.PromotionAdminModelView;
namespace BeautySpa.Contract.Services.Interface
{
    public interface IPromotionAdminService
    {
        Task<BaseResponseModel<List<GETPromotionAdminModelView>>> GetAllAsync();
        Task<BaseResponseModel<GETPromotionAdminModelView>> GetByIdAsync(Guid id);
        Task<BaseResponseModel<string>> CreateAsync(POSTPromotionAdminModelView model);
        Task<BaseResponseModel<string>> UpdateAsync(PUTPromotionAdminModelView model);
    }
}
