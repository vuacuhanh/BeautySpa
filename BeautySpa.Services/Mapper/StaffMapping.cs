using AutoMapper;
using BeautySpa.ModelViews.StaffModelViews;
using System.Text.Json;

public class StaffMapping : Profile
{
    public StaffMapping()
    {
        CreateMap<POSTStaffModelView, Staff>()
            .ForMember(dest => dest.StaffServiceCategories, opt => opt.Ignore());

        CreateMap<PUTStaffModelView, Staff>()
            .ForMember(dest => dest.StaffServiceCategories, opt => opt.Ignore());

        CreateMap<Staff, GETStaffModelView>()
            .ForMember(dest => dest.ServiceCategoryIds,opt => opt.MapFrom(s => s.StaffServiceCategories.Select(x => x.ServiceCategoryId)));
    }
}
