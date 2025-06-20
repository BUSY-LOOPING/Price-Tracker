namespace PriceTracker.Models.ViewModels
{
    public class DashboardViewModel
    {
        // Counts
        public int TotalProducts { get; set; }
        public int TotalTags { get; set; }
        public int TotalCategories { get; set; }

        // Recent product overview (Top 5 by date)
        public List<ProductDto> RecentProducts { get; set; } = new();

        // Price trend points
        public List<PriceTrendPoint> PriceTrends { get; set; } = new();
    }

    public class PriceTrendPoint
    {
        public DateTime RecordedAt { get; set; }
        public decimal Price { get; set; }
        public string ProductName { get; set; }
    }
}
