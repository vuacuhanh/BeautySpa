using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.PaymentModelViews;

namespace BeautySpa.Services.Mapper
{
    public class PaymentMapping : Profile
    {
        public PaymentMapping()
        {
            CreateMap<Payment, GETPaymentModelViews>();

            CreateMap<POSTPaymentModelViews, Payment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore());

            CreateMap<PUTPaymentModelViews, Payment>()
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore());
        }
    }
}
