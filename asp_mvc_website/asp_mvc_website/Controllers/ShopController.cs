using asp_mvc_website.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace asp_mvc_website.Controllers
{
    public class ShopController : Controller
    {
        private readonly HttpClient _client;

        public ShopController()
        {
            _client = new HttpClient();
            //_client.BaseAddress = new Uri("http://localhost:5012/api/");
            _client.BaseAddress = new Uri("https://apiartwork.azurewebsites.net/api/");
        }
        [HttpGet]
        public IActionResult Index()
        {
            //get list artwork
            List<ArtworkModel> artworkList = new List<ArtworkModel>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "Artwork/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                artworkList = JsonConvert.DeserializeObject<List<ArtworkModel>>(data);

                //get 12 first artwork
                artworkList = artworkList.Take(15).ToList();
            }
            return View(artworkList);
        }
        [HttpPost]
        public async Task<IActionResult> Filter(string txtSearch)
        {
            try
            {
                var fillModel = new FillModel
                {
                    txtSearch = txtSearch,
                    MinPrice = null,
                    MaxPrice = null,
                    PageNumber = 1,
                    PageSize = 5
                };
                var json = JsonConvert.SerializeObject(fillModel);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                if (content == null)
                {
                    return View("Index");
                }
                else
                {
                    var result = await _client.PostAsync(_client.BaseAddress + "Artwork/GetArtworkByFilter", content);
                    if (result.IsSuccessStatusCode)
                    {
                        var data = await result.Content.ReadAsStringAsync();
                        var artworkList = JsonConvert.DeserializeObject<List<ArtworkModel>>(data);
                        return View("Index", artworkList);
                    }
                    else
                    {
                        ViewBag.Message = "An error occurred: " + result.StatusCode;
                        return View("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "An error occurred: " + ex.Message;
                return View();
            }
        }

    }
}
