using asp_mvc_website.Models;
using asp_mvc_website.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System.Text;

namespace asp_mvc_website.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _client;
        private readonly IHttpClientFactory _factory;
        private readonly ICurrentUserService _currentUserService;
        private CartService cartService;
        private readonly string _cartCookieName = "CartItems";
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
			var cartItems = GetCartItem();

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
			var cartItems = GetCartItem();

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

            SaveCartToCookie(cartItems);

            return false;
		}

		[Route("Cart/DeleteArtwork/{artworkId}")]
		[HttpDelete]
		public IActionResult DeleteArtwork(int artworkId)
		{

			var cartItems = GetCartItem();

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

				SaveCartToCookie(cartItems);
			}

			// Return true to indicate success or failure
			return Ok();
		}

		[HttpGet]
		public async Task<IActionResult> CheckOut(int artworkId)
		{
			ArtworkModel artworkModel = new ArtworkModel();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "Artwork/GetById/" + artworkId).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                artworkModel = JsonConvert.DeserializeObject<ArtworkModel>(data);
				if(artworkModel != null)
				{
					if (artworkModel.Status == ArtWorkStatus.PendingConfirmation){
						return RedirectToAction("Index");
					}
				}
				artworkModel.Status = ArtWorkStatus.PendingConfirmation;

                // Send registration request to Web API
                var responseUpdateArtwork = await _client.PostAsync(
							_client.BaseAddress + "Artwork/Update",
							new StringContent(
								JsonConvert.SerializeObject(artworkModel),
								Encoding.UTF8,
								"application/json"
							));
                if (responseUpdateArtwork.IsSuccessStatusCode)
                {
					var cartItems = GetCartItem();

					var itemToUpdate = cartItems.FirstOrDefault(item => item.artworkId == artworkId	);

					if (itemToUpdate != null)
					{
						// Update the item's quantity
						itemToUpdate.Status = ArtWorkStatus.PendingConfirmation;

                        // Save the updated cart back to session or database
                        SaveCartToCookie(cartItems);

                        // Return a success response or redirect to the cart page
                        return RedirectToAction("Index");
					}
					else
					{
						// Item not found in the cart, handle error
						return View("Error");
					}
				}

            }
			return RedirectToAction("Index");
		}

		private List<CartItemModel> GetCartItem()
		{
			var settings = new JsonSerializerSettings
			{
				// Provide default value for nullable enum property
				DefaultValueHandling = DefaultValueHandling.Populate,
				NullValueHandling = NullValueHandling.Ignore
			};

			// Retrieve cart items from session or cookie
			var cartItemsJson = HttpContext.Request.Cookies.ContainsKey(_cartCookieName)
				? HttpContext.Request.Cookies[_cartCookieName]
				: null;

			var cartItems = cartItemsJson != null
			? JsonConvert.DeserializeObject<List<CartItemModel>>(cartItemsJson, settings)
	:		new List<CartItemModel>();

			return cartItems;
		}

        private void SaveCartToCookie(List<CartItemModel> cartItems)
        {
            var cartJson = JsonConvert.SerializeObject(cartItems);
            HttpContext.Response.Cookies.Append(_cartCookieName, cartJson, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(7) // Adjust expiration as needed
            });
        }


    }
}
