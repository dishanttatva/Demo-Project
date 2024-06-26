using ExpenseTrackerEntity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTrackerEntity.ViewModel
{
    public class RecurrenceVM
    {
        public int RecurrenceId { get; set; }
        [Required(ErrorMessage ="RecurrenceName is required")]
        public string? RecurrenceName { get; set;}=null!;
        [Required(ErrorMessage ="Freequency is required")]
        public int FrequencyId { get; set; }
        [Required(ErrorMessage ="Date is required")]
        public DateOnly RecurrenceDate {  get; set; }
        [Required(ErrorMessage = "Amount should not be 0")]
        public int? Amount { get; set; } = null!;
        public string? Description { get; set; }
        public List<Recurrence>? Recurrences { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Freequency>? Freequencies { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
    }
}
