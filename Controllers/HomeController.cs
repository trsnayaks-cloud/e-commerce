using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ECommercePortfolio.Models;
using ECommercePortfolio.Extensions;

namespace ECommercePortfolio.Controllers;

public class HomeController : Controller
{
    // The product data has been restored in this list
    private static readonly List<Product> _products = new List<Product>
    {
        new Product { Id = 1, Name = "Printed Signature Hoodie", Category = "Shirt", Description = "This light, summery hoodie crafted from organic cotton jersey brings a graphic signature accent to everyday looks...", Price = 19.99m, OriginalPrice = 22.99m, ImageUrl = "/images/tshirt.png" },
        new Product { Id = 2, Name = "Patchwork Denim Skate Pants", Category = "Jeans", Description = "These patchworked denim skate pants are from the Fall-Winter 2025 collaboration...", Price = 49.99m, OriginalPrice = 59.99m, ImageUrl = "/images/jeans.png" },
        new Product { Id = 3, Name = "LV Trainer Sneaker(1AIN14)", Category = "Shoes", Description = "This exclusive iteration of the cult LV Trainer sneaker combines nubuck leather with Damier canvas...", Price = 79.99m, OriginalPrice = 89.99m, ImageUrl = "/images/shoes.png" },
        new Product { Id = 4, Name = "Damier Long-Sleeved Shirt", Category = "Shirt", Description = "A wardrobe essential, this crisp white cotton shirt is adorned with a discreet jacquard Damier Seeds motif...", Price = 24.99m, OriginalPrice = 30.00m, ImageUrl = "/images/shirt1.png" },
        new Product { Id = 5, Name = "Silk Evening Shirt", Category = "Shirt", Description = "This black evening shirt is tailored in lustrous jacquard silk embellished with this collection’s Damier Bandana motif...", Price = 29.99m, OriginalPrice = 34.99m, ImageUrl = "/images/shirt2.png" },
        new Product { Id = 6, Name = "Wide Leg Denim Pants", Category = "Jeans", Description = "These wide-leg fit jeans from the Fall-Winter 2025 show are crafted from washed indigo denim...", Price = 59.99m, OriginalPrice = 70.00m, ImageUrl = "/images/jeans1.png" },
        new Product { Id = 7, Name = "Workwear Pants", Category = "Jeans", Description = "With their relaxed fit and versatile beige tones, these cotton canvas workwear pants offer a classic, effortless style...", Price = 64.99m, OriginalPrice = 75.00m, ImageUrl = "/images/jeans2.png" },
        new Product { Id = 8, Name = "LV Trainer Sneaker(1ABODA)", Category = "Shoes", Description = "Designed for Louis Vuitton by Virgil Abloh, and inspired by vintage basketball sneakers...", Price = 120.00m, OriginalPrice = 150.00m, ImageUrl = "/images/shoes1.png" },
        new Product { Id = 9, Name = "LV Trainer Sneaker(1A9JGR)", Category = "Shoes", Description = "Designed by Louis Vuitton Artistic Director Virgil Abloh, the iconic LV Trainer sneaker is revisited this season...", Price = 75.00m, OriginalPrice = 85.00m, ImageUrl = "/images/shoes2.png" }
    };

    public IActionResult Index(string searchString, string category, string sortOrder)
    {
        ViewData["CurrentFilter"] = searchString;
        ViewData["CurrentCategory"] = category;
        ViewData["CurrentSortOrder"] = sortOrder;

        var products = _products.AsQueryable();

        if (!String.IsNullOrEmpty(searchString))
        {
            products = products.Where(p => p.Name!.ToLower().Contains(searchString.ToLower()) || p.Description!.ToLower().Contains(searchString.ToLower()));
        }

        if (!String.IsNullOrEmpty(category) && category != "All")
        {
            products = products.Where(p => p.Category == category);
        }

        switch (sortOrder)
        {
            case "price_desc":
                products = products.OrderByDescending(p => p.Price);
                break;
            case "price_asc":
                products = products.OrderBy(p => p.Price);
                break;
            default:
                products = products.OrderBy(p => p.Name);
                break;
        }
        
        ViewData["Favourites"] = HttpContext.Session.Get<List<int>>("Favourites") ?? new List<int>();
        return View(products.ToList());
    }

    public IActionResult ProductDetails(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    public IActionResult ToggleFavourite(int id)
    {
        var favourites = HttpContext.Session.Get<List<int>>("Favourites") ?? new List<int>();
        if (favourites.Contains(id))
        {
            favourites.Remove(id);
        }
        else
        {
            favourites.Add(id);
        }
        HttpContext.Session.Set("Favourites", favourites);
        return RedirectToAction("Index");
    }

    public IActionResult Favourites()
    {
        var favouriteIds = HttpContext.Session.Get<List<int>>("Favourites") ?? new List<int>();
        var favouriteProducts = _products.Where(p => favouriteIds.Contains(p.Id)).ToList();
        return View(favouriteProducts);
    }

    [HttpPost]
    public IActionResult AddToCartConfirmed(int id, int quantity, string size)
    {
        var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product != null)
        {
            cart.Add(new CartItem { Product = product, Quantity = quantity, Size = size });
            HttpContext.Session.Set("Cart", cart);
        }
        return RedirectToAction("Cart");
    }

    public IActionResult Cart()
    {
        var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
        return View(cart);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}