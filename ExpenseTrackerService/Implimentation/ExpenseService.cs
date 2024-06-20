using ExpenseTrackerEntity.Models;
using ExpenseTrackerEntity.ViewModel;
using ExpenseTrackerRepository.Interface;
using ExpenseTrackerService.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
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

        public void RegisterUser(RegisterVM model)
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
                PhoneNumber = model.PhoneNumber.ToString(),
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
            var userId = principal.FindFirst("UserId")?.Value;


            if (userId != null)
            {
                return int.Parse(userId);
            }

            return 0;
        }

        public bool CreateCategory(string categoryName, int? userId)
        {
            Category category = new()
            {
                Name = categoryName,
                CreatedBy = (int?)userId??0,
            };
            if (!_repository.isCategorySaved(categoryName, userId))
            {
                _repository.SaveCategory(category);
                return true;
            }
            return false;
        }

        public HomeVM GetCategories(int userId)
        {
            return _repository.GetCategories(userId);
        }

        public void DeleteCategory(int id, int? userId)
        {
            _repository.DeleteCategory(id, userId);
        }

        public CategoryVM GetCategory(int id, int? userId)
        {
            return _repository.GetCategory(id, userId);
        }

        public bool EditCategory(CategoryVM model, int? userId)
        {
            Category category = _repository.GetCategoryModel(model.CategoryId, (int?)userId??0);
            if (!_repository.isCategorySaved(model.CategoryName, userId))
            {
                category.Name = model.CategoryName;
                _repository.UpdateCategory(category);
                return true;
            }
            return false;
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
                UserId = (int?)userId??0,
                CategoryId = (int?)viewModel.CategoryId??0,
                Amount = (int?)viewModel.Amount??0,
                Description = viewModel.Description,
                CreatedDate = viewModel.ExpenseDate,
                Name = viewModel.ExpenseName,
            };
            _repository.SaveExpense(expense);
        }

        public HomeVM GetExpenses(int categoryId, int userId, int CurrentPage, int ItemsPerPage, bool OrderByDate, bool OrderByAmount)
        {
            return _repository.GetExpenses(categoryId, userId, CurrentPage, ItemsPerPage, OrderByDate, OrderByAmount);
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
            expense.CategoryId = (int?)model.CategoryId??0;
            expense.Amount = (int?)model.Amount??0;
            _repository.UpdateExpense(expense);
        }

        public void DeleteExpense(int id, int? userId)
        {
            Expense expense = _repository.GetExpense(id, userId);
            _repository.DeleteExpense(expense);
        }



        public void SendMail(string email, string token)
        {
            var receiver = email ?? "";

            var subject = "Reset Password";
            var message = "Tap on link for Reset the Password : https://localhost:7145/expense-tracker/change-password?token=" + token;


            var mail = "tatva.dotnet.dishantsoni@outlook.com";
            var password = "I'm not written password beacuase of security";

            var client = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };

            client.SendMailAsync(new MailMessage(from: mail, to: receiver, subject, message));
        }

        public string GenerateToken(string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
              new Claim(ClaimTypes.Email, email),
            };

            var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateEmailToken(LoginVM vm)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "");
            var tokenValidationParameters = new TokenValidationParameters
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
            var principal = tokenHandler.ValidateToken(vm.Token, tokenValidationParameters, out var validatedToken);

            var emailClaim = principal.FindFirst(ClaimTypes.Email)?.Value;
            bool isEmail = _repository.isEmail(emailClaim);
            if (isEmail)
            {
                User user = _repository.ChangePassword(emailClaim, vm.Password);
                _repository.UpdateUser(user);
                return true;
            }
            return false;
        }

        public SalesData GetDailyReportData(int UserId)
        {

            SalesData data = new SalesData();
            DateOnly TodayDate = DateOnly.FromDateTime(DateTime.Now);
            for (int i = 1; i <= TodayDate.Day; i++)
            {
                DateOnly date = DateOnly.FromDateTime(new DateTime(TodayDate.Year, TodayDate.Month, i));

                data.labels?.Add(i.ToString("D2"));
                data.budget?.Add(_repository.GetSumAmountByDate(date, UserId));
            }
            return data;
        }
        public enum MonthEnum
        {
            Jan = 1,
            Feb = 2,
            Mar = 3,
            Apr = 4,
            May = 5,
            Jun = 6,
            Jul = 7,
            Aug = 8,
            Sep = 9,
            Oct = 10,
            Nov = 11,
            Dec = 12
        }
        public enum WeekEnum
        {
            First = 1,
            Second = 2,
            Third = 3,
            Fourth = 4,
            Fifth = 5,

        }
        public SalesData GetMonthlyReportData(int userId)
        {

            SalesData data = new SalesData();
            DateOnly TodayDate = DateOnly.FromDateTime(DateTime.Now);
            for (int i = 1; i <= TodayDate.Month; i++)
            {
                MonthEnum monthEnum = (MonthEnum)i;
                string monthName = monthEnum.ToString();
                data.labels?.Add(monthName);
                data.budget?.Add(_repository.GetSumAmountByMonth(i, TodayDate.Year, userId));
            }
            return data;
        }

        public SalesData GetWeeklyReportData(int userId)
        {
            SalesData data = new SalesData();
            DateOnly TodayDate = DateOnly.FromDateTime(DateTime.Now);
            DateOnly firstDate = new DateOnly(TodayDate.Year, TodayDate.Month, 1);
            int DayOfWeek = (int)firstDate.DayOfWeek;
            DateOnly Date = firstDate.AddDays(7 - DayOfWeek);
            int amount = _repository.GetSumAmountByWeek(userId, firstDate, Date);
            int index = 1;
            data.labels?.Add("First");
            index++;
            data.budget?.Add(amount);
            DateOnly lastDate = new DateOnly(TodayDate.Year, TodayDate.Month, DateTime.DaysInMonth(TodayDate.Year, TodayDate.Month));
            while (Date < lastDate)
            {
                WeekEnum weekEnum = (WeekEnum)index;
                string weekName = weekEnum.ToString();
                data.labels?.Add(weekName);
                data.budget?.Add(_repository.GetSumAmountByWeek(userId, Date, Date.AddDays(+7)));
                Date = Date.AddDays(+7);
                index++;
            }
            return data;
        }


    }
}
