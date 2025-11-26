using System;
using System.Collections.Generic;

namespace Entities;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int CategoryId { get; set; }

    public string? Description { get; set; }

    public double Price { get; set; }

    public string? ImgUrl { get; set; }

    public virtual Category Category { get; set; } = null!;
}
