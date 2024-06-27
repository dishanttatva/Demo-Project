using System;
using System.Collections.Generic;

namespace ExpenseTrackerEntity.Models;

public partial class Budget
{
    public int BudgetId { get; set; }

    public int? CreatedBy { get; set; }

    public int? Type { get; set; }

    public int? CategroryId { get; set; }

    public int? FrequenceyId { get; set; }

    public int? Amount { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? IsAlertSend { get; set; }

    public virtual Category? Categrory { get; set; }

    public virtual User? CreatedByNavigation { get; set; }
}
