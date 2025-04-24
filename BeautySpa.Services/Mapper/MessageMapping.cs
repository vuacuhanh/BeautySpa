using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.MessageModelViews;

namespace BeautySpa.Services.Mapper
{
    public class MessageMapping : Profile
    {
        public MessageMapping()
        {
            CreateMap<Message, GETMessageModelViews>();

            CreateMap<POSTMessageModelViews, Message>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore());

            CreateMap<PUTMessageModelViews, Message>()
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedTime, opt => opt.Ignore());
        }
    }
}
