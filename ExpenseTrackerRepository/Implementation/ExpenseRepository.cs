using ExpenseTrackerEntity.Models;
using ExpenseTrackerEntity.ViewModel;
using ExpenseTrackerRepository.Interface;
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

        public int validate(string email, string password)
        {
            User user = _context.Users.FirstOrDefault(u => u.Email == email) ?? new();
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
                Categories = _context.Categories.Where(x => x.CreatedBy == userId).ToList(),
            };
        }

        public void DeleteCategory(int id, int? userId)
        {
            Category category = _context.Categories.FirstOrDefault(x => x.CategoryId == id && x.CreatedBy == userId) ?? new();
            _context.Categories.Remove(category);
            _context.SaveChanges();
        }

        public CategoryVM GetCategory(int id, int? userId)
        {
            Category category = _context.Categories.FirstOrDefault(u => u.CategoryId == id && u.CreatedBy == userId) ?? new();
            CategoryVM viewModel = new()
            {
                CategoryId = category.CategoryId,
                CategoryName = category.Name,
            };
            return viewModel;
        }

        public void UpdateCategory(Category category1)
        {
            _context.Categories.Update(category1);
            _context.SaveChanges();
        }

        public UserDetailsVM GetUserDetails(int? v)
        {
            User user = _context.Users.FirstOrDefault(x => x.Id == v) ?? new();
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

        public HomeVM GetExpenses(int categoryId, int userId, int CurrentPage, int ItemsPerPage, bool OrderByDate, bool OrderByAmount)
        {
            List<Expense> Expenses = _context.Expenses.Where(x => x.UserId == userId).ToList();
            if (categoryId != 0)
            {
                Expenses = Expenses.Where(x => x.CategoryId == categoryId).ToList();
            }
            if (OrderByDate)
            {
                Expenses = Expenses.OrderBy(x => x.CreatedDate).ToList();
            }
            if (OrderByAmount)
            {
                Expenses = Expenses.OrderBy(x => x.Amount).ToList();
            }
            var pageCount = Expenses.Count();

            if (CurrentPage != 0 && ItemsPerPage != 0)
            {
                Expenses = Expenses.Skip((CurrentPage - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
            }

            if (categoryId == 0)
            {
                return new HomeVM()
                {
                    ItemsPerPage = ItemsPerPage,
                    PageCount = pageCount,
                    Expenses = Expenses,
                    Sum = _context.Expenses.Where(x => x.UserId == userId).Sum(x => x.Amount),
                };
            }
            return new HomeVM()
            {
                ItemsPerPage = ItemsPerPage,
                PageCount = pageCount,
                Expenses = Expenses,
                Sum = _context.Expenses.Where(x => x.UserId == userId && x.CategoryId == categoryId).Sum(x => x.Amount),
            };

        }

        public HomeVM GetExpenseData(int id, int? userId)
        {
            return _context.Expenses.Where(x => x.ExpenseId == id && x.UserId == userId).Select(x => new HomeVM()
            {
                ExpenseName = x.Name,
                ExpenseDate = x.CreatedDate,
                Amount = x.Amount,
                CategoryId = x.CategoryId,
                Description = x.Description,
                Categories = _context.Categories.Where(x => x.CreatedBy == userId).ToList(),
                ExpenseId = id,
            }).First();
        }

        public Expense GetExpense(int expenseId, int? userId)
        {
            return _context.Expenses.FirstOrDefault(x => x.ExpenseId == expenseId && x.UserId == userId) ?? new();
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

        public bool isCategorySaved(string categoryName, int? userId)
        {
            Category category = _context.Categories.FirstOrDefault(x => x.Name.Trim().ToLower() == categoryName.Trim().ToLower() && x.CreatedBy == userId) ?? new();
            return category.CategoryId != 0 ? true : false;
        }

        public bool isEmail(string? emailClaim)
        {
            User user = _context.Users.FirstOrDefault(x => x.Email == emailClaim) ?? new();
            if (user.Id != 0) return true;
            return false;
        }

        public User ChangePassword(string? emailClaim, string Password)
        {
            User user = _context.Users.FirstOrDefault(x => x.Email == emailClaim) ?? new();
            user.Password = Crypto.HashPassword(Password);
            return user;
        }

        public int GetSumAmountByDate(DateOnly date, int UserId)
        {
            return _context.Expenses.Where(x => x.UserId == UserId && x.CreatedDate == date).Sum(x => x.Amount);
        }

        public int GetSumAmountByMonth(int month, int year, int userId)
        {
            return _context.Expenses.Where(x => x.UserId == userId && x.CreatedDate.Month == month && x.CreatedDate.Year == year).Sum(x => x.Amount);

        }
        public Category GetCategoryModel(int categoryId, int userId)
        {
            return _context.Categories.FirstOrDefault(x => x.CategoryId == categoryId && x.CreatedBy == userId) ?? new();
        }

        public int GetSumAmountByWeek(int userId, DateOnly firstDate, DateOnly date)
        {
            return _context.Expenses.Where(x => x.UserId == userId && x.CreatedDate >= firstDate && x.CreatedDate <= date).Sum(x => x.Amount);
        }
    }
}
