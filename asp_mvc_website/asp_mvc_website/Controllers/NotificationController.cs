using asp_mvc_website.Models;
using asp_mvc_website.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace asp_mvc_website.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _client;
		private readonly ICurrentUserService _currentUserService;
		public NotificationController(ILogger<HomeController> logger, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _client = new HttpClient();
			_currentUserService = currentUserService;
			_client.BaseAddress = new Uri("https://localhost:44357/api/");
            //_client.BaseAddress = new Uri("https://apiartwork.azurewebsites.net/api/");
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
