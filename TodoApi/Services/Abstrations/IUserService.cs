using TodoApi.Models.Dto;

namespace TodoApi.Services.Abstrations
{
    public interface IUserService
    {
        public Task<string> RegisterUser(RegistrationRequestDto registrationRequestDto);

        public Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        public Task<bool> AssignRole(string email, string roleName);

        public Task<bool> IsAdmin(string id);
    }
}
