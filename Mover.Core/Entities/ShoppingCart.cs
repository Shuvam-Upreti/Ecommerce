using System;
using System.Collections.Generic;

namespace Mover.Core.Entities;

public partial class ShoppingCart
{
    public int CartId { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual UserDetail User { get; set; } = null!;
}
