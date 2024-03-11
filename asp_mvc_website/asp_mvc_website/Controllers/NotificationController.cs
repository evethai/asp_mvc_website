using Microsoft.AspNetCore.Mvc;

namespace asp_mvc_website.Controllers
{
    public class NotificationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
