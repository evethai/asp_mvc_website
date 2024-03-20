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
        public async Task<IActionResult> Index(string id)
        {
            //id = "a88a4533-52da-4b30-b9c5-b259423f14b2";
            var response = await _client.GetAsync(_client.BaseAddress + "UserNotifcation/getNotiUser?userId=" + id);
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
               var  userNoti = JsonConvert.DeserializeObject<GetUserNoti>(data);
				//userNoti = userNoti.OrderByDescending(noti => noti.dateTime).ToList();

				//    HttpContext.Session.SetString("MyListSessionKey", JsonConvert.SerializeObject(userNoti));
				return View(userNoti);
			}
			return View(null);
        }

		[HttpPut]
		public async Task<IActionResult> MarkReadNoti(int notificationId)
		{
			try
			{
				var requestUri = $"Notification/MarkReadNoti?id={notificationId}";
				HttpResponseMessage response = await _client.PutAsync(requestUri, null);

				if (response.IsSuccessStatusCode)
				{
					//return RedirectToAction("Index"); // Return Ok if marking as read is successful
					return Ok();
				}
				else
				{
					return BadRequest(); // Return BadRequest if there's an error
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				_logger.LogError(ex, "An error occurred while marking notification as read.");

				// Return an error response
				return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
			}
		}


	}
}
