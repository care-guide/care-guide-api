using CareGuide.Core.Interfaces;
using CareGuide.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CareGuide.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IPersonAnnotationService, PersonAnnotationService>();
            services.AddScoped<IPersonHealthService, PersonHealthService>();
            services.AddScoped<IPhoneService, PhoneService>();
            services.AddScoped<IPersonPhoneService, PersonPhoneService>();
            services.AddScoped<IPersonDiseaseService, PersonDiseaseService>();
            services.AddScoped<IPersonFamilyHistoryService, PersonFamilyHistoryService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IDoctorPhoneService, DoctorPhoneService>();
            services.AddScoped<IDoctorSpecialtyService, DoctorSpecialtyService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IFileAssetService, FileAssetService>();

            return services;
        }
    }
}
