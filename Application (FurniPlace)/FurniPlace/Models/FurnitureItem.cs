namespace FurnitureMarketplace.Models
{
    public class FurnitureItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Condition { get; set; } = "Вживане";
        public string Category { get; set; } = "Інше";
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; } 
        public string ContactInfo { get; set; } = string.Empty; 
    }
}