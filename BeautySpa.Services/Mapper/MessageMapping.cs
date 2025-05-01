using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.MessageModelViews;

namespace BeautySpa.Services.Mapper
{
    public class MessageMapping : Profile
    {
        public MessageMapping()
        {
            CreateMap<Message, GETMessageModelViews>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
                .ForMember(dest => dest.SenderType, opt => opt.MapFrom(src => src.SenderType))
                .ForMember(dest => dest.ReceiverType, opt => opt.MapFrom(src => src.ReceiverType))
                .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
                .ForMember(dest => dest.ReceiverId, opt => opt.MapFrom(src => src.ReceiverId))
                .ForMember(dest => dest.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime));
        }
    }
}
