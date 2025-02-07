using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mover.Core.Dto.Product
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public int? InStock { get; set; }

        public string ProductName { get; set; } = null!;

        public string? Description { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal? DiscountedPrice { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public string? Category { get; set; }
        public int? CategoryId { get; set; }

        // Image URLs of the existing product
        public List<string> ImageUrls { get; set; } = new List<string>();

        // List of new images to be added
        public List<IFormFile> NewImages { get; set; } = new List<IFormFile>();

        // List of image URLs to be deleted
        public List<string> DeleteImages { get; set; } = new List<string>();
    }
}
