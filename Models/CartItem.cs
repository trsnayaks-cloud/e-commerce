using ECommercePortfolio.Models;

namespace ECommercePortfolio.Models
{
    public class CartItem
    {
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public string? Size { get; set; }
    }
}