using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ExpenseTrackerEntity.Models;
using ExpenseTrackerService.Interface;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracker.AuthMIddleware;
using ExpenseTrackerEntity.ViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ExpenseTracker.Controllers;
[CustomeAuthorize()]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IExpenseService _service;

    public HomeController(ILogger<HomeController> logger, IExpenseService service)
    {
        _logger = logger;
        _service = service;
    }
    [HttpGet("expense-tracker/home")]
    public IActionResult Index()
    {
            int UserId = _service.ValidateToken(Request.Cookies["myToken"]??"");
            HttpContext.Session.SetInt32("UserId", UserId);
            HomeVM data = _service.GetCategories(UserId);
            return View(data);
    }

    [HttpGet]
    public IActionResult LogOut()
    {
        Response.Cookies.Delete("myToken");
        TempData["success"] = "LogOut successful";
        return RedirectToAction("Login","Login");
    }
    [HttpGet("expense-tracker/categories")]
    public IActionResult Categories()
    {
        var UserId = (int?)HttpContext.Session.GetInt32("UserId")??0;
        HomeVM viewModel = _service.GetCategories(UserId);
        return View(viewModel);
    }

    [HttpPost("expense-tracker/create-categories")]
    public IActionResult CreateCategory(HomeVM viewModel)
    {
        var UserId = HttpContext.Session.GetInt32("UserId");
        bool flag= _service.CreateCategory(viewModel.CategoryName, UserId);
        if (flag)
        { 
        TempData["success"] = "Category has been created";
        }
        else
        {
            TempData["error"] = "Category already exists";
        }
        return RedirectToAction(nameof(Categories));
    }

    [HttpGet]
   public IActionResult DeleteCategory(int id)
    {
        var UserId = HttpContext.Session.GetInt32("UserId");
        _service.DeleteCategory(id, UserId);
        TempData["success"] = "Category has been deleted";
        return RedirectToAction(nameof(Categories));
    }

    [HttpGet]
    public IActionResult ShowEditCategoryModal(int id)
    {
        var UserId = HttpContext.Session.GetInt32("UserId");
        CategoryVM category= _service.GetCategory(id, UserId);
        return PartialView("_EditCategory", category);
    }

    [HttpPost]
    public IActionResult EditCategory(CategoryVM model)
    {
        var userId= HttpContext.Session.GetInt32("UserId");
        bool flag= _service.EditCategory(model, userId);
        if (flag)
        {
        TempData["success"] = "Category has been updated";
        }
        else
        {
        TempData["error"] = "Category already exists";
        }
        return RedirectToAction(nameof(Categories));
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
        if(ModelState.IsValid)
        {
            _service.EditProfile(user,userId);
            TempData["success"] = "Profile has been updated";
            return RedirectToAction(nameof(MyProfile));
        }
        return View("~/Views/Home/MyProfile.cshtml", user);
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
    public IActionResult ExpenseTable(int categoryId,int currentPage, int ItemsPerPage,bool OrderByDate,bool OrderByAmount)
    {
        var userId = (int?)HttpContext.Session.GetInt32("UserId")??0;
        HomeVM viewModel= _service.GetExpenses(categoryId, userId,currentPage,ItemsPerPage,OrderByDate,OrderByAmount);
        ViewBag.Page = currentPage;
        /// for lazy loading return partial view named _LazyLoading.cshtml
        return PartialView("_ExpenseTable", viewModel);
    }

    [HttpGet("expense-tracker/my-trends")]
    public IActionResult Chart()
    {
        return View();   
    }

    [HttpGet]
    public IActionResult ChartGraph(int id)
    {
        var UserId = (int?)HttpContext.Session.GetInt32("UserId")??0;
        SalesData salesData=new();
        switch (id)
        {
            case 1:  salesData = _service.GetDailyReportData(UserId);
                    salesData.Type = "1";
                    break;
            case 2:  salesData=_service.GetWeeklyReportData(UserId);
                    salesData.Type = "2";
                    break;
            case 3: salesData = _service.GetMonthlyReportData(UserId);
                    salesData.Type = "3";
                    break;
        }
       
        return PartialView("_ChartGraph",salesData);
    }

    [HttpGet]
    public IActionResult PieGraph(int id)
    {
        var UserId = (int?)HttpContext.Session.GetInt32("UserId")??0;
        SalesData salesData = new();
        switch (id)
        {
            case 1:
                salesData = _service.GetDailyReportData(UserId);
                salesData.Type = "1";
                break;
            case 2:
                salesData = _service.GetWeeklyReportData(UserId);
                salesData.Type = "2";
                break;
            case 3:
                salesData = _service.GetMonthlyReportData(UserId);
                salesData.Type = "3";
                break;
        }
        return PartialView("_PieGraph", salesData);
    }
    [HttpGet]
    public IActionResult ShowDeleteExpenseModal(int id)
    {
        return PartialView("_Delete", id);
    }
    public IActionResult LazyLoading()
    {
        var userId = (int?)HttpContext.Session.GetInt32("UserId")??0;
        var number = 1;
        HomeVM viewModel = _service.GetExpenses(0, userId, number, 5, false, false);
        return View(viewModel);
    }

    public IActionResult ExpenseTableWithLazyLoading(int categoryId, int currentPage, int ItemsPerPage, bool OrderByDate, bool OrderByAmount)
    {
        var userId = (int?)HttpContext.Session.GetInt32("UserId")??0;
        HomeVM viewModel = _service.GetExpenses(categoryId, userId, currentPage, ItemsPerPage, false, false);
       // ViewBag.Page = CurrentPage;
        return Json(viewModel);
    }
}
