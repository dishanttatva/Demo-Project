using ExpenseTrackerEntity.Models;
using ExpenseTrackerEntity.ViewModel;
//using ExpenseTracker.Models;
using ExpenseTrackerRepository.Interface;
using ExpenseTrackerService.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Cryptography.X509Certificates;

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
            var reqid = principal.FindFirst("UserId")?.Value;


            if (reqid != null)
            {
                return int.Parse(reqid);
            }

            return 0;
        }

        public bool CreateCategory(string categoryName, int? userId)
        {
            Category category = new()
            {
                Name = categoryName,
                CreatedBy = (int)userId,
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

        public Category GetCategory(int id, int? userId)
        {
            return _repository.GetCategory(id, userId);
        }

        public bool EditCategory(Category model, int? userId)
        {
            Category category = _repository.GetCategory(model.CategoryId, userId);
            if (!_repository.isCategorySaved(model.Name, userId))
            {
                category.Name = model.Name;
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
                UserId = (int)userId,
                CategoryId = (int)viewModel.CategoryId,
                Amount = (int)viewModel.Amount,
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
            expense.CategoryId = (int)model.CategoryId;
            expense.Amount = (int)model.Amount;
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
            var message = "Tap on link for Reset the Password : https://localhost:7145/Login/PasswordChange?token=" + token;


            var mail = "tatva.dotnet.dishantsoni@outlook.com";
            var password = "Dishant@2002";

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
            if (isEmail) {
               User user= _repository.ChangePassword(emailClaim,vm.Password);
                _repository.UpdateUser(user);
                return true; 
            }
            return false;
        }

        public SalesData GetDailyReportData(int UserId)
        {

            SalesData data=new SalesData ();
            DateOnly TodayDate = DateOnly.FromDateTime(DateTime.Now);
            for(int i=1;i<=TodayDate.Day;i++)
            {
                DateOnly date = DateOnly.FromDateTime(new DateTime(TodayDate.Year, TodayDate.Month, i));
                
                data.labels.Add(i.ToString("D2"));
                data.budget.Add(_repository.GetSumAmountByDate(date,UserId));
            }
            return data;
        }

        public SalesData GetMonthlyReportData(int userId)
        {
            
            SalesData data=new SalesData();
            DateOnly TodayDate = DateOnly.FromDateTime(DateTime.Now);
            for(int i=1;i<=TodayDate.Month; i++)
            {
                data.labels.Add(i.ToString("D2"));
                data.budget.Add(_repository.GetSumAmountByMonth(i,TodayDate.Year, userId));
            }
            return data;
        }

        //public string ValidatePassword(string email)
        //{
        //    int userId = _repository.validate(email);

        //    if (userId != 0)
        //    {

        //        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
        //        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        //        var claims = new List<Claim>
        //     {
        //       new Claim("UserId", userId.ToString()),
        //     };



        //        var token = new JwtSecurityToken(
        //            issuer: _configuration["Jwt:Issuer"],
        //            audience: _configuration["Jwt:Audience"],
        //            claims: claims,
        //            expires: DateTime.Now.AddMinutes(30),
        //            signingCredentials: credentials);

        //        return new JwtSecurityTokenHandler().WriteToken(token);

        //    }
        //    return "";
        //}
    }
}
