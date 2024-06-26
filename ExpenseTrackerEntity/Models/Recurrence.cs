using System;
using System.Collections.Generic;

namespace ExpenseTrackerEntity.Models;

public partial class Recurrence
{
    public string? RecurrenceName { get; set; }

    public int? Amount { get; set; }

    public int? FreequencyId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? DueDate { get; set; }

    public bool? IsDeleted { get; set; }

    public int? CreatedBy { get; set; }

    public bool? IsAlertSend { get; set; }

    public int RecurrenceId { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Freequency? Freequency { get; set; }
}
