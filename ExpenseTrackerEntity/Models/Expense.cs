using System;
using System.Collections.Generic;

namespace ExpenseTrackerEntity.Models;

public partial class Expense
{
    public int ExpenseId { get; set; }

    public int UserId { get; set; }

    public int CategoryId { get; set; }

    public int Amount { get; set; }

    public string? Description { get; set; }

    public DateOnly CreatedDate { get; set; }

    public string? Name { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
