using asp_mvc_website.Models;
using asp_mvc_website.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Text;

namespace asp_mvc_website.Controllers
{
	public class ProfileController : Controller
	{
        private readonly IHttpClientFactory _factory;
        private readonly HttpClient _client;
        private readonly ICurrentUserService _currentUserService;

        public ProfileController(IConfiguration configuration,
            IHttpClientFactory httpClientFactory, ICurrentUserService currentUserService)
        {
            _factory = httpClientFactory;
            _client = new HttpClient();
            _currentUserService = currentUserService;
            _client = _factory.CreateClient("ServerApi");
            _client.BaseAddress = new Uri(configuration["Cron:localhost"]);
        }

        public IActionResult Index(string id)
		{
			ProfileModel userModel = new ProfileModel();
			HttpResponseMessage responseUser = _client.GetAsync(_client.BaseAddress + "User/GetUserById/" + id).Result;
			if (responseUser.IsSuccessStatusCode)
			{
				string data = responseUser.Content.ReadAsStringAsync().Result;
				userModel = JsonConvert.DeserializeObject<ProfileModel>(data);
			}
			return View(userModel);
		}
		public async Task<IActionResult> LikeArtwork(LikeModel like) 
		{
			try
			{
				var user = await _currentUserService.User();
				like.UserId = user.Id.ToString();
				string data = JsonConvert.SerializeObject(like);
				StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
				HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "/Like/CreateLike", content);

				if (response.IsSuccessStatusCode)
				{
					return RedirectToAction("Index");
				}
			}
			catch (Exception ex)
			{
				View(ex);
			}
			return View("Index");
		}
	}
}
