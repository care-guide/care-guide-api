using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.Account;
using CareGuide.Models.DTOs.Person;
using CareGuide.Models.DTOs.User;
using CareGuide.Models.Entities;
using CareGuide.Security.Helpers;

namespace CareGuide.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> GetAllAsync(int page = PaginationConstants.DefaultPage, int pageSize = PaginationConstants.DefaultPageSize, CancellationToken cancellationToken = default)
        {
            var list = await _userRepository.GetAllAsync(page, pageSize, cancellationToken);
            return _mapper.Map<List<UserDto>>(list);
        }

        public async Task<UserDto> GetByIdDtoAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var userTable = await GetByIdAsync(id, cancellationToken);
            return _mapper.Map<UserDto>(userTable);
        }

        public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetAsync(id, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException();

            return user;
        }

        public async Task<UserDto> CreateAsync(PersonDto person, CreateUserDto createUser, CancellationToken cancellationToken = default)
        {
            if (await _userRepository.GetByEmailAsync(createUser.Email, cancellationToken) != null)
                throw new InvalidOperationException("Email already registered");

            var (isSecure, feedback) = PasswordManagerHelper.CheckPassword(createUser.Password);

            if (!isSecure)
                throw new InvalidOperationException(feedback);

            var user = _mapper.Map<User>(createUser);

            user.Password = PasswordManagerHelper.HashPassword(user.Password);

            await _userRepository.AddAsync(user, cancellationToken);

            return _mapper.Map<UserDto>(user);
        }

        public async Task UpdatePasswordAsync(Guid id, UpdatePasswordAccountDto user, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userRepository.GetAsync(id, cancellationToken);

            if (existingUser == null)
                throw new KeyNotFoundException();

            if (!PasswordManagerHelper.ValidatePassword(user.CurrentPassword, existingUser.Password))
                throw new UnauthorizedAccessException("Current password is incorrect.");

            var (isSecure, feedback) = PasswordManagerHelper.CheckPassword(user.NewPassword);

            if (!isSecure)
                throw new InvalidOperationException(feedback);

            if (PasswordManagerHelper.ValidatePassword(user.NewPassword, existingUser.Password))
            {
                throw new InvalidOperationException("The new password cannot be the same as the current password.");
            }

            existingUser.Password = PasswordManagerHelper.HashPassword(user.NewPassword);

            await _userRepository.UpdateAsync(existingUser, cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userRepository.GetAsync(id, cancellationToken);

            if (existingUser == null)
                throw new KeyNotFoundException();

            await _userRepository.DeleteAsync(existingUser.Id, cancellationToken);
        }
    }
}
