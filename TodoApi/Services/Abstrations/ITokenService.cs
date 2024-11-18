using TodoApi.Models;

namespace TodoApi.Services.Abstrations
{
    public interface ITokenService
    {
        public string GenerateToken(ApplicationUser applicationUser, List<string> roles);
    }
}
