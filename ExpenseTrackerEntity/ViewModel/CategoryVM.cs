using ExpenseTrackerEntity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTrackerEntity.ViewModel
{
    public class CategoryVM
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "CategoryName is Required")]
        public string CategoryName { get; set; } = null!;
        public List<Category>? Categories { get; set; }
        public int ItemsPerPage { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
    }
}
