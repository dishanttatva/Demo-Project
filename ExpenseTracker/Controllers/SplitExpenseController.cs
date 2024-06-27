using ExpenseTrackerEntity.ViewModel;
using ExpenseTrackerService.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    public class SplitExpenseController : Controller
    {
        private readonly IExpenseService _service;
        public SplitExpenseController(IExpenseService service)
        {
            _service = service;
        }

        [HttpGet("expense-tracker/split-expense")]
        public IActionResult SplitExpense()
        {
            return View();
        }
        public IActionResult ValidateEmail(string email)
        {
            bool isValidateEmail = _service.ValidateEmail(email);
            if(isValidateEmail)
            {
                TempData["success"] = "Email validated successfully";
            }
            return Json(new { result = isValidateEmail });
        }
        public IActionResult RegisterModal(string email)
        {
            UserDetailsVM vm = new UserDetailsVM();
            vm.Email = email;
            return PartialView("_RegisterModal", vm);
        }
        [HttpPost]
        public IActionResult QuickRegister(string email, string password, string firstname, DateOnly dateofbirth)
        {
            try
            {
                if (!email.Contains("@") || !email.Contains('.'))
                {
                    TempData["error"] = "Email is not valid";
                    return Ok();
                }
                _service.QuickRegister(email, password, firstname, dateofbirth);
                _service.SendMailForCreateAccount(email, password);
                TempData["success"] = "Account created successfully";
                return RedirectToAction(nameof(SplitExpense));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error", ex.Message);
            }
        }
        [HttpPost]
        public IActionResult SplitExpense(SpliteExpenseVM vm)
        {
            try
            {
                string errorMessage = _service.ValidateEmails(vm);
                if ((vm.SplittedAmount == 0 && vm.SplitAmounts?.Count() == 0) || (vm.SplitAmounts?.Count() != 0 && vm.SplitAmounts?.Count() != vm.Totals))
                {
                    TempData["error"] = "Split amount should not 0";
                    return RedirectToAction(nameof(SplitExpense));
                }

                if (errorMessage == "")
                {
                    _service.SendMailForSplitAmount(vm);
                    _service.SplitExpense(vm);
                    TempData["success"] = "Split Expense has been created";
                }
                else
                {
                    TempData["error"] = errorMessage;
                }
                return RedirectToAction(nameof(SplitExpense));
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error", ex.Message);
            }
        }
    }
}
