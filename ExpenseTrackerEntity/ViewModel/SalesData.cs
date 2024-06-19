using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTrackerEntity.ViewModel
{
    public class SalesData
    {
        public List<string>? labels {  get; set; } = new List<string>();
        public List<int>? budget { get; set; }= new List<int>();
        public string? Type { get; set; }
    }
}
