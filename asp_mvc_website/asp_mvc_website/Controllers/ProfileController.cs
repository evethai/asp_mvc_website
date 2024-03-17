using asp_mvc_website.Models;
using asp_mvc_website.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace asp_mvc_website.Controllers
{
	public class ProfileController : Controller
	{
        private readonly ILogger<ProfileController> _logger;
        private readonly IHttpClientFactory _factory;
        private readonly HttpClient _client;
        private readonly ICurrentUserService _currentUserService;

        public ProfileController(ILogger<ProfileController> logger, IConfiguration configuration,
            IHttpClientFactory httpClientFactory, ICurrentUserService currentUserService)
        {
            _factory = httpClientFactory;
            _logger = logger;
            _client = new HttpClient();
            _currentUserService = currentUserService;
            //_client.BaseAddress = new Uri("https://localhost:7021/api/");
            //_client.BaseAddress = new Uri("https://apiartwork.azurewebsites.net/api/");
            _client = _factory.CreateClient("ServerApi");
            _client.BaseAddress = new Uri(configuration["Cron:localhost"]);
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
            List<LikeModel> likeModels = new List<LikeModel>();
            HttpResponseMessage responseLike = _client.GetAsync(_client.BaseAddress + "Like/GetAllLikeByArtworkId/" + id).Result;
            if (responseLike.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                likeModels = JsonConvert.DeserializeObject<List<LikeModel>>(data);
            }

            ProfileModel profileModel = new ProfileModel();
			profileModel.user = userModel;
			profileModel.artworks = artworkModels;
            profileModel.like = likeModels;

			return View(profileModel);
		}
	}
}
