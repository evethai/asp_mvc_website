using asp_mvc_website.Models;
using asp_mvc_website.Services;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;


namespace asp_mvc_website.Controllers
{
    public class PostArtworkController : Controller
    {
        private readonly ILogger<PostArtworkController> _logger;
        private readonly IHttpClientFactory _factory;
        private readonly HttpClient _client;
        private readonly ICurrentUserService _currentUserService;

        public PostArtworkController(ILogger<PostArtworkController> logger, IConfiguration configuration,
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

        private static string ApiKey = "AIzaSyB8sC0Z0tEfdI-1z-KHp7N25OJYBw8d1XU";
        private static string Bucket = "asp-system-57914.appspot.com";  
        private static string AuthEmail = "thaieve@gmail.com";
        private static string AuthPassword = "thaieve@123";
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> _Post(PostArtworkModel model)
        {
            if (ModelState.IsValid)
            {
                var file = model.File;
                if (model.IsNewCategory == true)
                {
                    CategoryAddDTO caDto = new CategoryAddDTO()
                    {
                        Name = model.Category,
                        Status = false,
                    };
                    var response = await _client.PostAsync(
                    _client.BaseAddress + "Catalogy",
                        new StringContent(
                        JsonConvert.SerializeObject(caDto),
                        Encoding.UTF8,
                        "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        //take category id
                        var data = response.Content.ReadAsStringAsync().Result;
                        var category = JsonConvert.DeserializeObject<CategoryModel>(data);
                        int id = category.Id;
                        string imageUrl = ImageURL(file);
                        PostArtworkDTO PDto = new PostArtworkDTO()
                        {
                            Title = model.Title,
                            Description = model.Description,
                            Price = (double)model.Price,
                            ImagesUrl = new List<string> { imageUrl },
                            CategoryIds = new List<int> { id },
                        };
                        var postArtwork = await _client.PostAsync(
                            _client.BaseAddress + "Artwork/AddArtwork",
                                new StringContent(
                                JsonConvert.SerializeObject(PDto),
                                Encoding.UTF8,
                                "application/json"));
                        if (postArtwork.IsSuccessStatusCode)
                        {
                            //true
                        }
                        else
                        {
                            return (StatusCode(500));
                        }

                    }
                    else
                    {
                        return(StatusCode(500));    
                    }
                }
                else
                {
                    string imageUrl = ImageURL(file);
                    PostArtworkDTO PDto = new PostArtworkDTO()
                    {
                        Title = model.Title,
                        Description = model.Description,
                        Price = (double)model.Price,
                        UserId = HttpContext.Session.GetString("UserId"),
                        ReOrderQuantity = 0,
                        ImagesUrl = new List<string> { imageUrl },
                        CategoryIds = new List<int> { int.Parse(model.Category) },
                    };
                    var postArtwork = await _client.PostAsync(
                        _client.BaseAddress + "Artwork/AddArtwork",
                        new StringContent(
                        JsonConvert.SerializeObject(PDto),
                        Encoding.UTF8,
                        "application/json"));
                    if (postArtwork.IsSuccessStatusCode)
                    {
                        
                    }else
                    {
                        return (StatusCode(500));
                    }
                }

            }

            return BadRequest(model);
        }

        private async Task<string> UploadImageToFirebase(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                    var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
                    var cancellation = new CancellationTokenSource();

                    var task = new FirebaseStorage(
                        Bucket,
                        new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                            ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                        })
                        .Child("assets")
                        .Child($"{file.FileName}")
                        .PutAsync(ms, cancellation.Token);

                    try
                    {
                        return await task;
                    }
                    catch (Exception ex)
                    {
                        // Handle exception if necessary
                        ViewBag.error = $"Exception was thrown: {ex}";
                        return null;
                    }
                }
            }
            else
            {
                ModelState.AddModelError("File", "Please select a file to upload.");
                return null;
            }
        }

        private string ImageURL(IFormFile file)
        {
            return UploadImageToFirebase(file).GetAwaiter().GetResult();
        }

    }
}
