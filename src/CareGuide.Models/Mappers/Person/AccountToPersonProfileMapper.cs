using AutoMapper;
using CareGuide.Models.DTOs.Account;
using CareGuide.Models.DTOs.Person;

namespace CareGuide.Models.Mappers.Person
{
    public class AccountToPersonProfileMapper : Profile
    {
        public AccountToPersonProfileMapper()
        {
            CreateMap<CreateAccountDto, CreatePersonDto>()
                .ConstructUsing(src => new CreatePersonDto(
                    src.Name,
                    src.Gender,
                    src.Birthday,
                    null
                ));
        }
    }
}
