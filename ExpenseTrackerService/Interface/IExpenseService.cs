﻿using ExpenseTrackerEntity.Models;
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
        void RegisterUser(User model);
         string ValidateUser(LoginVM viewModel);
        int ValidateToken(string token);
        void CreateCategory(string categoryName, int? userId);
        HomeVM GetCategories(int userId);
        void DeleteCategory(int id, int? userId);
        Category GetCategory(int id, int? userId);
        void EditCategory(Category model, int? userId);
        UserDetailsVM GetUserDetials(int? v);
        void EditProfile(UserDetailsVM user,int? userId);
        void AddExpense(HomeVM viewModel, int? userId);
        HomeVM GetExpenses(int categoryId,int userId);
        HomeVM GetExpenseData(int id, int? userId);
        void EditExpense(HomeVM model, int? userId);
        void DeleteExpense(int id, int? userId);
    }
}