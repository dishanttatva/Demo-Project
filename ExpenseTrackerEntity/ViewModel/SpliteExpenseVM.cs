using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTrackerEntity.ViewModel
{
    public class SpliteExpenseVM
    {
        public List<string>? Emails { get; set; }
        public int SplittedAmount { get; set; }
        public List<int>? SplitAmounts {  get; set; }
        public int Totals { get; set; }
    }
}
