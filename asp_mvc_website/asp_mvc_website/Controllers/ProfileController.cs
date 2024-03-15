using asp_mvc_website.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace asp_mvc_website.Controllers
{
	public class ProfileController : Controller
	{
		private readonly HttpClient _client;
		public ProfileController()
		{
			_client = new HttpClient();
			//_client.BaseAddress = new Uri("http://localhost:5012/api/");
			_client.BaseAddress = new Uri("https://apiartwork.azurewebsites.net/api/");
		}
		public IActionResult Index(string id)
		{
			UserModel userModel = new UserModel();
			HttpResponseMessage responseUser = _client.GetAsync(_client.BaseAddress + "User/GetUserById/" + id).Result;
			if (responseUser.IsSuccessStatusCode)
			{
				string data = responseUser.Content.ReadAsStringAsync().Result;
				userModel = JsonConvert.DeserializeObject<UserModel>(data);
			}

			List<ArtworkModel> artworkModels = new List<ArtworkModel>();
			HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "Artwork/GetAllArtworkByUserID/" + id).Result;
			if (response.IsSuccessStatusCode)
			{
				string data = response.Content.ReadAsStringAsync().Result;
				artworkModels = JsonConvert.DeserializeObject<List<ArtworkModel>>(data);
			}

			ProfileModel profileModel = new ProfileModel();
			profileModel.user = userModel;
			profileModel.artworks = artworkModels;

			return View(profileModel);
		}
	}
}
