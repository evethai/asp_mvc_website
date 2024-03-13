using System.ComponentModel.DataAnnotations;

namespace asp_mvc_website.DTO
{
    public class RegisterDTO
    {
        [Required]
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
