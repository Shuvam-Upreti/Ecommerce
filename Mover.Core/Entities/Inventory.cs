using System;
using System.Collections.Generic;

namespace Mover.Core.Entities;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public int? ProductId { get; set; }

    public int? QuantityInStock { get; set; }

    public DateTime? LastStockUpdate { get; set; }

    public virtual Product? Product { get; set; }
}
