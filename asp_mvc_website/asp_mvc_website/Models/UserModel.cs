namespace asp_mvc_website.Models
{
    public class UserModel
    {
        public string userId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }

    }
}
