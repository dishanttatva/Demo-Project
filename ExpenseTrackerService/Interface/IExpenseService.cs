using ExpenseTrackerEntity.Models;
using ExpenseTrackerEntity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTrackerService.Interface
{
    public interface IExpenseService
    {
        void RegisterUser(RegisterVM model);
         string ValidateUser(LoginVM viewModel);
        int ValidateToken(string token);
        bool CreateCategory(string categoryName, int? userId);
        List<Category> GetCategories(int userId);
        void DeleteCategory(int id, int? userId);
        CategoryVM GetCategory(int id, int? userId);
        bool EditCategory(CategoryVM model, int? userId);
        UserDetailsVM GetUserDetials(int? v);
        void EditProfile(UserDetailsVM user,int? userId);
        void AddExpense(HomeVM viewModel, int? userId);
        HomeVM GetExpenses(int categoryId,int userId,int CurrentPage,int ItemsPerPage, bool OrderByDate, bool OrderByAmount,string search);
        HomeVM GetExpenseData(int id, int? userId);
        void EditExpense(HomeVM model, int? userId);
        void DeleteExpense(int id, int? userId);
        void SendMail(string email, string token);
        string GenerateToken(string email);
        bool ValidateEmailToken(LoginVM vm);
        SalesData GetDailyReportData(int UserId);
        SalesData GetMonthlyReportData(int userId);
        SalesData GetWeeklyReportData(int userId);
        byte[] GeneratePdf(string imagePath);
        bool ValidateEmail(string email);
        void QuickRegister(string email, string password, string firstname, DateOnly dateofbirth);
        string ValidateEmails(SpliteExpenseVM vm);
        void SendMailForSplitAmount(SpliteExpenseVM vm);
        void SplitExpense(SpliteExpenseVM vm);
        void SendMailForCreateAccount(string email, string password);
        void AddRecurrenceExpense(RecurrenceVM vm, int userId);
        Recurrence CheckDueDate();
        void TriggerAlert(Recurrence recurrence);
        RecurrenceVM GetRecurrences(int categoryId, int userId, int currentPage, int itemsPerPage, bool orderByDate, bool orderByAmount, string search);
        RecurrenceVM GetRecurrenceData(int id, int? userId);
        void EditRecurrence(RecurrenceVM model, int? userId);
        void DeleteRecurrence(int id, int? userId);
        void CreateBudget(BudgetVM budgetVM, int userId);
        List<Budget> GetBudgets(int userId);
        BudgetVM GetBudgetsData(int userId,int currentPage,int itemsPerPage,bool OrderByAmount);
        BudgetVM GetBudget(int id, int? userId);
        void EditBudget(BudgetVM budgetVM, int? userId);
        void DeleteBudget(int id, int? userId);
        Budget CheckOverBudget(int? userId);
        void SendMailForOverBudget(Budget budget, int? userId);
        List<Freequency> GetFreequencies();
        //string ValidatePassword(string email);
    }
}
