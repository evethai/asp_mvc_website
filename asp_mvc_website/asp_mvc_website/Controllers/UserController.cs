using asp_mvc_website.DTO;
using asp_mvc_website.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
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
			//_client.BaseAddress = new Uri("https://apiartwork.azurewebsites.net/api/");
			_client.BaseAddress = new Uri("http://localhost:5012/api/");
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

		[AllowAnonymous]
		public async Task<ActionResult> ExternalLogin()
		{
            var props = new AuthenticationProperties { RedirectUri = "/user/GoogleLogin" };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
		}

		public async Task<ActionResult> GoogleLogin()
		{
            var responseGoogle = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (responseGoogle.Principal == null) return BadRequest();
            var name = responseGoogle.Principal.FindFirstValue(ClaimTypes.Name);
            var givenName = responseGoogle.Principal.FindFirstValue(ClaimTypes.GivenName);
            var email = responseGoogle.Principal.FindFirstValue(ClaimTypes.Email);
            //Do something with the claims
            // var user = await UserService.FindOrCreate(new { name, givenName, email});
            var user = new RegisterDTO { 
                FirstName = name,
                LastName = name,
                Email = email,
                Password = email
            };

            // Send login request to Web API
            var response = await _client.PostAsync(
                _client.BaseAddress + "User/GoogleLogin", 
                new StringContent(
                    JsonConvert.SerializeObject(user),
                    Encoding.UTF8, 
                    "application/json"));

            if (response.IsSuccessStatusCode)
            {
                // Read response content
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

                // Store token in session, cookie, or local storage
                HttpContext.Session.SetString("AccessToken", tokenResponse.Token);
                HttpContext.Session.SetString("UserEmail", email);
                // Redirect user to the home page or another appropriate page
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["ErrorMessage"] = "Invalid username or password";
                //ModelState.AddModelError(string.Empty, "Invalid username or password");
                return View();
            }


            //var response = await _client.PostAsync(
            //    _client.BaseAddress + "User/SignUp",
            //    new StringContent(
            //        JsonConvert.SerializeObject(user),
            //        Encoding.UTF8,
            //        "application/json"));

            //if (response.IsSuccessStatusCode)
            //{   
            //    HttpContext.Session.SetString("UserEmail", email);
            //    // Redirect user to the home page or another appropriate page
            //    return RedirectToAction("Index", "Home");
            //}
            //else
            //{
            //    // Handle error response
            //    // Example: Display error message to user
            //    ViewBag.ErrorMessage = "An error occurred during registration. Please try again.";
            //    return RedirectToAction("Login", "User");
            //}

            return RedirectToAction("Login", "User");
        }


	}

    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
