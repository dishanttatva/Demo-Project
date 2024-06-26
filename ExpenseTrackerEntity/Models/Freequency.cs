using System;
using System.Collections.Generic;

namespace ExpenseTrackerEntity.Models;

public partial class Freequency
{
    public int FreequencyId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Budget> Budgets { get; } = new List<Budget>();

    public virtual ICollection<Recurrence> Recurrences { get; } = new List<Recurrence>();
}
