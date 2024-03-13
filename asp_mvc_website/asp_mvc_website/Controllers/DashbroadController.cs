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
            HttpResponseMessage responseOrder = _client.GetAsync(_client.BaseAddress + "Order/GetAll").Result;

            List<UserModel> user = new List<UserModel>();
            HttpResponseMessage responseUser = _client.GetAsync(_client.BaseAddress + "User/getAllUser").Result;

            List<NotificationUserModel> noti = new List<NotificationUserModel>();
            HttpResponseMessage responseNoti = _client.GetAsync(_client.BaseAddress + "Notification").Result;

            List<ArtworkModel> artworkStatus1 = new List<ArtworkModel>();
            HttpResponseMessage responseStatus1 = _client.GetAsync(_client.BaseAddress + "Artwork/GetAllArtworkByStatus/1").Result;

            List<ArtworkModel> artworkStatus2 = new List<ArtworkModel>();
            HttpResponseMessage responseStatus2 = _client.GetAsync(_client.BaseAddress + "Artwork/GetAllArtworkByStatus/2").Result;

            List<ArtworkModel> artworkStatus3 = new List<ArtworkModel>();
            HttpResponseMessage responseStatus3 = _client.GetAsync(_client.BaseAddress + "Artwork/GetAllArtworkByStatus/3").Result;

            List<PosterModel> post = new List<PosterModel>();
            HttpResponseMessage responsePost = _client.GetAsync(_client.BaseAddress + "Poster").Result;

            if (responseOrder.IsSuccessStatusCode && responseUser.IsSuccessStatusCode && responseNoti.IsSuccessStatusCode
                && responseStatus1.IsSuccessStatusCode && responseStatus2.IsSuccessStatusCode && responseStatus3.IsSuccessStatusCode
                && responsePost.IsSuccessStatusCode
                )
            {
                string dataOrder = responseOrder.Content.ReadAsStringAsync().Result;
                order = JsonConvert.DeserializeObject<List<OrderModel>>(dataOrder);

                string dataUser = responseUser.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<List<UserModel>>(dataUser);

                string dataNoti = responseNoti.Content.ReadAsStringAsync().Result;
                noti = JsonConvert.DeserializeObject<List<NotificationUserModel>>(dataNoti);

                string dataChartStatus1 = responseStatus1.Content.ReadAsStringAsync().Result;
                artworkStatus1 = JsonConvert.DeserializeObject<List<ArtworkModel>>(dataChartStatus1);

                string dataChartStatus2 = responseStatus2.Content.ReadAsStringAsync().Result;
                artworkStatus2 = JsonConvert.DeserializeObject<List<ArtworkModel>>(dataChartStatus2);

                string dataChartStatus3 = responseStatus3.Content.ReadAsStringAsync().Result;
                artworkStatus3 = JsonConvert.DeserializeObject<List<ArtworkModel>>(dataChartStatus3);

                string dataPost = responsePost.Content.ReadAsStringAsync().Result;
                post = JsonConvert.DeserializeObject<List<PosterModel>>(dataPost);

                AdminModel AdminPage = new AdminModel
                {
                    orderModel = order,
                    userModels = user,
                    notificationUserModels = noti,
                    artModelStatus1 = artworkStatus1,
                    artModelStatus2 = artworkStatus2,
                    artModelStatus3 = artworkStatus3,
                    posterModels = post
                };
                return View(AdminPage);
            }
            return View();
        }
    }
}
