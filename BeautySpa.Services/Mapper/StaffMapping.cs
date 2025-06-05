using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.StaffModelViews;
using System.Text.Json;

public class StaffMapping : Profile
{
    public StaffMapping()
    {
        CreateMap<Staff, GETStaffModelView>()
            .ForMember(dest => dest.ServiceCategoryIds,
                opt => opt.MapFrom(src => src.StaffServiceCategories.Select(x => x.ServiceCategoryId)))
            .ForMember(dest => dest.ServiceCategoryNames,
                opt => opt.MapFrom(src => src.StaffServiceCategories.Select(x => x.ServiceCategory.CategoryName)));

        CreateMap<POSTStaffModelView, Staff>()
            .ForMember(dest => dest.StaffServiceCategories, opt => opt.Ignore());
        CreateMap<PUTStaffModelView, Staff>()
            .ForMember(dest => dest.StaffServiceCategories, opt => opt.Ignore());
    }
}
