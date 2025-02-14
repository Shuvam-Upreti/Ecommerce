using Mover.Areas.Admin.ViewModel.Product;

namespace Mover.Areas.Admin.ViewModel.Order
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }

        public int? UserId { get; set; }

        public DateTime? OrderDate { get; set; }

        public string? OrderStatus { get; set; }

        public string? PaymentStatus { get; set; }

        public decimal? TotalAmount { get; set; }

        public string? ShippingAddressLine { get; set; }

        public string? ShippingCity { get; set; }

        public string? ShippingState { get; set; }

        public string? ShippingZipCode { get; set; }
        public string? CurrentUserRole { get; set; }

        public List<OrderItemViewModel> OrderItemsViewModel { get; set; } = new List<OrderItemViewModel>();

        public string? CreatedBy { get; set; }
        public string? PhoneNumber { get; set; }
    }   
    public class OrderItemViewModel
    {
        public int OrderItemId { get; set; }

        public int? OrderId { get; set; }

        public int? ProductId { get; set; }
        public string? ProductName { get; set; }

        public int? Quantity { get; set; }

        public decimal? PriceAtPurchase { get; set; }

        public decimal? DiscountAtPurchase { get; set; }

        //public List<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
    }
}
