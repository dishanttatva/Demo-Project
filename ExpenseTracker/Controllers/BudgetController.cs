using ExpenseTrackerEntity.Models;
using ExpenseTrackerEntity.ViewModel;
using ExpenseTrackerService.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    public class BudgetController : Controller
    {
        private readonly IExpenseService _service;
        public BudgetController(IExpenseService service)
        {
            _service = service;
        }
        [HttpGet("expense-tracker/budget")]
        public IActionResult Budget()
        {
            try
            {
                var userId = (int?)HttpContext.Session.GetInt32("UserId") ?? 0;
                List<Category> categories = _service.GetCategories(userId);
                List<Freequency> freequencies = _service.GetFrequencies();
                BudgetVM budgetVM = new BudgetVM() { Categories = categories, Freequencies = freequencies };
                return View(budgetVM);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error", ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CreateBudget(BudgetVM budgetVM)
        {
            try
            {
                var userId = (int?)HttpContext.Session.GetInt32("UserId") ?? 0;
                _service.CreateBudget(budgetVM, userId);
                TempData["success"] = "Budget has been created";
                return RedirectToAction(nameof(Budget));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error", ex.Message);
            }
        }
        [HttpGet]
        public IActionResult BudgetTable(int currentPage, int itemsPerPage, bool orderByAmount, int type)
        {
            try
            {
                var userId = (int?)HttpContext.Session.GetInt32("UserId") ?? 0;
                BudgetVM data = _service.GetBudgetsData(userId, currentPage, itemsPerPage, orderByAmount, type);
                ViewBag.Page = currentPage;
                return PartialView("_BudgetTable", data);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error", ex.Message);
            }
        }
        public IActionResult ShowBudgetModal(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                BudgetVM data = _service.GetBudget(id, userId);
                return PartialView("_EditBudget", data);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error", ex.Message);
            }
        }
        [HttpPost]
        public IActionResult EditBudget(BudgetVM budgetVM)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                _service.EditBudget(budgetVM, userId);
                TempData["success"] = "Budget has been updated";
                return RedirectToAction(nameof(Budget));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error", ex.Message);
            }
        }
        public IActionResult DeleteBudget(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                _service.DeleteBudget(id, userId);
                TempData["success"] = "Budget has been Deleted";
                return RedirectToAction(nameof(Budget));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error", ex.Message);
            }
        }
        public IActionResult ShowDeleteBudgetModal(int id)
        {
            return PartialView("_DeleteBudget", id);
        }
    }
}
