using asp_mvc_website.DTO;
using asp_mvc_website.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace asp_mvc_website.Controllers
{
    public class UserController : Controller
    {

        private readonly ILogger<UserController> _logger;
        private readonly HttpClient _client;
        public UserController(ILogger<UserController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _client = new HttpClient();
			//_client = httpClientFactory.CreateClient();
			_client.BaseAddress = new Uri("https://apiartwork.azurewebsites.net/api/");
			//_client.BaseAddress = new Uri("http://localhost:5012/api/");
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            var model = new LoginModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var loginDTO = new LoginDTO
            {
                Email = model.Email,
                Password = model.Password
            };

            var json = JsonConvert.SerializeObject(loginDTO);
            var url = _client.BaseAddress + "User/SignIn";

            // Send login request to Web API
            var response = await _client.PostAsync(
                _client.BaseAddress + "User/SignIn", 
                new StringContent(
                    JsonConvert.SerializeObject(model),
                    Encoding.UTF8, 
                    "application/json"));

            if (response.IsSuccessStatusCode)
            {
                // Read response content
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

                // Store token in session, cookie, or local storage
                HttpContext.Session.SetString("AccessToken", tokenResponse.Token);
                HttpContext.Session.SetString("UserEmail", model.Email);
                // Redirect user to the home page or another appropriate page
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["ErrorMessage"] = "Invalid username or password";
                //ModelState.AddModelError(string.Empty, "Invalid username or password");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult _IndexCha(string id)
        {
            id = "1932234f-af4a-45e5-b8d9-b613d2dfbb5d";
            List<GetUsetNotification> userNoti = new List<GetUsetNotification>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "UserNotifcation/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                userNoti = JsonConvert.DeserializeObject<List<GetUsetNotification>>(data);
                HttpContext.Session.SetString("MyListSessionKey", JsonConvert.SerializeObject(userNoti));
            }

            return View(userNoti.ToList());
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var registerDTO = new RegisterDTO
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
            };

            // Send registration request to Web API
            var response = await _client.PostAsync(
                _client.BaseAddress + "User/SignUp", 
                new StringContent(
                    JsonConvert.SerializeObject(registerDTO),
                    Encoding.UTF8, 
                    "application/json"));

            if (response.IsSuccessStatusCode)
            {
                // Registration successful, redirect to login page or other appropriate action
                return RedirectToAction("Login", "User");
            }
            else
            {
                // Handle error response
                // Example: Display error message to user
                ViewBag.ErrorMessage = "An error occurred during registration. Please try again.";
                return View(model);
            }
        }
    }

    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
