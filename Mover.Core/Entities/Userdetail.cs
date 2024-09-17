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

    public virtual AspNetUser AspUser { get; set; } = null!;
}
