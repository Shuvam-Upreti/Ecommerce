using System;
using System.Collections.Generic;

namespace Mover.Core.Entities;

public partial class ProductReview
{
    public int ReviewId { get; set; }

    public int? ProductId { get; set; }

    public int? UserId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? ReviewDate { get; set; }

    public virtual Product? Product { get; set; }

    public virtual UserDetail? User { get; set; }
}
