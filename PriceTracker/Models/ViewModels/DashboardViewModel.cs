namespace PriceTracker.Models.ViewModels
{
    public class DashboardViewModel
    {
        public List<ProductDto> TopThreeProducts { get; set; } = new();
        //public List<CategoryRowViewModel> TopThreeCategories { get; set; } = new();
        public List<TagDto> TopThreeTags { get; set; } = new();
    }

    public class CategoryRowViewModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int ItemsCount { get; set; }
    }

    public class TagRowViewModel
    {
        public int TagId { get; set; }
        public string Name { get; set; }
        public int ProductsCount { get; set; }
    }
}
