using ExpenseTrackerEntity.Models;
using ExpenseTrackerEntity.ViewModel;
// using ExpenseTracker.ViewModel;
using ExpenseTrackerService.Interface;
using Microsoft.AspNetCore.Mvc;
// using ExpenseTrackerEntity.ViewModel;
namespace ExpenseTracker.Controllers
{
    public class LoginController : Controller
    {
        private readonly IExpenseService _service;
        public LoginController(IExpenseService service)
        {
            _service = service;
        }
        [HttpGet("expense-tracker/register")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost("expense-tracker/register")]
        public IActionResult Register(RegisterVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _service.RegisterUser(model);
                    TempData["success"] = "User Registered Successfully";
                    return RedirectToAction(nameof(Login));
                }
                return View(model);
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error", ex.Message);
            }
        }
        [HttpPost]
        public IActionResult Login(LoginVM viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string token = _service.ValidateUser(viewModel);
                    if (token != "")
                    {
                        var cookieOptions = new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(30),
                            HttpOnly = true,
                            Secure = true
                        };
                        HttpContext.Response.Cookies.Append("myToken", token, cookieOptions);
                        TempData["success"] = "Login Successfull";
                        return RedirectToAction("Index", "Expense");
                    }
                    TempData["error"] = "Invalid Credentials";
                    return RedirectToAction(nameof(Login));

                }
                return View(viewModel);
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error", ex.Message);
            }
        }
        [HttpGet("expense-tracker/forgot-password")]
        public IActionResult ForgotPassword()
        {
            LoginVM vm = new();
            return View(vm);
        }

        [HttpPost("expense-tracker/forgot-password")]
        public IActionResult ForgotPassword(LoginVM model)
        {
            try
            {
                string token = _service.GenerateToken(model.Email);
                _service.SendMailForCreateAccount(model.Email, token);
                TempData["success"] = "Email sent successfully";
                return RedirectToAction(nameof(Login));
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error", ex.Message);
            }
        }

        [HttpGet("expense-tracker/change-password")]
        public IActionResult ChangePassword(string token)
        {
            LoginVM vm= new LoginVM();
            vm.Token = token;
            return View(vm);
        }
        [HttpPost("expense-tracker/change-password")]
        public IActionResult ChangePassword(LoginVM vm)
        {
            try
            {
                bool isValid = _service.ValidateEmailToken(vm);
                TempData["success"] = "Password Changed Successfully";
                return RedirectToAction(nameof(Login));
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error", ex.Message);
            }
        }
        
    }
}
