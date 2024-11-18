using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models.Dto
{
    public class RegistrationRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public int Age { get; set; }
    }
}
