using asp_mvc_website.DTO;
using asp_mvc_website.Models;
using asp_mvc_website.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace asp_mvc_website.Controllers
{
    public class UserController : Controller
    {

        private readonly ILogger<UserController> _logger;
        private readonly HttpClient _client;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICurrentUserService _currentUserService;
        public UserController(ILogger<UserController> logger, IHttpClientFactory httpClientFactory,
            IConfiguration configuration, ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _logger = logger;
            _client = new HttpClient();
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri(configuration["Cron:localhost"]);
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
                HttpContext.Session.SetString("RefeshToken", tokenResponse.RefreshToken);
                HttpContext.Session.SetString("UserEmail", model.Email);
                // Redirect user to the home page or another appropriate page


                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);
                var user = await _currentUserService.User();
                if (user != null)
                {
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                }

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
                IsAdmin = false
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

        public async Task<IActionResult> Logout()
        {
            var response = await _client.DeleteAsync(_client.BaseAddress + "User/SignOut");
            HttpContext.Session?.Remove("AccessToken");
            HttpContext.Session?.Remove("RefeshToken");
            return RedirectToAction("Index");
        }

        //public async Task<IActionResult> RefeshToken()
        //{
        //    var model = new TokenResponse
        //    {
        //        Token = HttpContext.Session?.GetString("AccessToken"),
        //        RefreshToken = HttpContext.Session?.GetString("RefeshToken"),
        //    };
        //    var response = await _client.PostAsync(_client.BaseAddress + "User/refresh-token", new StringContent(model.RefreshToken));
        //    if(!response.IsSuccessStatusCode)
        //    {
        //        return RedirectToAction("Logout");
        //    }
        //    var responseContent = await response.Content.ReadAsStringAsync();
        //    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);
        //    HttpContext.Session.SetString("AccessToken", tokenResponse.Token);
        //    HttpContext.Session.SetString("RefeshToken", tokenResponse.RefreshToken);
        //}

    }



    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}