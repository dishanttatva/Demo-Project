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
        int userId = _service.ValidateToken(Request.Cookies["myToken"] ?? "");
        HttpContext.Session.SetInt32("UserId", userId);
        List<Category> categories= _service.GetCategories(userId);
        HomeVM data = new() { Categories = categories };
        return View(data);
    }


    [HttpGet]
    public IActionResult LogOut()
    {
        Response.Cookies.Delete("myToken");
        TempData["success"] = "LogOut successful";
        return RedirectToAction("Login", "Login");
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
        var userId = HttpContext.Session.GetInt32("UserId");
        if (ModelState.IsValid)
        {
            _service.EditProfile(user, userId);
            TempData["success"] = "Profile has been updated";
            return RedirectToAction(nameof(MyProfile));
        }
        return View("~/Views/Expense/MyProfile.cshtml", user);
    }

    [HttpPost]
    public IActionResult CreateExpense(HomeVM viewModel)
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

    [HttpGet]
    public IActionResult ShowExpenseModal(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        HomeVM data = _service.GetExpenseData(id, userId);
        return PartialView("_EditExpense", data);
    }
   
    [HttpPost]
    public IActionResult EditExpense(HomeVM model)
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

    [HttpGet]
    public IActionResult DeleteExpense(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        _service.DeleteExpense(id, userId);
        TempData["success"] = "Expense has been Deleted";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult ExpenseTable(int categoryId, int currentPage, int itemsPerPage, bool orderByDate, bool orderByAmount, string search)
    {
        var userId = (int?)HttpContext.Session.GetInt32("UserId") ?? 0;
        HomeVM viewModel = _service.GetExpenses(categoryId, userId, currentPage, itemsPerPage, orderByDate, orderByAmount, search);
        ViewBag.Page = currentPage;
        return PartialView("_ExpenseTable", viewModel);
    }

   
    [HttpGet]
    public IActionResult ShowDeleteExpenseModal(int id)
    {
        return PartialView("_Delete", id);
    }
    public IActionResult LazyLoading()
    {
        var userId = (int?)HttpContext.Session.GetInt32("UserId") ?? 0;
        var number = 1;
        HomeVM viewModel = _service.GetExpenses(0, userId, number, 5, false, false, "");
        return View(viewModel);
    }

    public IActionResult ExpenseTableWithLazyLoading(int categoryId, int currentPage, int itemsPerPage, bool orderByDate, bool orderByAmount, string search)
    {
        try
        {
            var userId = (int?)HttpContext.Session.GetInt32("UserId") ?? 0;
            HomeVM viewModel = _service.GetExpenses(categoryId, userId, currentPage, itemsPerPage, orderByDate, orderByAmount, search);
            return Json(viewModel);
        }
        catch(Exception ex)
        {
            ViewBag.ErrorMessage = "An error occurred: " + ex.Message;
            return View("Error");
        }
    }

    [HttpGet]
    public IActionResult Budget()
    {
        var userId = (int?)HttpContext.Session.GetInt32("UserId") ?? 0;
        List<Category> categories = _service.GetCategories(userId);
        List<Freequency> freequencies = _service.GetFreequencies();
        BudgetVM budgetVM = new BudgetVM() { Categories=categories, Freequencies=freequencies};
        return View(budgetVM);
    }

    [HttpPost]
    public IActionResult CreateBudget(BudgetVM budgetVM)
    {
        var userId = (int?)HttpContext.Session.GetInt32("UserId") ?? 0;
        _service.CreateBudget(budgetVM, userId);
        return RedirectToAction(nameof(Budget));
    }
    [HttpGet]
    public IActionResult BudgetTable(int currentPage,int itemsPerPage,bool orderByAmount)
    {
        var userId = (int?)HttpContext.Session.GetInt32("UserId") ?? 0;
        BudgetVM data = _service.GetBudgetsData(userId,currentPage,itemsPerPage,orderByAmount);
        ViewBag.Page = currentPage;
        return PartialView("_BudgetTable",data);
    }
    public IActionResult ShowBudgetModal(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        BudgetVM data = _service.GetBudget(id, userId);
        return PartialView("_EditBudget", data);
    }
    [HttpPost]
    public IActionResult EditBudget(BudgetVM budgetVM)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        _service.EditBudget(budgetVM, userId);
        return RedirectToAction(nameof(Budget));
    }
    public IActionResult DeleteBudget(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        _service.DeleteBudget(id, userId);
        return RedirectToAction(nameof(Budget));
    }
    public IActionResult ShowDeleteBudgetModal(int id)
    {
        return PartialView("_DeleteBudget", id);
    }
}
