namespace CareGuide.Models.DTOs.User
{
    public record CreateUserDto(
        string Email,
        string Password
    );
}
