using ExpenseTrackerEntity.Models;
using ExpenseTrackerEntity.ViewModel;
//using ExpenseTracker.Models.Data;
using ExpenseTrackerRepository.Interface;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace ExpenseTrackerRepository.Implementation
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly DemoProjectContext _context;
        public ExpenseRepository(DemoProjectContext context)
        {
            _context = context;
        }

        public int validate(string email,string password)
        {
            User user= _context.Users.FirstOrDefault(u => u.Email == email)??new();
            if (user.Id != 0 && Crypto.VerifyHashedPassword(user.Password, password))
            {
                return user.Id;
            }
            return 0;
        }

       

        public void RegisterUser(User model)
        {
            _context.Users.Add(model);
            _context.SaveChanges();
        }

        public void SaveCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public HomeVM GetCategories(int userId)
        {
            return new HomeVM()
            {
                Categories = _context.Categories.Where(x=>x.CreatedBy==userId).ToList(),
            };
        }

        public void DeleteCategory(int id, int? userId)
        {
            Category category = _context.Categories.FirstOrDefault(x => x.CategoryId == id && x.CreatedBy == userId)??new();
            _context.Categories.Remove(category);
            _context.SaveChanges();
        }

        public Category GetCategory(int id, int? userId)
        {
            return _context.Categories.FirstOrDefault(u => u.CategoryId == id && u.CreatedBy == userId) ?? new();
        }

        public void UpdateCategory(Category category1)
        {
            _context.Categories.Update(category1);
            _context.SaveChanges();
        }

        public UserDetailsVM GetUserDetails(int? v)
        {
            User user= _context.Users.FirstOrDefault(x => x.Id == v) ?? new();
            return new UserDetailsVM()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,

            };
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public User GetUserDetail(int? u)
        {
            return _context.Users.FirstOrDefault(x => x.Id == u) ?? new();
        }

        public void SaveExpense(Expense expense)
        {
           _context.Expenses.Add(expense);
           _context.SaveChanges();
        }

        public HomeVM GetExpenses(int categoryId, int userId)
        {
            if (categoryId == 0)
            {
                return new HomeVM() { 
                    Expenses = _context.Expenses.Where(x => x.UserId == userId).ToList(),
                    Sum= _context.Expenses.Where(x=>x.UserId==userId).Sum(x=>x.Amount),
                };
            }
            return new HomeVM() {
                Expenses= _context.Expenses.Where(x => x.UserId == userId && x.CategoryId == categoryId).ToList(),
                Sum = _context.Expenses.Where(x => x.UserId == userId && x.CategoryId==categoryId).Sum(x => x.Amount),
            };

        }

        public HomeVM GetExpenseData(int id, int? userId)
        {
           return _context.Expenses.Where(x => x.ExpenseId == id && x.UserId == userId).Select(x => new HomeVM()
            {
                ExpenseName=x.Name,
                ExpenseDate=x.CreatedDate,
                Amount=x.Amount,
                CategoryId=x.CategoryId,
                Description=x.Description,
                Categories=_context.Categories.Where(x=>x.CreatedBy==userId).ToList(),
                ExpenseId=id,
            }).First();
        }

        public Expense GetExpense(int expenseId, int? userId)
        {
           return _context.Expenses.FirstOrDefault(x=>x.ExpenseId==expenseId && x.UserId==userId)??new();
        }

        public void UpdateExpense(Expense expense)
        {
            _context.Expenses.Update(expense);
            _context.SaveChanges();
        }

        public void DeleteExpense(Expense expense)
        {
            _context.Expenses.Remove(expense);
            _context.SaveChanges();
        }
    }
}
