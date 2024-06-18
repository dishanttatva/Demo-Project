using ExpenseTrackerEntity.Models;
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
        public IActionResult RegisterUser(User model)
        {
            if (ModelState.IsValid)
            {
                _service.RegisterUser(model);
            }
            return RedirectToAction(nameof(Register));
        }
        [HttpPost]
        public IActionResult LogIn(ExpenseTrackerEntity.ViewModel.LoginVM viewModel)
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
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction(nameof(Register));

            }
            return RedirectToAction(nameof(Register));
        }
    }
}
