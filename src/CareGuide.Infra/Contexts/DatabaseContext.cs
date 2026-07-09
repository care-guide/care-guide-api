using CareGuide.Infra.Mappings;
using CareGuide.Models.Entities;
using CareGuide.Models.Entities.Shared;
using CareGuide.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareGuide.Infra.Contexts
{
    public class DatabaseContext : DbContext
    {
        private readonly IUserSessionContext? _userSessionContext;

        public DatabaseContext(DbContextOptions<DatabaseContext> options, IUserSessionContext? userSessionContext = null) : base(options)
        {
            _userSessionContext = userSessionContext;
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Person> Persons { get; set; } = null!;
        public DbSet<PersonAnnotation> PersonAnnotations { get; set; } = null!;
        public DbSet<PersonHealth> PersonHealths { get; set; } = null!;
        public DbSet<Phone> Phones { get; set; } = null!;
        public DbSet<PersonPhone> PersonPhones { get; set; } = null!;
        public DbSet<PersonDisease> PersonDiseases { get; set; } = null!;
        public DbSet<PersonFamilyHistory> PersonFamilyHistories { get; set; } = null!;
        public DbSet<Doctor> Doctors { get; set; } = null!;
        public DbSet<DoctorPhone> DoctorPhones { get; set; } = null!;
        public DbSet<DoctorSpecialty> DoctorSpecialties { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<EventAppointment> EventAppointments { get; set; } = null!;
        public DbSet<EventSurgery> EventSurgeries { get; set; } = null!;
        public DbSet<FileAsset> FileAssets { get; set; } = null!;
        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<DoctorAddress> DoctorAddresses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserMapping());
            modelBuilder.ApplyConfiguration(new PersonMapping());
            modelBuilder.ApplyConfiguration(new PersonAnnotationMapping());
            modelBuilder.ApplyConfiguration(new PersonHealthMapping());
            modelBuilder.ApplyConfiguration(new PersonDiseaseMapping());
            modelBuilder.ApplyConfiguration(new PhoneMapping());
            modelBuilder.ApplyConfiguration(new PersonPhoneMapping());
            modelBuilder.ApplyConfiguration(new PersonFamilyHistoryMapping());
            modelBuilder.ApplyConfiguration(new DoctorMapping());
            modelBuilder.ApplyConfiguration(new DoctorPhoneMapping());
            modelBuilder.ApplyConfiguration(new DoctorSpecialtyMapping());
            modelBuilder.ApplyConfiguration(new RefreshTokenMapping());
            modelBuilder.ApplyConfiguration(new EventMapping());
            modelBuilder.ApplyConfiguration(new EventAppointmentMapping());
            modelBuilder.ApplyConfiguration(new EventSurgeryMapping());
            modelBuilder.ApplyConfiguration(new FileAssetMapping());
            modelBuilder.ApplyConfiguration(new AddressMapping());
            modelBuilder.ApplyConfiguration(new DoctorAddressMapping());

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            ApplyPersonIdRules();
            UpdateEntityTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ApplyPersonIdRules();
            UpdateEntityTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void ApplyPersonIdRules()
        {
            if (_userSessionContext == null || _userSessionContext.IsAdmin || _userSessionContext.PersonId == Guid.Empty)
                return;

            var personId = _userSessionContext.PersonId;

            foreach (var entry in ChangeTracker.Entries<IPersonOwnedEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    if (!entry.Entity.PersonId.HasValue)
                        entry.Entity.PersonId = personId;
                }
                else if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                {
                    if (entry.Entity.PersonId.HasValue && entry.Entity.PersonId != personId)
                        throw new UnauthorizedAccessException("You are not allowed to modify entities from another person.");
                }
            }
        }

        private void UpdateEntityTimestamps()
        {
            var currentTime = DateTime.UtcNow;
            var entries = ChangeTracker.Entries().Where(e => e.Entity is IEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                var entity = (IEntity)entityEntry.Entity;
                entity.UpdatedAt = currentTime;

                if (entityEntry.State == EntityState.Added)
                {
                    entity.CreatedAt = currentTime;
                }
            }
        }
    }
}
