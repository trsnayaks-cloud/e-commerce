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
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
{
    return RedirectToAction("Login", "Account");
}

        ViewData["CurrentFilter"] = searchString;
        ViewData["CurrentCategory"] = category;
        ViewData["CurrentSortOrder"] = sortOrder;

        var products = _products.AsQueryable();

        if (!String.IsNullOrEmpty(searchString))
        {
            products = products.Where(p => (p.Name != null && p.Name.ToLower().Contains(searchString.ToLower())) ||
                                           (p.Description != null && p.Description.ToLower().Contains(searchString.ToLower())));
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
                products = products.OrderBy(p => p.Name ?? "");
                break;
        }

        ViewData["Favourites"] = HttpContext.Session.Get<List<int>>("Favourites") ?? new List<int>();
        var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
        ViewData["ProductIdsInCart"] = cart.Select(item => item.Product?.Id).ToList();

        return View(products.ToList());

    }

  public IActionResult ProductDetails(int id)
{
    if (HttpContext.Session.GetString("IsLoggedIn") != "true")
{
    return RedirectToAction("Login", "Account");
}

    var product = _products.FirstOrDefault(p => p.Id == id);
    if (product == null)
    {
        return NotFound();
    }

    var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
    ViewData["Cart"] = cart;   // Send the full cart to view

    return View(product);
}




    // MODIFIED: Added debug logging
    public IActionResult ToggleFavourite(int id)
    {
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
{
    return RedirectToAction("Login", "Account");
}

        var favourites = HttpContext.Session.Get<List<int>>("Favourites") ?? new List<int>();
        
        // --- DEBUG LOGGING ---
        Console.WriteLine($"--- ToggleFavourite (Product ID: {id}) ---");
        Console.WriteLine($"Favorites BEFORE toggling: [{string.Join(", ", favourites)}]");

        if (favourites.Contains(id))
        {
            favourites.Remove(id);
        }
        else
        {
            favourites.Add(id);
        }
        
        HttpContext.Session.Set("Favourites", favourites);

        // --- DEBUG LOGGING ---
        Console.WriteLine($"Favorites AFTER toggling: [{string.Join(", ", favourites)}]");
        Console.WriteLine("---------------------------------------------");

        return RedirectToAction("Index");
    }

    // MODIFIED: Added debug logging
    public IActionResult Favourites()
    {
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
{
    return RedirectToAction("Login", "Account");
}

        var favouriteIds = HttpContext.Session.Get<List<int>>("Favourites") ?? new List<int>();

        // --- DEBUG LOGGING ---
        Console.WriteLine($"--- Favourites Page Loaded ---");
        Console.WriteLine($"Favorites loaded from session: [{string.Join(", ", favouriteIds)}]");
        Console.WriteLine("---------------------------------------------");

        var favouriteProducts = _products.Where(p => favouriteIds.Contains(p.Id)).ToList();
        return View(favouriteProducts);
    }

    [HttpPost]
public IActionResult AddToCartConfirmed(int id, int quantity, string size)
{
    if (HttpContext.Session.GetString("IsLoggedIn") != "true")
{
    return RedirectToAction("Login", "Account");
}

    var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
    var product = _products.FirstOrDefault(p => p.Id == id);

    if (product != null)
    {
        // ✅ Find by Product + Size
        var existingItem = cart.FirstOrDefault(item => item.Product?.Id == id && item.Size == size);

        if (existingItem != null)
        {
            if (existingItem.Quantity + quantity > 3)
            {
                TempData["CartError"] = $"You can only order up to 3 units of {product.Name} (Size: {size}).";
            }
            else
            {
                existingItem.Quantity += quantity;
            }
        }
        else
        {
            if (quantity > 3)
            {
                TempData["CartError"] = $"You can only order up to 3 units of {product.Name} (Size: {size}).";
            }
            else
            {
                cart.Add(new CartItem { Product = product, Quantity = quantity, Size = size });
            }
        }

        HttpContext.Session.Set("Cart", cart);
    }

    return RedirectToAction("Cart");
}



   public IActionResult Cart()
{
    if (HttpContext.Session.GetString("IsLoggedIn") != "true")
{
    return RedirectToAction("Login", "Account");
}

    var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();

    // Detect referer (previous page URL)
    var referer = Request.Headers["Referer"].ToString();

    if (!string.IsNullOrEmpty(referer))
    {
        if (referer.Contains("/Home/ProductDetails"))
        {
            ViewData["GoBackUrl"] = referer; // back to Product Details
        }
        else if (referer.Contains("/Home/Favourites"))
        {
            ViewData["GoBackUrl"] = Url.Action("Favourites", "Home"); // back to Favourites
        }
        else
        {
            ViewData["GoBackUrl"] = Url.Action("Index", "Home"); // back to Home
        }
    }
    else
    {
        ViewData["GoBackUrl"] = Url.Action("Index", "Home"); // default
    }

    return View(cart);
}



    [HttpPost]
public IActionResult RemoveFromCart(int id, string size)
{
    if (HttpContext.Session.GetString("IsLoggedIn") != "true")
{
    return RedirectToAction("Login", "Account");
}

    var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();

    // Find by product + size
    var itemToRemove = cart.FirstOrDefault(item => item.Product?.Id == id && item.Size == size);
    if (itemToRemove != null)
    {
        cart.Remove(itemToRemove);
    }

    HttpContext.Session.Set("Cart", cart);
    return RedirectToAction("Cart");
}


   [HttpPost]
public IActionResult UpdateCartQuantity(int id, string size, int change)
{
    if (HttpContext.Session.GetString("IsLoggedIn") != "true")
{
    return RedirectToAction("Login", "Account");
}

    var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
    var item = cart.FirstOrDefault(i => i.Product?.Id == id && i.Size == size);

    if (item != null)
    {
        if (change > 0 && item.Quantity >= 3)
        {
            TempData["CartError"] = $"You can only order up to 3 units of {item.Product?.Name} ({item.Size}).";
        }
        else
        {
            item.Quantity += change;

            if (item.Quantity <= 0)
            {
                cart.Remove(item);
            }

            HttpContext.Session.Set("Cart", cart);
        }
    }

    return RedirectToAction("Cart");
}



    public IActionResult Privacy()
    {
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
{
    return RedirectToAction("Login", "Account");
}

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
{
    return RedirectToAction("Login", "Account");
}

        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    
}