using ExpenseTrackerEntity.Models;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerEntity.ViewModel
{
    public class BudgetVM
    {
        public int BudgetID { get; set; }
        [Required(ErrorMessage ="BudgetName is required")]
        public string? BudgetName { get; set; }
        [Required(ErrorMessage = "Type is required")]
        public int? BudgetType { get; set; } = null!;
        [Required(ErrorMessage = "CategoryId is required")]
        public int? Category_Id { get; set; } = null!;
        [Required(ErrorMessage = "Freequency is required")]
        public int? Freequency_Type { get; set; } = null!;
        [Required(ErrorMessage = "Amount should not be 0")]
        public int? BudgetAmount { get; set; } = null!;
        public List<Freequency>? Freequencies { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Budget>? Budgets { get; set; }
        public int ItemsPerPage { get; set; }
        public int PageCount { get; set; }
    }
}
