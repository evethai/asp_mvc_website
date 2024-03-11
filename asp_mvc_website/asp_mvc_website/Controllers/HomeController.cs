using asp_mvc_website.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace asp_mvc_website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _client;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://localhost:44357/api/");
        }

        [HttpGet]
        public IActionResult Index()
        { 
            //get list artwork
            //List<ArtworkModel> artworkList = new List<ArtworkModel>();
            //HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Artwork/GetAll").Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    string data = response.Content.ReadAsStringAsync().Result;
            //    artworkList = JsonConvert.DeserializeObject<List<ArtworkModel>>(data);
            //}
            //return View(artworkList);

            //get a artwork
            ArtworkModel artwork = new ArtworkModel();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "Artwork/GetById/1").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                artwork = JsonConvert.DeserializeObject<ArtworkModel>(data);
            }
            return View(artwork);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
