using asp_mvc_website.Models;
using asp_mvc_website.Services;
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
        private readonly IHttpClientFactory _factory;
        private readonly ICurrentUserService _currentUserService;
        private CartService cartService;
        public CartController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, ICurrentUserService currentUserService, IConfiguration configuration)
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

			cartService = new CartService(cartItems);
			ViewBag.TotalPrice =  cartService.CalculateTotalPrice();

            return View(cartItems);	
        }

        [HttpGet]
        public IActionResult AddToCart(string id)
        {
            ArtworkModel artworkModel = new ArtworkModel();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "Artwork/GetById/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                artworkModel = JsonConvert.DeserializeObject<ArtworkModel>(data);

				var IsExisted = AddArtworkToCart(artworkModel);
                if (IsExisted)
                {
                    TempData["DupplicateId"] = "The artwork is already in the cart";
                }
            }
            return Redirect("/Shop");
        }

        private bool AddArtworkToCart(ArtworkModel artworkModel)
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

            cartService = new CartService(cartItems);

			bool IsExisted = cartService.IsDuplicateArtworkId(cartItems, artworkModel.artworkId);
			if (IsExisted)
			{
                return true;
			}
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
			return false;
		}

		[Route("Cart/DeleteArtwork/{artworkId}")]
		[HttpDelete]
		public IActionResult DeleteArtwork(int artworkId)
		{
			// Deserialize the cart items from the cookie
			var cartItemsJson = HttpContext.Request.Cookies["CartItems"];
			var cartItems = cartItemsJson != null
				? JsonConvert.DeserializeObject<List<CartItemModel>>(cartItemsJson)
				: new List<CartItemModel>();

			// Find the index of the item to delete based on its artworkId
			int indexToDelete = -1;
			for (int i = 0; i < cartItems.Count; i++)
			{
				if (cartItems[i].artworkId == artworkId)
				{
					indexToDelete = i;
					break;
				}
			}

			// If the item exists in the cart, remove it
			if (indexToDelete != -1)
			{
				cartItems.RemoveAt(indexToDelete);

				// Save the updated cart items back to the cookie
				var options = new Microsoft.AspNetCore.Http.CookieOptions
				{
					Expires = System.DateTimeOffset.Now.AddDays(1)
				};
				HttpContext.Response.Cookies.Append("CartItems", JsonConvert.SerializeObject(cartItems), options);
			}

			// Return true to indicate success or failure
			return Ok();
		}


	}
}
