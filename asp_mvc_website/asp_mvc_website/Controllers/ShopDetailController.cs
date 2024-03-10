using asp_mvc_website.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace asp_mvc_website.Controllers
{
    public class ShopDetailController : Controller
    {
        private readonly HttpClient _client;
        public ShopDetailController()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:5012/api/");
            //_client.BaseAddress = new Uri("https://apiartwork.azurewebsites.net/api/");
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
