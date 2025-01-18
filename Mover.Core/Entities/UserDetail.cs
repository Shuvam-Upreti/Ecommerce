using System;
using System.Collections.Generic;

namespace Mover.Core.Entities;

public partial class UserDetail
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string AspUserId { get; set; } = null!;

    public DateTime DateOfJoin { get; set; }

    public string? Department { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual AspNetUser AspUser { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
}
