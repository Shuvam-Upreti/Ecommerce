using System;
using System.Collections.Generic;

namespace Mover.Core.Entities;

public partial class Address
{
    public int AddressId { get; set; }

    public int? UserId { get; set; }

    public string? AddressLine { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? ZipCode { get; set; }

    public virtual UserDetail? User { get; set; }
}
