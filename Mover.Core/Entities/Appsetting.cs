using System;
using System.Collections.Generic;

namespace Mover.Core.Entities;

public partial class Appsetting
{
    public int Id { get; set; }

    public string? Key { get; set; }

    public string? Value { get; set; }
}
