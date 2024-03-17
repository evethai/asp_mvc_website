using asp_mvc_website.Models;
using asp_mvc_website.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace asp_mvc_website.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly IHttpClientFactory _factory;
        private readonly HttpClient _client;
        private readonly ICurrentUserService _currentUserService;

        public NotificationController(ILogger<NotificationController> logger, IConfiguration configuration,
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
            id = "a88a4533-52da-4b30-b9c5-b259423f14b2";
            List<GetUsetNotification> userNoti = new List<GetUsetNotification>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "UserNotifcation/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                userNoti = JsonConvert.DeserializeObject<List<GetUsetNotification>>(data);


                //    HttpContext.Session.SetString("MyListSessionKey", JsonConvert.SerializeObject(userNoti));
            }
            return View(userNoti);
        }


        [HttpPost]
        public async Task<IActionResult> MarkReadNoti(int notificationId)
        {
            try
            {
                HttpResponseMessage response = await _client.PutAsync($"MarkReadNoti/{notificationId}", null);
                if (response.IsSuccessStatusCode)
                {
                    return Ok(); // Trả về Ok nếu đánh dấu thành công
                }
                else
                {
                    return BadRequest(); // Trả về BadRequest nếu có lỗi xảy ra
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
