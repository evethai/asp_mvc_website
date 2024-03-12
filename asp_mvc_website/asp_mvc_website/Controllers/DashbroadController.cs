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
            _client.BaseAddress = new Uri("http://localhost:5012/api/");
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<OrderModel> order = new List<OrderModel>();           
            List<UserModel> user = new List<UserModel>();
            List<NotificationUserModel> noti = new List<NotificationUserModel>();
            HttpResponseMessage responseOrder = _client.GetAsync(_client.BaseAddress + "Order/GetAll").Result;
            HttpResponseMessage responseUser = _client.GetAsync(_client.BaseAddress + "User/getAllUser").Result;
            HttpResponseMessage responseNoti = _client.GetAsync(_client.BaseAddress + "Notification").Result;
            if (responseOrder.IsSuccessStatusCode && responseUser.IsSuccessStatusCode)
            {
                string dataOrder = responseOrder.Content.ReadAsStringAsync().Result;
                order = JsonConvert.DeserializeObject<List<OrderModel>>(dataOrder);

                string dataUser = responseUser.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<List<UserModel>>(dataUser);

                string dataNoti = responseUser.Content.ReadAsStringAsync().Result;
                noti = JsonConvert.DeserializeObject<List<NotificationUserModel>>(dataNoti);
                AdminModel AdminPage = new AdminModel
                {
                    orderModel = order,
                    userModels = user,
                    notificationUserModels = noti,
                };
                return View(AdminPage);
            }
            return View();
        }
    }
}
