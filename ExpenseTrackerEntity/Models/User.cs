using System;
using System.Collections.Generic;

namespace ExpenseTrackerEntity.Models;

public partial class User
{
    public int Id { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string Email { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public string? PhoneNumber { get; set; }

    public DateOnly CreatedAt { get; set; }

    public DateOnly UpdatedAt { get; set; }

    public virtual ICollection<Expense> Expenses { get; } = new List<Expense>();
}
