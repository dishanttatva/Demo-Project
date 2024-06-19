using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ExpenseTrackerEntity.Models;
using ExpenseTrackerService.Interface;
// using ExpenseTrackerEntity.ViewModel;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracker.AuthMIddleware;
using ExpenseTrackerEntity.ViewModel;
// using ExpenseTracker.ViewModel;
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
    
    public IActionResult Index()
    {
        if (HttpContext.Request.Cookies.ContainsKey("myToken"))
        {
            int UserId = _service.ValidateToken(Request.Cookies["myToken"]);
            HttpContext.Session.SetInt32("UserId", UserId);
            HomeVM data = _service.GetCategories(UserId);
            return View(data);
        }
        return RedirectToAction("Login","Login");
    }

    public IActionResult Privacy()
    {
        return View();
    }
   
    public IActionResult LogOut()
    {
        Response.Cookies.Delete("myToken");
        TempData["success"] = "LogOut successful";
        return RedirectToAction("Login","Login");
    }
    [HttpGet]
    public IActionResult Categories()
    {
        var UserId = (int)HttpContext.Session.GetInt32("UserId");
        HomeVM viewModel = _service.GetCategories(UserId);
        return View(viewModel);
    }
    public IActionResult CreateCategory(HomeVM viewModel)
    {
        var UserId = HttpContext.Session.GetInt32("UserId");
        bool flag= _service.CreateCategory(viewModel.CategoryName, UserId);
        TempData["success"] = "Category has been created";
        return RedirectToAction(nameof(Categories));
    }
   public IActionResult DeleteCategory(int id)
    {
        var UserId = HttpContext.Session.GetInt32("UserId");
        _service.DeleteCategory(id, UserId);
        TempData["success"] = "Category has been deleted";
        return RedirectToAction(nameof(Categories));
    }
    public IActionResult ShowModal(int id)
    {
        var UserId = HttpContext.Session.GetInt32("UserId");
        Category category= _service.GetCategory(id, UserId);
        return PartialView("_EditCategory", category);
    }
    [HttpPost]
    public IActionResult EditCategory(Category model)
    {
        var userId= HttpContext.Session.GetInt32("UserId");
        _service.EditCategory(model, userId);
        TempData["success"] = "Category has been updated";
        return RedirectToAction(nameof(Categories));
    }
    [HttpGet]
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

    public IActionResult DeleteExpense(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        _service.DeleteExpense(id, userId);
        TempData["success"] = "Expense has been Deleted";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult ExpenseTable(int categoryId,int CurrentPage, int ItemsPerPage,bool OrderByDate,bool OrderByAmount)
    {
        var userId = (int)HttpContext.Session.GetInt32("UserId");
        HomeVM viewModel= _service.GetExpenses(categoryId, userId,CurrentPage,ItemsPerPage,OrderByDate,OrderByAmount);
        ViewBag.Page = CurrentPage;
        return PartialView("_ExpenseTable", viewModel);
    }
    public IActionResult Chart()
    {
        //var UserId = (int)HttpContext.Session.GetInt32("UserId");
        //SalesData salesData = _service.GetDailyReportData(UserId);
        return View();   
    }

    public IActionResult ChartGraph(int id)
    {
        var UserId = (int)HttpContext.Session.GetInt32("UserId");
        SalesData salesData=new();
        switch (id)
        {
            case 1:  salesData = _service.GetDailyReportData(UserId);
                    salesData.Type = "1";
                    break;
            case 3: salesData = _service.GetMonthlyReportData(UserId);
                    salesData.Type = "3";
                    break;
        }
       
        return PartialView("_ChartGraph",salesData);
    }


}
