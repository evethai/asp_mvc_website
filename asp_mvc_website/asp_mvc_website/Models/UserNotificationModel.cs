using asp_mvc_website.Enums;

namespace asp_mvc_website.Models
{
    public class UserNotificationModel
    {
        public int Id { get; set; }
        public int? NotificationId { get; set; }
        public int? ArtworkId { get; set; }
        public string UserId { get; set; }

        public virtual NotificationModel Notification { get; set; }
        public virtual ArtworkModel Artwork { get; set; }
        public virtual ApplicationUserModel User { get; set; }

    }

    public class GetUsetNotification   
    {
        public int id {  get; set; }
        public string? artworkTitle { get; set;}
        public string? notificationTitle { get; set; }
        public  string? notificationDescription {  get; set; }
        public bool isRead {  get; set; }
        public DateTime dateTime { get; set; }
        public NotiStatus notiStatus { get; set; }
        public string? nameUser { get; set; }
        public string artwordUrl { get; set; }
        public int artworkId { get; set; }
        public int notificationId { get; set; }
    }
}
