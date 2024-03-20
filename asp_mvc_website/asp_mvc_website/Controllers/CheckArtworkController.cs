using asp_mvc_website.Enums;
using asp_mvc_website.Models;
using asp_mvc_website.Services;
using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace asp_mvc_website.Controllers
{
    public class CheckArtworkController : Controller
    {
		private readonly IHttpClientFactory _factory;
		private readonly HttpClient _client;
		private readonly ICurrentUserService _currentUserService;

		public CheckArtworkController(IConfiguration configuration,
			IHttpClientFactory httpClientFactory, ICurrentUserService currentUserService)
		{
			_factory = httpClientFactory;
			_client = new HttpClient();
			_currentUserService = currentUserService;
			_client = _factory.CreateClient("ServerApi");
			_client.BaseAddress = new Uri(configuration["Cron:localhost"]);
		}
		[HttpGet]
		public async Task<IActionResult> Index()
        {
			var user = await _currentUserService.User();
			string userId = user.Id.ToString();
			Result result = new Result();
			DefaultSearch search = new DefaultSearch
			{
				perPage = 10,
				currentPage = 0,
			};

			try
			{
				string searchParams = $"?userId={userId}&perPage={search.perPage}&currentPage={search.currentPage}&sortBy={search.sortBy}&isAscending={search.isAscending}";
				UriBuilder builder = new UriBuilder(_client.BaseAddress);
				builder.Path = $"api/UserNotifcation/getNotiUser";
				builder.Query =  searchParams;



				HttpResponseMessage response = await _client.GetAsync(builder.Uri);
				if (response.IsSuccessStatusCode)
				{
					var data = response.Content.ReadAsStringAsync().Result;
					result = JsonConvert.DeserializeObject<Result>(data);
				}
				else
				{
					return BadRequest("Get notification failed");
				}
			}catch(HttpRequestException ex)
			{
				return BadRequest("Get notification failed" +ex);
			}
			

            return View(result);
        }




		[HttpPost]
		public async Task<IActionResult> CheckPost(int artworkId, bool isAccept, int notiId)
		{
			if (isAccept)
			{
				var updateStatusArtworkResult = await UpdateStatusArtwork(artworkId);
				if (!updateStatusArtworkResult.IsSuccess)
					return BadRequest("Update status artwork failed");
			}
			var updateStatusNotiResult = await UpdateStatusNoti(notiId, isAccept);
			if (!updateStatusNotiResult.IsSuccess)
				return BadRequest("Post notification failed");
			var postNotificationResult = await PostNotification(isAccept);
			if (!postNotificationResult.IsSuccess)
				return BadRequest("Post notification failed");
			var postUserNotificationResult = await PostUserNotification(artworkId, postNotificationResult.NotificationID);
			if (!postUserNotificationResult.IsSuccess)
				return BadRequest("Post user notification failed");

			return RedirectToAction("Index", "Dashbroad");
		}

		private async Task<(bool IsSuccess, HttpResponseMessage Response)> UpdateStatusArtwork(int artworkId)
		{
			ArtworkUpdateDTO dto = new ArtworkUpdateDTO
			{
				ArtworkId = artworkId,
				Status = ArtWorkStatus.InProgress
			};
			var response = await _client.PutAsJsonAsync<ArtworkUpdateDTO>(_client.BaseAddress + "Artwork/UpdateArtwork", dto);

			return (response.IsSuccessStatusCode, response);
		}

		private async Task<(bool IsSuccess, HttpResponseMessage Response)> UpdateStatusNoti(int notiId, bool isAccept)
		{
			var status = isAccept ? NotiStatus.AcceptPost : NotiStatus.DenyPost;

			UpdateNotiStatusDTO dto = new UpdateNotiStatusDTO
			{
				Id = notiId,
				notiStatus = status
			};
			var response = await _client.PutAsJsonAsync<UpdateNotiStatusDTO>(_client.BaseAddress + "Notification/UpdateStatusNoti", dto);

			return (response.IsSuccessStatusCode, response);
		}

		private async Task<(bool IsSuccess, int NotificationID)> PostNotification(bool IsAccept)
		{
			string description = IsAccept ? "Your artwork has been accepted" : "Your artwork has been rejected";

			createNotificationModel model = new createNotificationModel
			{
				Title = "Confirm post artwork",
				Description = description,
				notiStatus = NotiStatus.Normal
			};
			var response = await _client.PostAsync(
			   _client.BaseAddress + "Notification/CreateNotification",
			   new StringContent(
				   JsonConvert.SerializeObject(model),
				   Encoding.UTF8,
				   "application/json"));
			if (!response.IsSuccessStatusCode)
				return (false, 0);
			var data = response.Content.ReadAsStringAsync().Result;
			var dto = JsonConvert.DeserializeObject<ResponseNotificationDTO>(data);
			return (true, dto.Data.Id);
		}
		private async Task<(bool IsSuccess, HttpResponseMessage Response)> PostUserNotification(int artworkId, int notiId)
		{
			var user = await GetUserId(artworkId);
			if (!user.IsSuccess)
				return (false, null);

			var dto = new CreateUserNotificationDTO
			{
				userId = user.userId,
				notificationId = notiId,
				artworkId = artworkId
			};
			var response = await _client.PostAsync(
			   _client.BaseAddress + "UserNotifcation/CreateNotification",
			   new StringContent(
				   JsonConvert.SerializeObject(dto),
				   Encoding.UTF8,
				   "application/json"));

			return (response.IsSuccessStatusCode, response);
		}

		private async Task<(bool IsSuccess, string userId)> GetUserId(int artworkId)
		{
			ArtworkModel model = new ArtworkModel();
			HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + "Artwork/GetUserIdByArtworkId/" + artworkId);
			if (!response.IsSuccessStatusCode)
				return (false, string.Empty);

			var data = response.Content.ReadAsStringAsync().Result;
			model = JsonConvert.DeserializeObject<ArtworkModel>(data);
			return (true, model.UserId);

		}


	}
}
