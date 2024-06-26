using ExpenseTrackerEntity.Models;
using ExpenseTrackerEntity.ViewModel;
using ExpenseTrackerRepository.Interface;
using ExpenseTrackerService.Interface;
using iText.IO.Image;
using iText.Kernel.Pdf;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Numerics;
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
                CreatedBy = (int?)userId ?? 0,
            };
            if (!_repository.isCategorySaved(categoryName, userId))
            {
                _repository.SaveCategory(category);
                return true;
            }
            return false;
        }

        public List<Category> GetCategories(int userId)
        {
            return _repository.GetCategories(userId);
        }

        public void DeleteCategory(int id, int? userId)
        {
            _repository.DeleteCategory(id, userId);
            _repository.DeleteExpensesByCategoryId(id, userId);
        }

        public CategoryVM GetCategory(int id, int? userId)
        {
            return _repository.GetCategory(id, userId);
        }

        public bool EditCategory(CategoryVM model, int? userId)
        {
            Category category = _repository.GetCategoryModel(model.CategoryId, (int?)userId ?? 0);
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
                UserId = (int?)userId ?? 0,
                CategoryId = (int?)viewModel.CategoryId ?? 0,
                Amount = (int?)viewModel.Amount ?? 0,
                Description = viewModel.Description,
                CreatedDate = viewModel.ExpenseDate,
                Name = viewModel.ExpenseName,
            };
            _repository.SaveExpense(expense);
        }

        public HomeVM GetExpenses(int categoryId, int userId, int CurrentPage, int ItemsPerPage, bool OrderByDate, bool OrderByAmount, string search)
        {
            return _repository.GetExpenses(categoryId, userId, CurrentPage, ItemsPerPage, OrderByDate, OrderByAmount, search);
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
            expense.CategoryId = (int?)model.CategoryId ?? 0;
            expense.Amount = (int?)model.Amount ?? 0;
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

        public byte[] GeneratePdf(string imagePath)
        {


            using (MemoryStream stream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(stream);
                PdfDocument pdf = new PdfDocument(writer);
                iText.Layout.Document document = new iText.Layout.Document(pdf);
                iText.Layout.Element.Image graphImage = new(ImageDataFactory.Create(imagePath));
                graphImage.SetFixedPosition(100, 300);
                graphImage.ScaleToFit(400, 200);
                document.Add(graphImage);
                document.Close();
                return stream.ToArray();
            }
        }

        public bool ValidateEmail(string email)
        {
            return _repository.ValidateEmail(email);
        }

        public void QuickRegister(string email, string password, string firstname, DateOnly dateofbirth)
        {
            User user = new()
            {
                Email = email,
                Password = Crypto.HashPassword(password),
                FirstName = firstname,
                DateOfBirth = dateofbirth,
                UserName = firstname,
            };
            _repository.RegisterUser(user);
        }

        public string ValidateEmails(SpliteExpenseVM vm)
        {
            if (vm.Emails == null) { return "Please select atleast 2 members"; }
            else if (vm.Emails.Count != vm.Totals) { return "Please Validate all the members"; }
            else if (vm.Emails.Count() <= 1) { return "Please select atleast 2 members"; }
            else
            {
                var seenValues = new HashSet<string>();
                foreach (var item in vm.Emails)
                {

                    if (seenValues.Contains(item))
                    {
                        return "Please enter unique emails";
                    }
                    else
                    {
                        seenValues.Add(item);
                    }
                }
            }
            return "";
        }

        public void SendMailForSplitAmount(SpliteExpenseVM vm)
        {

            for (int i = 0; i < vm.Emails.Count(); i++)
            {
                var receiver = vm.Emails[i] ?? "";

                var subject = "Split Expense";
                var message = "";
                if (vm.SplittedAmount != 0)
                {

                    message = "You are having the split expense with amount of " + vm.SplittedAmount / vm.Totals;
                }
                else
                {
                    message = "You are having the split expense with amount of " + vm.SplitAmounts[i];
                }

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
        }

        public void SplitExpense(SpliteExpenseVM vm)
        {
            for (int i = 0; i < vm.Emails.Count(); i++)
            {
                var userId = _repository.GetUserIdByEmail(vm.Emails[i]);
                Expense expense = new Expense()
                {
                    UserId = userId,
                    Amount = vm.SplittedAmount != 0 ? vm.SplittedAmount / vm.Totals : vm.SplitAmounts[i],
                    Description = "Splitted amount",
                    CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                    CategoryId = -1,
                    Name = "Splitted amount",
                };
                _repository.SaveExpense(expense);
            }
        }

        public void SendMailForCreateAccount(string email, string password)
        {
            var receiver = email ?? "";

            var subject = "Created Account for Expense Tracker";
            var message = "Your account has been created. Email: " + email + "Password is : " + password;


            var mail = "tatva.dotnet.dishantsoni@outlook.com";
            var securityPassword = "Dishant@2002";

            var client = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, securityPassword)
            };

            client.SendMailAsync(new MailMessage(from: mail, to: receiver, subject, message));
        }

        public void AddRecurrenceExpense(RecurrenceVM vm, int userId)
        {
            Recurrence recurrence = new Recurrence()
            {
                RecurrenceName = vm.RecurrenceName,
                Amount = vm.Amount,
                FreequencyId = vm.FrequencyId,
                StartDate = vm.RecurrenceDate,
                DueDate = vm.FrequencyId == 1 ? vm.RecurrenceDate.AddDays(+1) : vm.FrequencyId == 2 ? vm.RecurrenceDate.AddDays(+7) : vm.RecurrenceDate.AddMonths(+1),
                CreatedBy = userId,
            };
            _repository.SaveRecurrence(recurrence);
        }

        public Recurrence CheckDueDate()
        {
            Recurrence recurrence = _repository.CheckDueDate();
            if (recurrence != null)
            {


                if (recurrence.FreequencyId == 1)
                {
                    recurrence.DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(+1));
                }
                else if (recurrence.FreequencyId == 2)
                {
                    recurrence.DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(+7));
                }
                else
                {
                    recurrence.DueDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(+1));

                }
                _repository.UpdateRecurrence(recurrence);
            }
            return recurrence;
        }

        public void TriggerAlert(Recurrence recurrence)
        {
            var receiver = _repository.GetEmailFromUserId(recurrence.CreatedBy);

            var subject = "Alert for Recurring Expense";
            var message = "Alert for the recurring expense of " + recurrence.RecurrenceName + " Created on " + recurrence.StartDate + " of amount: " + recurrence.Amount;


            var mail = "tatva.dotnet.dishantsoni@outlook.com";
            var securityPassword = "Dishant@2002";

            var client = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, securityPassword)
            };

            client.SendMailAsync(new MailMessage(from: mail, to: receiver, subject, message));
        }

        public RecurrenceVM GetRecurrences(int categoryId, int userId, int currentPage, int itemsPerPage, bool orderByDate, bool orderByAmount, string search)
        {
            return _repository.GetRecurrences(categoryId, userId, currentPage, itemsPerPage, orderByDate, orderByAmount, search);
        }

        public RecurrenceVM GetRecurrenceData(int id, int? userId)
        {
            return _repository.GetRecurrenceData(id, userId);
        }

        public void EditRecurrence(RecurrenceVM model, int? userId)
        {
            Recurrence recurrence = _repository.GetRecurrence(model.RecurrenceId, userId);
            recurrence.RecurrenceName = model.RecurrenceName;
            recurrence.StartDate = model.RecurrenceDate;
            recurrence.FreequencyId = (int?)model.FrequencyId ?? 0;
            recurrence.Amount = (int?)model.Amount ?? 0;
            _repository.UpdateRecurrence(recurrence);
        }

        public void DeleteRecurrence(int id, int? userId)
        {
            Recurrence recurrence = _repository.GetRecurrence(id, userId);
            recurrence.IsDeleted = true;
            _repository.UpdateRecurrence(recurrence);
        }

        public void CreateBudget(BudgetVM budgetVM, int userId)
        {
            Budget budget = new()
            {
                CreatedBy = userId,
                Type = budgetVM.BudgetType,
                Amount = budgetVM.BudgetAmount,
            };
            if (budgetVM.BudgetType == 1)
            {
                budget.CategroryId = budgetVM.Category_Id;
            }
            else if (budgetVM.BudgetType == 2)
            {
                budget.FrequenceyId = budgetVM.Freequency_Type;
            }
            else
            {
                budget.CategroryId = budgetVM.Category_Id;
                budget.FrequenceyId = budgetVM.Freequency_Type;
            }
            _repository.AddBudget(budget);
        }

        public List<Budget> GetBudgets(int userId)
        {
            return _repository.GetBudgets(userId);
        }

        public BudgetVM GetBudgetsData(int userId, int currentPage, int itemsPerPage, bool OrderByAmount)
        {
            return _repository.GetBudgetsData(userId, currentPage, itemsPerPage, OrderByAmount);
        }

        public BudgetVM GetBudget(int id, int? userId)
        {
            Budget budget = _repository.GetBudgetById(id);
            return new BudgetVM()
            {
                BudgetID = id,
                BudgetType = (int)budget.Type,
                Category_Id = budget.Type == 1 ? (int)budget.CategroryId : budget.Type == 2 ? 0 : (int)budget.CategroryId,
                Freequency_Type = budget.Type == 2 ? (int)budget.FrequenceyId : budget.Type == 1 ? 0 : (int)budget.FrequenceyId,
                Categories = _repository.GetCategories((int)userId),
                Freequencies=_repository.GetFreequencies(),
                BudgetAmount = (int)budget.Amount,
            };
        }

        public void EditBudget(BudgetVM budgetVM, int? userId)
        {
            Budget budget = _repository.GetBudgetById(budgetVM.BudgetID);
            if (budget.Type == 1)
            {
                budget.CategroryId = budgetVM.Category_Id;
            }
            else if (budget.Type == 2)
            {
                budget.FrequenceyId = budgetVM.Freequency_Type;
            }
            else
            {
                budget.CategroryId = budgetVM.Category_Id;
                budget.FrequenceyId = budgetVM.Freequency_Type;
            }
            budget.Amount = budgetVM.BudgetAmount;
            _repository.UpdateBudget(budget);
        }

        public void DeleteBudget(int id, int? userId)
        {
            Budget budget = _repository.GetBudgetById(id);
            budget.IsDeleted = true;
            _repository.UpdateBudget(budget);
        }

        public Budget CheckOverBudget(int? userId)
        {
            List<Budget> budgets = _repository.GetBudgetsForAlert((int)userId);
            foreach (Budget budget in budgets)
            {
                if (budget.Type == 1)
                {
                    int sum = _repository.GetSumAmountForCategoryWiseExpenses((int)budget.CategroryId, userId);
                    if (sum >= budget.Amount)
                    {
                        budget.IsAlertSend = true;
                        _repository.UpdateBudget(budget);
                        return budget;
                    }
                }
                else if (budget.Type == 2)
                {
                    int sum = 0;
                    if (budget.FrequenceyId == 1)
                    {
                        DateOnly date = DateOnly.FromDateTime(DateTime.Now);
                        sum = _repository.GetSumAmountForTimelyExpenses(date, date, userId);
                    }
                    else if (budget.FrequenceyId == 2)
                    {
                        int dayOfWeek = (int)DateTime.Now.DayOfWeek;
                        DateOnly startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-dayOfWeek));
                        DateOnly endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-dayOfWeek).AddDays(+6));
                        sum = _repository.GetSumAmountForTimelyExpenses(startDate, endDate, userId);
                    }
                    else if (budget.FrequenceyId == 3)
                    {

                        int daysInMonth = (int)DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                        DateOnly startDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1);
                        DateOnly endDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, daysInMonth);
                        sum = _repository.GetSumAmountForTimelyExpenses(startDate, endDate, userId);
                    }
                    if (sum >= budget.Amount)
                    {
                        budget.IsAlertSend = true;
                        _repository.UpdateBudget(budget);
                        return budget;
                    }
                }
                else
                {
                    DateOnly startDate = new(), endDate = new();
                    if (budget.FrequenceyId == 1)
                    {
                        startDate = DateOnly.FromDateTime(DateTime.Now);
                        endDate = DateOnly.FromDateTime(DateTime.Now);
                    }
                    else if (budget.FrequenceyId == 2)
                    {
                        int dayOfWeek = (int)DateTime.Now.DayOfWeek;
                        startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-dayOfWeek));
                        endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-dayOfWeek).AddDays(+6));
                    }
                    else if (budget.FrequenceyId == 3)
                    {
                        int daysInMonth = (int)DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                        startDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1);
                        endDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, daysInMonth);
                    }
                    int sum = _repository.GetSumForCategoryAndTimelyExpenses(userId, budget.CategroryId, startDate, endDate);
                    if (sum >= budget.Amount)
                    {
                        budget.IsAlertSend = true;
                        _repository.UpdateBudget(budget);
                        return budget;
                    }
                }
            }
            return new Budget();
        }



        public void SendMailForOverBudget(Budget budget, int? userId)
        {
            var receiver = _repository.GetEmailFromUserId(userId);

            var subject = "Alert for OverBudget";
            var message = "Alert for the OverBudget expense for amount : " + budget.Amount;


            var mail = "tatva.dotnet.dishantsoni@outlook.com";
            var securityPassword = "Dishant@2002";

            var client = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, securityPassword)
            };

            client.SendMailAsync(new MailMessage(from: mail, to: receiver, subject, message));
        }

        public List<Freequency> GetFreequencies()
        {
            return _repository.GetFreequencies();
        }
    }
}
