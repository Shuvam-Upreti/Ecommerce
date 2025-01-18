using System;
using System.Collections.Generic;

namespace Mover.Core.Entities;

public partial class Order
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

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual UserDetail? User { get; set; }
}
