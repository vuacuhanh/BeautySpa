using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.PaymentModelViews;

namespace BeautySpa.Services.Mapper
{
    public class PaymentMapping : Profile
    {
        public PaymentMapping()
        {
            CreateMap<Payment, GETPaymentModelView>();
            CreateMap<POSTPaymentModelView, Payment>();
        }
    }
}
