using ExpenseTracker.AuthMIddleware;
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
        int UserId = _service.ValidateToken(Request.Cookies["myToken"] ?? "");
        HttpContext.Session.SetInt32("UserId", UserId);
        HomeVM data = _service.GetCategories(UserId);
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
        UserDetailsVM user = _service.GetUserDetials(HttpContext.Session.GetInt32("UserId"));
        return View(user);
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
        TempData["success"] = "Expense has been Added";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult ShowExpenseModal(int id)
    {
        var UserId = HttpContext.Session.GetInt32("UserId");
        HomeVM data = _service.GetExpenseData(id, UserId);
        return PartialView("_EditExpense", data);
    }
   
    [HttpPost]
    public IActionResult EditExpense(HomeVM model)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        _service.EditExpense(model, userId);
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
    public IActionResult ExpenseTable(int categoryId, int currentPage, int ItemsPerPage, bool OrderByDate, bool OrderByAmount, string search)
    {
        var userId = (int?)HttpContext.Session.GetInt32("UserId") ?? 0;
        HomeVM viewModel = _service.GetExpenses(categoryId, userId, currentPage, ItemsPerPage, OrderByDate, OrderByAmount, search);
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

    public IActionResult ExpenseTableWithLazyLoading(int categoryId, int currentPage, int ItemsPerPage, bool OrderByDate, bool OrderByAmount, string search)
    {
        var userId = (int?)HttpContext.Session.GetInt32("UserId") ?? 0;
        HomeVM viewModel = _service.GetExpenses(categoryId, userId, currentPage, ItemsPerPage, false, false, search);
        // ViewBag.Page = CurrentPage;
        return Json(viewModel);
    }

    
   
    
  
   
}
