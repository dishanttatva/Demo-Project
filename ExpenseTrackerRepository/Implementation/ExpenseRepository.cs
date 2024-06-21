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



        public async Task RegisterUser(User model)
        {
            _context.Users.Add(model);
            await _context.SaveChangesAsync();
        }

        public async Task  SaveCategory(Category category)
        {
            _context.Categories.Add(category);
           await _context.SaveChangesAsync();
        }

        public HomeVM GetCategories(int userId)
        {
            return new HomeVM()
            {
                Categories = _context.Categories.Where(x => x.CreatedBy == userId).ToList(),
            };
        }

        public async Task DeleteCategory(int id, int? userId)
        {
            Category category = _context.Categories.FirstOrDefault(x => x.CategoryId == id && x.CreatedBy == userId) ?? new();
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
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

        public async Task UpdateCategory(Category category1)
        {
            _context.Categories.Update(category1);
            await _context.SaveChangesAsync();
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

        public async Task UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public User GetUserDetail(int? u)
        {
            return _context.Users.FirstOrDefault(x => x.Id == u) ?? new();
        }

        public async Task SaveExpense(Expense expense)
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
        }

        public HomeVM GetExpenses(int categoryId, int userId, int CurrentPage, int ItemsPerPage, bool OrderByDate, bool OrderByAmount,string search)
        {
            List<Expense> Expenses = _context.Expenses.Where(x => x.UserId == userId).ToList();
            if (categoryId != 0)
            {
                Expenses = Expenses.Where(x => x.CategoryId == categoryId).ToList();
            }

            var pageCount = Expenses.Count();
            if (search != "" && search != null && search != "undefined")
            {
                Expenses = Expenses.Where(x => x.Name.StartsWith(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (CurrentPage != 0 && ItemsPerPage != 0)
            {
                Expenses = Expenses.Skip((CurrentPage - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
            }
           
            if (OrderByDate)
            {
                Expenses = Expenses.OrderByDescending(x => x.CreatedDate).ToList();
            }
            if (OrderByAmount)
            {
                Expenses = Expenses.OrderByDescending(x => x.Amount).ToList();
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

        public async Task UpdateExpense(Expense expense)
        {
            _context.Expenses.Update(expense);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteExpense(Expense expense)
        {
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
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

        public async Task DeleteExpensesByCategoryId(int id, int? userId)
        {
            IQueryable<Expense> expenses= _context.Expenses.Where(x=>x.CategoryId==id && x.UserId==userId);
            foreach(var item in expenses)
            {
                _context.Expenses.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
