using Amazon.Runtime;
using Amazon.S3;
using CareGuide.Core.Interfaces.Storage;
using CareGuide.Infra.Contexts;
using CareGuide.Infra.Interfaces;
using CareGuide.Infra.Interfaces.Shared;
using CareGuide.Infra.Repositories;
using CareGuide.Infra.Repositories.Shared;
using CareGuide.Infra.Storage;
using CareGuide.Infra.TransactionManagement;
using CareGuide.Models.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CareGuide.Infra
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEfTransactionUnitOfWork, EfTransactionUnitOfWork>();
            services.AddScoped<IQueryScope, PersonQueryScope>();

            services.AddSingleton<IValidateOptions<R2StorageOptions>, R2StorageOptionsValidator>();
            services.AddOptions<R2StorageOptions>()
                .Bind(configuration.GetSection(R2StorageOptions.SectionName))
                .ValidateOnStart();
            services.AddSingleton<IAmazonS3>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<R2StorageOptions>>().Value;
                return new AmazonS3Client(
                    new BasicAWSCredentials(options.AccessKey, options.SecretKey),
                    new AmazonS3Config
                    {
                        ServiceURL = options.Endpoint,
                        ForcePathStyle = true,
                        AuthenticationRegion = "auto"
                    });
            });
            services.AddSingleton<IObjectStorage, R2ObjectStorage>();
            services.AddSingleton<IValidateOptions<FileUploadOptions>, FileUploadOptionsValidator>();
            services.AddOptions<FileUploadOptions>()
                .Bind(configuration.GetSection(FileUploadOptions.SectionName))
                .ValidateOnStart();

            services.AddDbContext<DatabaseContext>(opt =>
            {
                var connectionString = configuration.GetConnectionString("DatabaseConnection");
                var environment = configuration["ASPNETCORE_ENVIRONMENT"];

                opt.UseNpgsql(connectionString);

                if (string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase))
                {
                    opt.EnableSensitiveDataLogging();
                }
            });

            RegisterRepositories(services);

            return services;
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IPersonAnnotationRepository, PersonAnnotationRepository>();
            services.AddScoped<IPersonHealthRepository, PersonHealthRepository>();
            services.AddScoped<IPhoneRepository, PhoneRepository>();
            services.AddScoped<IPersonPhoneRepository, PersonPhoneRepository>();
            services.AddScoped<IPersonDiseaseRepository, PersonDiseaseRepository>();
            services.AddScoped<IPersonFamilyHistoryRepository, PersonFamilyHistoryRepository>();
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IDoctorPhoneRepository, DoctorPhoneRepository>();
            services.AddScoped<IDoctorSpecialtyRepository, DoctorSpecialtyRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IEventAppointmentRepository, EventAppointmentRepository>();
            services.AddScoped<IEventSurgeryRepository, EventSurgeryRepository>();
            services.AddScoped<IEventMedicineRepository, EventMedicineRepository>();
            services.AddScoped<IEventExamRepository, EventExamRepository>();
            services.AddScoped<IEventFileRepository, EventFileRepository>();
            services.AddScoped<IEventAddressRepository, EventAddressRepository>();
            services.AddScoped<IFileAssetRepository, FileAssetRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IDoctorAddressRepository, DoctorAddressRepository>();
        }
    }
}
