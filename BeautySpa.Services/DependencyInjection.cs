using AutoMapper;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Services.Service;
using Microsoft.Extensions.DependencyInjection;
using BeautySpa.Repositories.UOW;
using BeautySpa.Contract.Services.Interface;


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
            services.AddScoped<IRoles, RoleService>();
            services.AddScoped<IUsers, UserService>();
            services.AddScoped<IServices, SerService>();
            services.AddScoped<IServiceCategory, ServiceCategoryService>();
            services.AddScoped<IServiceProviders, ServiceProviderSer>();
            services.AddScoped<IServiceImages, ServiceImageSer>();
            services.AddScoped<IFavoriteService, FavoriteService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IPromotionService, PromotionService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IWorkingHourService, WorkingHourService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IRankService, RankService>();
            services.AddScoped<IMemberShipService, MemberShipService>();
            services.AddScoped<IStaff, StaffService>();
            services.AddScoped<IAdminStaff, AdminStaffService>();
        }
    }
}