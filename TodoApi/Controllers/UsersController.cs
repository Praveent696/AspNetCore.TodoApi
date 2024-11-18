using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoApi.Models.Dto;
using TodoApi.Services.Abstrations;

namespace TodoApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ApiBaseController
    {
        private IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("register")]
        [AllowAnonymous]
        [Produces<ApiResponse<RegistrationRequestDto>>]
        public async Task<IActionResult> Registration(RegistrationRequestDto registrationRequestDto)
        {
            var result = await _userService.RegisterUser(registrationRequestDto);

            // If the result is empty, user is successfully registered
            if (string.IsNullOrEmpty(result)) 
            {
                return Ok(CreateResponse<RegistrationRequestDto>(true, registrationRequestDto, "User successfully registered!"));
            }

            // If registration fails, return a bad request response
            return BadRequest(CreateResponse<object>(false, null, "Something went worng!!"));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [Produces<ApiResponse<LoginResponseDto>>]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            var loginResponse = await _userService.Login(loginRequestDto);

            // If the user is not found or invalid credentials are provided, return an unauthorized response
            if (loginResponse.User == null)
            {
                return Unauthorized(CreateResponse<object>(false, null, "Email and password combination is incorrect!!"));
            }

            // If login is successful, return an OK response with login details
            return Ok(CreateResponse<LoginResponseDto>(true, loginResponse, "Login successful!"));
        }

        // Endpoint for assigning a role to a user (admin-only access)
        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        [Produces<ApiResponse<object>>]
        public async Task<IActionResult> AssignRole(AssignRoleRequestDto assignRoleRequestDto)
        {
            var response = await _userService.AssignRole(assignRoleRequestDto.Email, assignRoleRequestDto.RoleName);

            // If the role assignment fails, return a bad request response
            if (!response)
            {
                return BadRequest(CreateResponse<object>(false, null, "Somthing went wrong, Role not assigned!!"));
            }
            return Ok(CreateResponse<object>(true, null, $"Role name {assignRoleRequestDto.RoleName} assigned to {assignRoleRequestDto.Email} successful!"));
        }
    }
}
