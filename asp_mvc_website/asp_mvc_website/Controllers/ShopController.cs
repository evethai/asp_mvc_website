using asp_mvc_website.Models;
using asp_mvc_website.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.Text;




namespace asp_mvc_website.Controllers
{
    public class ShopController : Controller
    {
        private readonly ILogger<ShopController> _logger;
        private readonly IHttpClientFactory _factory;
        private readonly HttpClient _client;
        private readonly ICurrentUserService _currentUserService;

        public ShopController(ILogger<ShopController> logger, IConfiguration configuration,
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
        int pageSize = 15;
        [HttpGet]
        public IActionResult Index()
        {
            //get list artwork
            List<ArtworkModel> artworkList = new List<ArtworkModel>();
            int page = 1;
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "Artwork/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                artworkList = JsonConvert.DeserializeObject<List<ArtworkModel>>(data);

                ////get 12 first artwork
                //artworkList = artworkList.Take(15).ToList();
                int totalItems = artworkList.Count;
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                ViewBag.TotalPages = totalPages;
                ViewBag.CurrentPage = page; ViewBag.IsPreviousDisabled = (page == 1);
                ViewBag.IsNextDisabled = (page == totalPages);

                artworkList = artworkList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            }
            List<CategoryModel> categoryList = new List<CategoryModel>();
            HttpResponseMessage responseCategory = _client.GetAsync(_client.BaseAddress + "Catalogy").Result;
            if (responseCategory.IsSuccessStatusCode)
            {
                string data = responseCategory.Content.ReadAsStringAsync().Result;
                categoryList = JsonConvert.DeserializeObject<List<CategoryModel>>(data);
            }
            HomeModel homeModel = new HomeModel();
            homeModel.ArtworkList = artworkList;
            homeModel.CategoryList = categoryList;

            if (TempData["DupplicateId"] != null)
            {
                ViewBag.DupplicateId = TempData["DupplicateId"];
            }

            return View(homeModel);
        }


        public IActionResult _searchArtwork(string txtSearch, int idCategory, int sortBy , int page)
        {
            // Get list of artwork
            List<ArtworkModel> artworkList = new List<ArtworkModel>();

            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "Artwork/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                artworkList = JsonConvert.DeserializeObject<List<ArtworkModel>>(data);

                if (!string.IsNullOrEmpty(txtSearch))
                {
                    artworkList = artworkList.Where(x => x.Description.ToLower().Contains(txtSearch.ToLower()) ||
                                                         x.Title.ToLower().Contains(txtSearch.ToLower())).ToList();
                }
                if (idCategory > 0)
                {
                    artworkList = artworkList.Where(x => x.categories.Contains(idCategory)).ToList();
                }

                if (sortBy == 1)
                {
                    artworkList = artworkList.OrderBy(x => x.Title).ToList();
                }
                else if (sortBy == 2)
                {
                    artworkList = artworkList.OrderByDescending(x => x.Price).ToList();
                }
                else if (sortBy == 3)
                {
                    artworkList = artworkList.OrderBy(x => x.CreateOn).ToList();
                }
            }

            int totalItems = artworkList.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            ViewBag.TotalItems = totalPages;
            ViewBag.CurrentPage = page;

            if (artworkList.Count == 0)
            {
                ViewBag.Message = "No artwork found";
            }

            artworkList = artworkList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return View(artworkList);
        }

    }
}
