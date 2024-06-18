using ExpenseTrackerEntity.Models;
using ExpenseTrackerEntity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTrackerRepository.Interface
{
    public interface IExpenseRepository
    {
        int validate(string email,string password);
       
        void RegisterUser(User model);
        void SaveCategory(Category category);
        HomeVM GetCategories(int userId);
        void DeleteCategory(int id, int? userId);
        Category GetCategory(int id, int? userId);
        void UpdateCategory(Category category1);
        UserDetailsVM GetUserDetails(int? v);
        void UpdateUser(User user);
        User GetUserDetail(int? u);
       
        void SaveExpense(Expense expense);
        HomeVM GetExpenses(int categoryId, int userId);
        HomeVM GetExpenseData(int id, int? userId);
        Expense GetExpense(int expenseId, int? userId);
        void UpdateExpense(Expense expense);
        void DeleteExpense(Expense expense);
    }
}
