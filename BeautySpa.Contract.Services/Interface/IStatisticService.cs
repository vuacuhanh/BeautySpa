using BeautySpa.Core.Base;
using BeautySpa.ModelViews.StatisticModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IStatisticService
    {
        Task<BaseResponseModel<StatisticResultModelView>> GetAdminStatisticsAsync(StatisticFilterModelView filter);
        Task<BaseResponseModel<StatisticResultModelView>> GetProviderStatisticsByTokenAsync(StatisticFilterModelView filter);
    }
}
