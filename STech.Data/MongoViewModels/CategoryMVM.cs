namespace STech.Data.MongoViewModels
{
    public class CategoryMVM
    {
        public string CategoryId { get; set; } = null!;

        public string CategoryName { get; set; } = null!;

        public string? ImageSrc { get; set; }

        public IEnumerable<ProductMVM>? Products { get; set; }
    }
}
