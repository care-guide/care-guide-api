using AutoMapper;
using CareGuide.Models.DTOs.Account;
using CareGuide.Models.DTOs.User;

namespace CareGuide.Models.Mappers.User
{
    public class AccountToUserProfileMapper : Profile
    {
        public AccountToUserProfileMapper()
        {
            CreateMap<CreateAccountDto, CreateUserDto>()
                .ConstructUsing(src => new CreateUserDto(
                    src.Email,
                    src.Password
                ));
        }
    }
}
