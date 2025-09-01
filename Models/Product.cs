namespace ECommercePortfolio.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; } // Added for accurate filtering
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public string? ImageUrl { get; set; }
    }
}