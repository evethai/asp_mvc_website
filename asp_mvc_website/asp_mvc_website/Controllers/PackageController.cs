using asp_mvc_website.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace asp_mvc_website.Controllers
{
    public class PackageController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _client;
        public PackageController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:5012/api/");
            //_client.BaseAddress = new Uri("https://apiartwork.azurewebsites.net/api/");
        }
        [HttpGet]
        public IActionResult Index()
        {
            List<PackageModel> package = new List<PackageModel>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "Package").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                package = JsonConvert.DeserializeObject<List<PackageModel>>(data);
            }
            return View(package);
        }

        public IActionResult CreatePoster()
        {
            PosterModelAdd posterModelAdd = new PosterModelAdd();
            return View(posterModelAdd);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePoster(PosterModelAdd post)
        {
            try
            {
                string data = JsonConvert.SerializeObject(post);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "Poster/AddPoster", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Error();
            }
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
