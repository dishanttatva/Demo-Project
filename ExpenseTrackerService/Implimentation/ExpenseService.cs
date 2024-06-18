using ExpenseTrackerEntity.Models;
using ExpenseTrackerEntity.ViewModel;
//using ExpenseTracker.Models;
using ExpenseTrackerRepository.Interface;
using ExpenseTrackerService.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web.Helpers;

namespace ExpenseTrackerService.Implimentation
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _repository;
        private readonly IConfiguration _configuration;
        public ExpenseService(IExpenseRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;

        }
        public void RegisterUser(User model)
        {
            User data = new()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.FirstName + model.LastName,
                DateOfBirth = model.DateOfBirth,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                Password = Crypto.HashPassword(model.Password),
                PhoneNumber = model.PhoneNumber,
            };
            _repository.RegisterUser(data);
        }

        public string ValidateUser(LoginVM viewModel)
        {
            int userId = _repository.validate(viewModel.Email, viewModel.Password);

            if (userId != 0)
            {

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>
             {
               new Claim("UserId", userId.ToString()),
             };



                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);

            }
            return "";
        }
        public int ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "");
            var tokenvalidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Adjust the clock skew if needed
            };
            var principal = tokenHandler.ValidateToken(token, tokenvalidationParameters, out var validatedToken);
            var reqid = principal.FindFirst("UserId")?.Value;


            if (reqid != null)
            {
                return int.Parse(reqid);
            }

            return 0;
        }

        public void CreateCategory(string categoryName, int? userId)
        {
            Category category = new()
            {
                Name = categoryName,
                CreatedBy = (int)userId,
            };
            _repository.SaveCategory(category);
        }

        public HomeVM GetCategories(int userId)
        {
            return _repository.GetCategories(userId);
        }

        public void DeleteCategory(int id, int? userId)
        {
            _repository.DeleteCategory(id, userId);
        }

        public Category GetCategory(int id, int? userId)
        {
            return _repository.GetCategory(id, userId);
        }

        public void EditCategory(Category model, int? userId)
        {
            Category category = _repository.GetCategory(model.CategoryId, userId);
            category.Name = model.Name;
            _repository.UpdateCategory(category);
        }

        public UserDetailsVM GetUserDetials(int? v)
        {
            return _repository.GetUserDetails(v);
        }

        public void EditProfile(UserDetailsVM userDetail, int? u)
        {
            User user = _repository.GetUserDetail(u);
            user.FirstName = userDetail.FirstName;
            user.LastName = userDetail.LastName;
            user.PhoneNumber = userDetail.PhoneNumber;
            user.DateOfBirth = userDetail.DateOfBirth;
            user.Email = userDetail.Email;
            _repository.UpdateUser(user);
        }

        public void AddExpense(HomeVM viewModel, int? userId)
        {
            Expense expense = new()
            {
                UserId = (int)userId,
                CategoryId = (int)viewModel.CategoryId,
                Amount = (int)viewModel.Amount,
                Description = viewModel.Description,
                CreatedDate=viewModel.ExpenseDate,
                Name=viewModel.ExpenseName,
            };
            _repository.SaveExpense(expense);
        }

        public HomeVM GetExpenses(int categoryId,int userId)
        {
            return _repository.GetExpenses(categoryId,userId);
        }

        public HomeVM GetExpenseData(int id, int? userId)
        {
            return _repository.GetExpenseData(id, userId);
        }

        public void EditExpense(HomeVM model, int? userId)
        {
            Expense expense = _repository.GetExpense(model.ExpenseId, userId);
            expense.Name = model.ExpenseName;
            expense.Description = model.Description;
            expense.CreatedDate = model.ExpenseDate;
            expense.CategoryId = (int)model.CategoryId;
            expense.Amount = (int)model.Amount;
            _repository.UpdateExpense(expense);
        }

        public void DeleteExpense(int id, int? userId)
        {
            Expense expense= _repository.GetExpense(id, userId);
            _repository.DeleteExpense(expense);
        }
    }
}
