﻿using ExpenseTrackerEntity.Models;
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
       
        Task RegisterUser(User model);
        Task SaveCategory(Category category);
        HomeVM GetCategories(int userId);
        Task DeleteCategory(int id, int? userId);
        CategoryVM GetCategory(int id, int? userId);
        Task UpdateCategory(Category category1);
        UserDetailsVM GetUserDetails(int? v);
        Task UpdateUser(User user);
        User GetUserDetail(int? u);
       
        void SaveExpense(Expense expense);
        HomeVM GetExpenses(int categoryId, int userId,int CurrentPage,int ItemsPerPage, bool OrderByDate, bool OrderByAmount, string search);
        HomeVM GetExpenseData(int id, int? userId);
        Expense GetExpense(int expenseId, int? userId);
        Task UpdateExpense(Expense expense);
        Task DeleteExpense(Expense expense);
        bool isCategorySaved(string categoryName, int? userId);
        bool isEmail(string? emailClaim);
        User ChangePassword(string? emailClaim,string Password);
        int GetSumAmountByDate(DateOnly date,int UserId);
        int GetSumAmountByMonth(int month,int year, int userId);
        Category GetCategoryModel(int categoryId, int userId);
        int GetSumAmountByWeek( int userId, DateOnly firstDate, DateOnly date);
        Task DeleteExpensesByCategoryId(int id, int? userId);
        bool ValidateEmail(string email);
        int GetUserIdByEmail(string email);
        void SaveRecurrence(Recurrence recurrence);
        Recurrence CheckDueDate();
        void UpdateRecurrence(Recurrence recurrence);
        string GetEmailFromUserId(int? createdBy);
        HomeVM GetRecurrences(int categoryId, int userId, int currentPage, int itemsPerPage, bool orderByDate, bool orderByAmount, string search);
        HomeVM GetRecurrenceData(int id, int? userId);
        Recurrence GetRecurrence(int expenseId, int? userId);
    }
}
