using CareGuide.Models.Entities.Shared;
using CareGuide.Models.Enums;

namespace CareGuide.Models.Entities
{
    public class User : Entity, IPersonOwnedEntity
    {
        public required Guid? PersonId { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public UserRole Role { get; set; } = UserRole.User;

        public Person Person { get; set; } = null!;
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
