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
        HomeVM GetCategories(int userId);
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
        bool ValidateEmails(SpliteExpenseVM vm);
        void SendMailForSplitAmount(List<string>? emails,int amount, int total);
        void SplitExpense(List<string>? emails, int splittedAmount,int totals);
        void SendMailForCreateAccount(string email, string password);
        //string ValidatePassword(string email);
    }
}
