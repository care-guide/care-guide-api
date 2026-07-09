namespace CareGuide.Security.Interfaces
{
    public interface IUserSessionContext
    {
        Guid UserId { get; }
        Guid PersonId { get; }
        bool IsAdmin { get; }
    }
}
