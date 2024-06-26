using ExpenseTrackerEntity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTrackerEntity.ViewModel
{
    public class HomeVM
    {
        public List<Expense>? Expenses { get; set; }
        public List<Category>? Categories { get; set; }
        public Category? Category { get; set; }
        public int Sum { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string ExpenseName { get; set; } = null!;
        [Required(ErrorMessage = "Date is required")]
        public DateOnly ExpenseDate {  get; set; }
        [Required(ErrorMessage = "Amount is required")]
        public int? Amount { get; set; } = null!;
        [Required(ErrorMessage = "Category is required")]
        public int? CategoryId { get; set; } = null!;
        public int ExpenseId { get; set; }
        [Required(ErrorMessage = "CategoryName is required")]
        public string CategoryName { get; set; } = null!;
        [Required(ErrorMessage ="Freequency is required")]
        public string? Description { get; set; }
        public double ItemsPerPage {  get; set; }
        public double PageCount {  get; set; }

    }
}
