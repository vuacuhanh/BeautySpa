using AutoMapper;
using BeautySpa.ModelViews.StaffModelViews;
using System.Text.Json;

public class StaffMapping : Profile
{
    public StaffMapping()
    {
        CreateMap<Staff, GETStaffModelView>()
            .ForMember(dest => dest.Permissions,
                opt => opt.ConvertUsing(new PermissionConverter(), src => src.Permissions));

        CreateMap<POSTStaffModelView, Staff>()
            .ForMember(dest => dest.Permissions,
                opt => opt.ConvertUsing(new PermissionToJsonConverter(), src => src.Permissions));

        CreateMap<PUTStaffModelView, Staff>()
            .ForMember(dest => dest.Permissions,
                opt => opt.ConvertUsing(new PermissionToJsonConverter(), src => src.Permissions));
    }

    private class PermissionConverter : IValueConverter<string?, List<string>>
    {
        public List<string> Convert(string? sourceMember, ResolutionContext context)
        {
            return string.IsNullOrEmpty(sourceMember)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(sourceMember!)!;
        }
    }

    private class PermissionToJsonConverter : IValueConverter<List<string>, string>
    {
        public string Convert(List<string> sourceMember, ResolutionContext context)
        {
            return JsonSerializer.Serialize(sourceMember);
        }
    }
}
