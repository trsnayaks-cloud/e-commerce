using Microsoft.AspNetCore.Mvc;

namespace ECommercePortfolio.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
                return RedirectToAction("Login", "Account");

            return View();
        }
    }
}
