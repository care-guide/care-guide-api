namespace CareGuide.Models.DTOs.Account
{
    public record UpdatePasswordAccountDto(
        string CurrentPassword,
        string NewPassword
    );
}
