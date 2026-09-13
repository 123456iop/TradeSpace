using System;
using System.Collections.Generic;

namespace TradeSpace.Models;

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    // Зв'язок з магазином продавця
    public Guid? StoreId { get; set; }
    public Store? Store { get; set; }

    public Guid? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();

    public ICollection<Product> Products { get; set; } = new List<Product>();
}