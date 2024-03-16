using asp_mvc_website.DTO;
using asp_mvc_website.Models;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;


namespace asp_mvc_website.Controllers
{
    public class PostArtworkController : Controller
    {
        private readonly HttpClient _client;
        public PostArtworkController()
        {
            _client = new HttpClient();
            //_client.BaseAddress = new Uri("https://localhost:7021/api/");
            _client.BaseAddress = new Uri("https://apiartwork.azurewebsites.net/api/");
        }

        private static string ApiKey = "AIzaSyB8sC0Z0tEfdI-1z-KHp7N25OJYBw8d1XU";
        private static string Bucket = "asp-system-57914.appspot.com";  
        private static string AuthEmail = "thaieve@gmail.com";
        private static string AuthPassword = "thaieve@123";
        public IActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> _Post(PostArtworkModel model)
        //{
        //    var file = model.File;
        //    var userId = HttpContext.Session.GetString("UserId");
        //    if (model.IsNewCategory == true)
        //    {
        //        CategoryAddDTO caDto = new CategoryAddDTO()
        //        {
        //            Name = model.Category,
        //            Status = false,
        //        };
        //        var response = await _client.PostAsync(
        //        _client.BaseAddress + "Catalogy",
        //            new StringContent(
        //            JsonConvert.SerializeObject(caDto),
        //            Encoding.UTF8,
        //            "application/json"));
        //        if (response.IsSuccessStatusCode)
        //        {
        //            //take category id
        //            var data = response.Content.ReadAsStringAsync().Result;
        //            var category = JsonConvert.DeserializeObject<CategoryModel>(data);
        //            int id = category.Id;
        //            string imageUrl = ImageURL(file);
        //            PostArtworkDTO PDto = new PostArtworkDTO()
        //            {
        //                Title = model.Title,
        //                Description = model.Description,
        //                Price = (double)model.Price,
        //                UserId = userId,
        //                ImagesUrl = new List<string> { imageUrl },
        //                CategoryIds = new List<int> { id },
        //            };
        //            var postArtwork = await _client.PostAsync(
        //                _client.BaseAddress + "Artwork/AddArtwork",
        //                    new StringContent(
        //                    JsonConvert.SerializeObject(PDto),
        //                    Encoding.UTF8,
        //                    "application/json"));
        //            if (postArtwork.IsSuccessStatusCode)
        //            {
        //                HttpResponseMessage decs = await DecsQuantityPoster(userId);
        //                if (decs.IsSuccessStatusCode)
        //                {
        //                    HttpContext.Session.SetString("IsPost", "OK");
        //                    return RedirectToAction("Index", "Shop");
        //                }
        //                else
        //                {
        //                    return (StatusCode(500,"Error when decs quantity post of poster"));
        //                }
        //            }
        //            else
        //            {
        //                return (StatusCode(500,"Can not post new Artwork"));
        //            }

        //        }
        //        else
        //        {
        //            return StatusCode(500,"Can not create new Category");
        //        }
        //    }
        //    else
        //    {
        //        string imageUrl = ImageURL(file);
        //        PostArtworkDTO PDto = new PostArtworkDTO()
        //        {
        //            Title = model.Title,
        //            Description = model.Description,
        //            Price = (double)model.Price,
        //            UserId = userId,
        //            ReOrderQuantity = 0,
        //            ImagesUrl = new List<string> { imageUrl },
        //            CategoryIds = new List<int> { int.Parse(model.Category) },
        //        };
        //        var postArtwork = await _client.PostAsync(
        //            _client.BaseAddress + "Artwork/AddArtwork",
        //            new StringContent(
        //            JsonConvert.SerializeObject(PDto),
        //            Encoding.UTF8,
        //            "application/json"));
        //        if (postArtwork.IsSuccessStatusCode)
        //        {
        //            HttpResponseMessage decs = await DecsQuantityPoster(userId);
        //            if (decs.IsSuccessStatusCode)
        //            {
        //                HttpContext.Session.SetString("IsPost", "OK");
        //                return RedirectToAction("Index", "Shop");
        //            }
        //            else
        //            {
        //                return (StatusCode(500, "Error when decs quantity post of poster"));
        //            }
        //        }
        //        else
        //        {
        //            return (StatusCode(500, "Can not post new Artwork"));
        //        }
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> _Post(PostArtworkModel model)
        //{
        //    //var userId = HttpContext.Session.GetString("UserId");

