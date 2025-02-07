using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mover.Core.Dto.Carts
{
	public class CartDto
	{
		public int CartId { get; set; }
		public int ProductId { get; set; }

		public string? ProductName { get; set; }

		public int Quantity { get; set; }
		public int CreatedBy { get; set; }
		public decimal? ProductPrice { get; set; }
		public decimal TotalPrice { get; set; }
		public decimal? DiscountPercentage { get; set; }
		public List<string> ImageUrl { get; set; } = new List<string>();
	}
    public class SummaryDto
    {
        public ShippingDto? ShippingDetails { get; set; }
        public List<CartDto> CartDto { get; set; } = new List<CartDto>();
    }  
	public class ShippingDto
    {
        public string? CreaterName { get; set; }
        public string? ShippingAddressLine { get; set; }

        public string? ShippingCity { get; set; }

        public string? ShippingState { get; set; }

        public string? ShippingZipCode { get; set; }
    }
}
