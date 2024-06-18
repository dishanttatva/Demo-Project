using System;
using System.Collections.Generic;

namespace ExpenseTrackerEntity.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public int CreatedBy { get; set; }

    public virtual ICollection<Expense> Expenses { get; } = new List<Expense>();
}