        //    //var categoryPostResult = await PostCategoryIfNew(model);
        //    //if (!categoryPostResult.IsSuccess)
        //    //    return StatusCode(500, "Cannot create new Category");

        //    //var artworkPostResult = await PostArtwork(model, userId, categoryPostResult.CategoryId);
        //    //if (!artworkPostResult.IsSuccess)
        //    //    return StatusCode(500, "Cannot post new Artwork");

        //    //var decsResult = await DecsQuantityPoster(userId);
        //    //if (!decsResult.IsSuccessStatusCode)
        //    //    return StatusCode(500, "Error when decs quantity post of poster");

        //    ViewBag.HideModal = true;

        //    HttpContext.Session.SetString("IsPost", "OK");
        //    return RedirectToAction("Index", "Home");
        //}

        //private async Task<(bool IsSuccess, int CategoryId)> PostCategoryIfNew(PostArtworkModel model)
        //{
        //    if (!model.IsNewCategory)
        //        return (true, int.Parse(model.Category));

        //    var caDto = new CategoryAddDTO
        //    {
        //        Name = model.Category,
        //        Status = false
        //    };

        //    var response = await _client.PostAsync(
        //        _client.BaseAddress + "Catalogy",
        //        new StringContent(
        //            JsonConvert.SerializeObject(caDto),
        //            Encoding.UTF8,
        //            "application/json"));

        //    if (!response.IsSuccessStatusCode)
        //        return (false, 0);

        //    var data = response.Content.ReadAsStringAsync().Result;
        //    var dto = JsonConvert.DeserializeObject<ResponseDTO>(data);
            
        //    return (true, dto.Data.id);
        //}

        //private async Task<(bool IsSuccess, HttpResponseMessage Response)> PostArtwork(PostArtworkModel model, string userId, int categoryId)
        //{
        //    var imageUrl = ImageURL(model.File);

        //    var PDto = new PostArtworkDTO
        //    {
        //        Title = model.Title,
        //        Description = model.Description,
        //        Price = (double)model.Price,
        //        UserId = userId,
        //        ReOrderQuantity = 0,
        //        ImagesUrl = new List<string> { imageUrl },
        //        CategoryIds = new List<int> { categoryId }
        //    };

        //    var postArtwork = await _client.PostAsync(
        //        _client.BaseAddress + "Artwork/AddArtwork",
        //        new StringContent(
        //            JsonConvert.SerializeObject(PDto),
        //            Encoding.UTF8,
        //            "application/json"));

        //    return (postArtwork.IsSuccessStatusCode, postArtwork);
        //}
        //private async Task<HttpResponseMessage> DecsQuantityPoster (string userId)
        //{
        //    var request = new HttpRequestMessage(HttpMethod.Put, _client.BaseAddress + "Poster/DecreasePost?userId=" + userId);
        //    return await  _client.SendAsync(request);
        //}

        //private async Task<string> UploadImageToFirebase(IFormFile file)
        //{
        //    if (file != null && file.Length > 0)
        //    {
        //        using (var ms = new MemoryStream())
        //        {
        //            await file.CopyToAsync(ms);
        //            ms.Seek(0, SeekOrigin.Begin);

        //            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
        //            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
        //            var cancellation = new CancellationTokenSource();

        //            var task = new FirebaseStorage(
        //                Bucket,
        //                new FirebaseStorageOptions
        //                {
        //                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
        //                    ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
        //                })
        //                .Child("assets")
        //                .Child($"{file.FileName}")
        //                .PutAsync(ms, cancellation.Token);

        //            try
        //            {
        //                return await task;
        //            }
        //            catch (Exception ex)
        //            {
        //                // Handle exception if necessary
        //                ViewBag.error = $"Exception was thrown: {ex}";
        //                return null;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("File", "Please select a file to upload.");
        //        return null;
        //    }
        //}

        //private string ImageURL(IFormFile file)
        //{
        //    return UploadImageToFirebase(file).GetAwaiter().GetResult();
        //}

    }
}
