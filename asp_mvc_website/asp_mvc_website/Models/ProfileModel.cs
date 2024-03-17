namespace asp_mvc_website.Models
{
	public class ProfileModel
	{
		public UserModel user { get; set; }
		public List<ArtworkModel> artworks { get; set; }
		public List<LikeModel> like { get; set; }
	}
}
