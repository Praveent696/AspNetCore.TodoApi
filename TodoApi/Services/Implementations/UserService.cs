using Microsoft.AspNetCore.Identity;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Models.Dto;
using TodoApi.Services.Abstrations;

namespace TodoApi.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _db;

        public UserService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user != null) 
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
                var result = await _userManager.AddToRoleAsync(user, roleName);
                return result.Succeeded;
            }
            return false;
        }

        public async Task<bool> IsAdmin(string id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            return user != null && await _userManager.IsInRoleAsync(user, "Admin");
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            try
            {
                var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == loginRequestDto.Email.ToLower());
                var roles = await _userManager.GetRolesAsync(user);
                var isValidUser = user != null && await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                if (isValidUser)
                {
                    UserDto userDto = new()
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Gender = user.Gender,
                        Age = user.Age,
                        PhoneNumber = user.PhoneNumber
                    };
                    return new LoginResponseDto()
                    {
                        User = userDto,
                        Token = _tokenService.GenerateToken(user, roles.ToList())
                    };
                }
                else
                {
                    return new LoginResponseDto()
                    {
                        User = null,
                        Token = null
                    };
                }
            }
            catch (Exception ex)
            {
                return new LoginResponseDto()
                {
                    User = null,
                    Token = null
                };
            }
        }

        public async Task<string> RegisterUser(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                FirstName = registrationRequestDto.FirstName,
                LastName = registrationRequestDto.LastName,
                Gender = registrationRequestDto.Gender,
                PhoneNumber = registrationRequestDto.PhoneNumber,
                Age = registrationRequestDto.Age
            };
            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    // add default role to user
                    await AssignRole(registrationRequestDto.Email, "Default");
                    return "";
                }
                else
                {
                    return string.Join(',', result.Errors.Select(x => x.Description).ToList());
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
