namespace asp_mvc_website.Models
{
    public class NotificationUserModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
        public bool? IsRead { get; set; }
    }
    public class CreateAdminNotificationDTO
    {
        public int? NotificationId { get; set; }
        public int? ArtworkId { get; set; }
    }

}
