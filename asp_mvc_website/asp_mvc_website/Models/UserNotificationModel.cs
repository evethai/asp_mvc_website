﻿using asp_mvc_website.Enums;

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

    //public class GetUsetNotification   
    //{
    //    public int id {  get; set; }
    //    public string? artworkTitle { get; set;}
    //    public string? notificationTitle { get; set; }
    //    public  string? notificationDescription {  get; set; }
    //    public bool isRead {  get; set; }
    //    public DateTime dateTime { get; set; }
    //    public NotiStatus notiStatus { get; set; }
    //    public string? nameUser { get; set; }
    //    public string artwordUrl { get; set; }
    //    public int artworkId { get; set; }
    //    public int notificationId { get; set; }
    //}
	public class UserVM
	{
		public String LastName { get; set; }
		public String FirstName { get; set; }
	}

	public class ArtWorkVM
	{
		public String ArtworkId { get; set; }
		public string Title { get; set; }
	}

	public class ArtWorkImageVM
	{
		public string Image { get; set; }

	}

	public class NotificationVM
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public DateTime Date { get; set; }
		public bool IsRead { get; set; }
		public NotiStatus notiStatus { get; set; }
	}



	public class GetUserNotificationDTO1
	{
		public DateTime Date { get; set; }
		public UserVM UserVM { get; set; }
		public ArtWorkVM ArtWorkVM { get; set; }
		public ArtWorkImageVM ArtWorkImageVM { get; set; }
		public NotificationVM NotificationVM { get; set; }
	}

	public class GetUserNoti
	{
		public int total { get; set; }
		public int currentPage { get; set; }
		public List<GetUserNotificationDTO1> data { get; set;}
	}
}
