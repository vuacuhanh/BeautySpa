using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.StaffAdminModelViews;

namespace BeautySpa.Services.Mapper
{
    public class AdminStaffMapping : Profile
    {
        public AdminStaffMapping()
        {
            CreateMap<AdminStaff, GETAdminStaffModelView>()
                .ForMember(dest => dest.Permissions,
                    opt => opt.ConvertUsing<PermissionJsonToListConverter, string?>());

            CreateMap<POSTAdminStaffModelView, AdminStaff>()
                .ForMember(dest => dest.Permissions,
                    opt => opt.ConvertUsing<ListToJsonConverter, List<string>>(src => src.Permissions));

            CreateMap<PUTAdminStaffModelView, AdminStaff>()
                .ForMember(dest => dest.Permissions,
                    opt => opt.ConvertUsing<ListToJsonConverter, List<string>>(src => src.Permissions));
        }
    }

    public class PermissionJsonToListConverter : IValueConverter<string?, List<string>>
    {
        public List<string> Convert(string? sourceMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(sourceMember)) return new();
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(sourceMember) ?? new();
        }
    }

    public class ListToJsonConverter : IValueConverter<List<string>, string>
    {
        public string Convert(List<string> sourceMember, ResolutionContext context)
        {
            return System.Text.Json.JsonSerializer.Serialize(sourceMember);
        }
    }
}
