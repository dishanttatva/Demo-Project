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
        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Home/Register.cshtml");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Home/Login.cshtml");
        }
        [HttpPost]
        public IActionResult RegisterUser(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                _service.RegisterUser(model);
                TempData["success"] = "User Registered Successfully";
                return RedirectToAction(nameof(Login));
            }
            return View("~/Views/Home/Register.cshtml",model);
        }
        [HttpPost]
        public IActionResult LogIn(LoginVM viewModel)
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
                    return RedirectToAction("Index", "Home");
                }
                TempData["error"] = "Invalid Credentials";
                return RedirectToAction(nameof(Login));

            }
            return View("~/Views/Home/Login.cshtml", viewModel);
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            LoginVM vm = new();
            return View("~/Views/Home/FP.cshtml",vm);
        }

        [HttpPost]
        public IActionResult ForgotPassword(LoginVM model)
        {
            string token = _service.GenerateToken(model.Email);
            _service.SendMail(model.Email,token);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ChangePassword(string token)
        {
            LoginVM vm= new LoginVM();
            vm.Token = token;
            return View("~/Views/Home/ChangePassword.cshtml", vm);
        }
        [HttpPost]
        public IActionResult ChangePassword(LoginVM vm)
        {
            bool isValid = _service.ValidateEmailToken(vm);
            TempData["success"] = "Password Changed Successfully";
            return RedirectToAction(nameof(Login));
        }
    }
}
