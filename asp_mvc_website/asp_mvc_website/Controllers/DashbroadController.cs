using asp_mvc_website.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace asp_mvc_website.Controllers
{
    public class DashbroadController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _client;
        public DashbroadController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://localhost:44357/api");
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<OrderModel> order = new List<OrderModel>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "Order/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                order = JsonConvert.DeserializeObject<List<OrderModel>>(data);
            }
            return View(order);
        }
    }
}
