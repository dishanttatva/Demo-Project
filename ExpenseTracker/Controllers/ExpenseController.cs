using ExpenseTracker.AuthMIddleware;
using ExpenseTrackerEntity.Models;
using ExpenseTrackerEntity.ViewModel;
using ExpenseTrackerService.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;
[CustomeAuthorize()]
public class ExpenseController : Controller
{
    private readonly ILogger<ExpenseController> _logger;
    private readonly IExpenseService _service;

    public ExpenseController(ILogger<ExpenseController> logger, IExpenseService service)
    {
        _logger = logger;
        _service = service;
    }
    [HttpGet("expense-tracker/home")]
    public IActionResult Index()
    {
        try
        {
            int userId = _service.ValidateToken(Request.Cookies["myToken"] ?? "");
            HttpContext.Session.SetInt32("UserId", userId);
            List<Category> categories = _service.GetCategories(userId);
            HomeVM data = new() { Categories = categories };
            return View(data);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Error", ex.Message);
        }
    }

    [HttpGet]
    public IActionResult LogOut()
    {
        try
        {
            Response.Cookies.Delete("myToken");
            TempData["success"] = "LogOut successful";
            return RedirectToAction("Login", "Login");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Error", ex.Message);
        }
    }


    [HttpGet("expense-tracker/my-profile")]
    public IActionResult MyProfile()
    {
        try
        {
            UserDetailsVM user = _service.GetUserDetials(HttpContext.Session.GetInt32("UserId"));
            return View(user);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Error");
        }
    }

    [HttpPost]
    public IActionResult UpdateProfile(UserDetailsVM user)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (ModelState.IsValid)
            {
                _service.EditProfile(user, userId);
                TempData["success"] = "Profile has been updated";
                return RedirectToAction(nameof(MyProfile));
            }
            return View("~/Views/Expense/MyProfile.cshtml", user);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Error", ex.Message);
        }
    }

    [HttpPost]
    public IActionResult CreateExpense(HomeVM viewModel)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            _service.AddExpense(viewModel, userId);
            Budget budget = _service.CheckOverBudget(userId);
            if (budget.BudgetId != 0)
            {
                _service.SendMailForOverBudget(budget, userId);
            }
            TempData["success"] = "Expense has been Added";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Error", ex.Message);
        }
    }

    [HttpGet]
    public IActionResult ShowExpenseModal(int id)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            HomeVM data = _service.GetExpenseData(id, userId);
            return PartialView("_EditExpense", data);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Error", ex.Message);
        }
    }

    [HttpPost]
    public IActionResult EditExpense(HomeVM model)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            _service.EditExpense(model, userId);
            Budget budget = _service.CheckOverBudget(userId);
            if (budget.BudgetId != 0)
            {
                _service.SendMailForOverBudget(budget, userId);
            }
            TempData["success"] = "Expense has been Updated";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Error", ex.Message);
        }
    }

    [HttpGet]
    public IActionResult DeleteExpense(int id)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            _service.DeleteExpense(id, userId);
            TempData["success"] = "Expense has been Deleted";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Error", ex.Message);
        }
    }

    [HttpGet]
    public IActionResult ExpenseTable(int categoryId, int currentPage, int itemsPerPage, bool orderByDate, bool orderByAmount, string search)
    {
        try
        {
            var userId = (int?)HttpContext.Session.GetInt32("UserId") ?? 0;
            HomeVM viewModel = _service.GetExpenses(categoryId, userId, currentPage, itemsPerPage, orderByDate, orderByAmount, search);
            ViewBag.Page = currentPage;
            return PartialView("_ExpenseTable", viewModel);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Error", ex.Message);
        }
    }


    [HttpGet]
    public IActionResult ShowDeleteExpenseModal(int id)
    {
        return PartialView("_Delete", id);
    }

    public IActionResult LazyLoading()
    {
        try
        {
            var userId = (int?)HttpContext.Session.GetInt32("UserId") ?? 0;
            var number = 1;
            HomeVM viewModel = _service.GetExpenses(0, userId, number, 5, false, false, "");
            return View(viewModel);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Error", ex.Message);
        }
    }

    public IActionResult ExpenseTableWithLazyLoading(int categoryId, int currentPage, int itemsPerPage, bool orderByDate, bool orderByAmount, string search)
    {
        try
        {
            var userId = (int?)HttpContext.Session.GetInt32("UserId") ?? 0;
            HomeVM viewModel = _service.GetExpenses(categoryId, userId, currentPage, itemsPerPage, orderByDate, orderByAmount, search);
            return Json(viewModel);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "An error occurred: " + ex.Message;
            return View("Error");
        }
    }


}
