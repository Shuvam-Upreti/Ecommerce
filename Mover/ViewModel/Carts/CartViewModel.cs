using Mover.Core.Dto.Carts;

namespace Mover.ViewModel.Carts
{
    public class CartViewModel
    {
        //product
        public int ProductId { get; set; }

        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        //cart
        public int CartId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal? ProductPrice { get; set; }

    }
    public class SummaryViewModel
    {
        public decimal SummaryTotalPrice { get; set; }
        public ShippingViewModel? ShippingDetails { get; set; }
        public List<CartViewModel> CartViewModel{ get; set; } = new List<CartViewModel>();
    }
    public class ShippingViewModel
    {
        public string? CreaterName { get; set; }
        public string? ShippingAddressLine { get; set; }

        public string? ShippingCity { get; set; }

        public string? ShippingState { get; set; }

        public string? ShippingZipCode { get; set; }
    }
}
