using asp_mvc_website.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;

namespace asp_mvc_website.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _client;
        public CartController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _client = new HttpClient();
            //_client.BaseAddress = new Uri("https://localhost:7021/api/");
            _client.BaseAddress = new Uri("https://apiartwork.azurewebsites.net/api/");
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Retrieve cart items from session or cookie
            var cartItemsJson = HttpContext.Request.Cookies.ContainsKey("CartItems")
                ? HttpContext.Request.Cookies["CartItems"]
                : null;

            var cartItems = cartItemsJson != null
                ? JsonConvert.DeserializeObject<List<CartItemModel>>(cartItemsJson)
                : new List<CartItemModel>();

            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(string id)
        {
            ArtworkModel artworkModel = new ArtworkModel();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "Artwork/GetById/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                artworkModel = JsonConvert.DeserializeObject<ArtworkModel>(data);

				AddArtworkToCart(artworkModel);
			}
            return View();
        }

        private void AddArtworkToCart(ArtworkModel artworkModel)
        {
			//// Retrieve existing cart items from session
			//var cartItemsJson = HttpContext.Session.GetString("CartItems");
			//var cartItems = string.IsNullOrEmpty(cartItemsJson) ? new List<CartItemModel>() : JsonConvert.DeserializeObject<List<CartItemModel>>(cartItemsJson);

			//// Add the product to the cart items list
			//var itemCart = new CartItemModel
			//{
			//    artworkId = artworkModel.artworkId,
			//    Image = artworkModel.Image,
			//    Price = artworkModel.Price,
			//    Title = artworkModel.Title,
			//};
			//cartItems.Add(itemCart);

			//// Save the updated cart items back to session
			//HttpContext.Session.SetString("CartItems", JsonConvert.SerializeObject(cartItems));

			// Retrieve cart items from session or cookie
			var cartItemsJson = HttpContext.Request.Cookies.ContainsKey("CartItems")
			? HttpContext.Request.Cookies["CartItems"]
			: null;

			var cartItems = cartItemsJson != null
				? JsonConvert.DeserializeObject<List<CartItemModel>>(cartItemsJson)
				: new List<CartItemModel>();

			// Add the new item to the cart
			cartItems.Add(new CartItemModel
			{
				artworkId = artworkModel.artworkId,
				Title = artworkModel.Title,
				Price = artworkModel.Price,
				Image = artworkModel.Image
			});

			// Save the updated cart items back to cookie
			var options = new Microsoft.AspNetCore.Http.CookieOptions
			{
				Expires = System.DateTimeOffset.Now.AddDays(1)
			};
			HttpContext.Response.Cookies.Append("CartItems", JsonConvert.SerializeObject(cartItems), options);


		}


	}
}
