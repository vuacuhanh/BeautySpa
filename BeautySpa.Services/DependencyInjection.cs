using AutoMapper;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Services.Service;
using Microsoft.Extensions.DependencyInjection;
using BeautySpa.Repositories.UOW;


namespace BeautySpa.Services
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddServices();
            services.AddRepositories();
            services.AddHttpContextAccessor();
            services.AddMapper();

        }
        public static void AddMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}
