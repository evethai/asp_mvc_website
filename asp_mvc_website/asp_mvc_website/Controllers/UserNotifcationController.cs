using asp_mvc_website.Models;
using asp_mvc_website.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace asp_mvc_website.Controllers
{
    public class UserNotifcationController : Controller
    {
        private readonly ILogger<UserNotifcationController> _logger;
        private readonly IHttpClientFactory _factory;
        private readonly HttpClient _client;
        private readonly ICurrentUserService _currentUserService;

        public UserNotifcationController(ILogger<UserNotifcationController> logger, IConfiguration configuration,
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
        public string GetNotify(string id)
        {
            List<GetUsetNotification> userNoti = new List<GetUsetNotification>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "UserNotifcation/"+ id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                userNoti = JsonConvert.DeserializeObject<List<GetUsetNotification>>(data);
                return data;

                //    HttpContext.Session.SetString("MyListSessionKey", JsonConvert.SerializeObject(userNoti));
            }
            else return null;
        }

    }
}
