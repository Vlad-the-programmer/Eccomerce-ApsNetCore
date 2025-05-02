namespace EcommerceWebApp.Models.Dtos
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Brand { get; set; }
        public string Code { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? Photo { get; set; } = default!;
        public string? OtherPhotos { get; set; } = default!;
        public string? About { get; set; } = default!;
        public string LongAbout { get; set; }
        public int? RatingSum { get; set; } = default!;
        public int? RatingVotes { get; set; } = default!;
        public string SubcategoryCode { get; set; }
        public string CategoryCode { get; set; }
        public bool IsActive { get; set; }
        public IList<ReviewDTO> Reviews { get; set; } = new List<ReviewDTO>();
    }
}
