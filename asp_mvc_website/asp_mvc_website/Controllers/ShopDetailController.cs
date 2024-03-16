using asp_mvc_website.Models;
using asp_mvc_website.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace asp_mvc_website.Controllers
{
    public class ShopDetailController : Controller
    {
        private readonly ILogger<ShopDetailController> _logger;
        private readonly IHttpClientFactory _factory;
        private readonly HttpClient _client;
        private readonly ICurrentUserService _currentUserService;

        public ShopDetailController(ILogger<ShopDetailController> logger, IConfiguration configuration,
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
        [HttpGet]
        public IActionResult Index(int id)
        {
            ArtworkModel artworkModel = new ArtworkModel();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "Artwork/GetById/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                artworkModel = JsonConvert.DeserializeObject<ArtworkModel>(data);
            }

            UserModel userModel = new UserModel();
            HttpResponseMessage responseUser = _client.GetAsync(_client.BaseAddress + "User/GetUserById/" + artworkModel.UserId).Result;
            if (responseUser.IsSuccessStatusCode)
            {
                string data = responseUser.Content.ReadAsStringAsync().Result;
                userModel = JsonConvert.DeserializeObject<UserModel>(data);
            }




            ShopDetailModel shopDetailModel = new ShopDetailModel();
            shopDetailModel.artworkModel = artworkModel;
            shopDetailModel.userModel = userModel;
            return View(shopDetailModel);
        }
    }
}
