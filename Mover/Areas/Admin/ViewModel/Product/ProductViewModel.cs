using System.ComponentModel.DataAnnotations;

namespace Mover.Areas.Admin.ViewModel.Product
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public string? Description { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal? DiscountedPrice { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public string? Category { get; set; }
        public int? CategoryId { get; set; }
        [Required]
        public List<IFormFile> Images{ get; set; } = new List<IFormFile>();

        // List to hold files selected by user for upload
        [Required]
        public List<IFormFile> NewImages { get; set; } = new List<IFormFile>();

        // List of image URLs that the user wants to delete
        public List<string> DeleteImages { get; set; } = new List<string>();

        // List of existing image URLs to be displayed in the view
        public List<string> ImageUrls { get; set; } = new List<string>();

    }
}
