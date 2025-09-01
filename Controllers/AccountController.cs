using Microsoft.AspNetCore.Mvc;

namespace ECommercePortfolio.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Demo login (later we can replace with DB check)
            if (username == "admin" && password == "1234")
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Details()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
                return RedirectToAction("Login");

            return View();
        }

        public IActionResult Address()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
                return RedirectToAction("Login");

            return View();
        }

        public IActionResult Settings()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
                return RedirectToAction("Login");

            return View();
        }
    }
}
