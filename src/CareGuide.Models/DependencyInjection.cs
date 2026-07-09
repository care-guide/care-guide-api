using CareGuide.Models.Mappers.Address;
using CareGuide.Models.Mappers.Doctor;
using CareGuide.Models.Mappers.DoctorPhone;
using CareGuide.Models.Mappers.DoctorSpecialty;
using CareGuide.Models.Mappers.Event;
using CareGuide.Models.Mappers.EventAddress;
using CareGuide.Models.Mappers.EventAppointment;
using CareGuide.Models.Mappers.EventExam;
using CareGuide.Models.Mappers.EventFile;
using CareGuide.Models.Mappers.EventMedicine;
using CareGuide.Models.Mappers.EventSurgery;
using CareGuide.Models.Mappers.FileAsset;
using CareGuide.Models.Mappers.Person;
using CareGuide.Models.Mappers.PersonAnnotation;
using CareGuide.Models.Mappers.PersonDisease;
using CareGuide.Models.Mappers.PersonFamilyHistory;
using CareGuide.Models.Mappers.PersonHealth;
using CareGuide.Models.Mappers.PersonPhone;
using CareGuide.Models.Mappers.Phone;
using CareGuide.Models.Mappers.User;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CareGuide.Models
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddModelMappings(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<AccountToPersonProfileMapper>();
                cfg.AddProfile<AccountToUserProfileMapper>();
                cfg.AddProfile<PersonAnnotationProfileMapper>();
                cfg.AddProfile<PersonHealthProfileMapper>();
                cfg.AddProfile<PersonProfileMapper>();
                cfg.AddProfile<UserProfileMapper>();
                cfg.AddProfile<PhoneProfileMapper>();
                cfg.AddProfile<PersonPhoneProfileMapper>();
                cfg.AddProfile<PersonDiseaseProfileMapper>();
                cfg.AddProfile<DoctorProfileMapper>();
                cfg.AddProfile<DoctorPhoneProfileMapper>();
                cfg.AddProfile<DoctorSpecialtyProfileMapper>();
                cfg.AddProfile<PersonFamilyHistoryProfileMapper>();
                cfg.AddProfile<EventProfileMapper>();
                cfg.AddProfile<EventAppointmentProfileMapper>();
                cfg.AddProfile<EventSurgeryProfileMapper>();
                cfg.AddProfile<EventMedicineProfileMapper>();
                cfg.AddProfile<EventExamProfileMapper>();
                cfg.AddProfile<EventFileProfileMapper>();
                cfg.AddProfile<EventAddressProfileMapper>();
                cfg.AddProfile<FileAssetProfileMapper>();
                cfg.AddProfile<AddressProfileMapper>();
            });

            return services;
        }

        public static IServiceCollection AddModelValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }
    }
}
