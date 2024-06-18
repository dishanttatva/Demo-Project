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
            //data.Expenses = _service.GetExpenses(UserId);
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
        return RedirectToAction(nameof(Index));
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
        _service.CreateCategory(viewModel.CategoryName, UserId);
        return RedirectToAction(nameof(Categories));
    }
   public IActionResult DeleteCategory(int id)
    {
        var UserId = HttpContext.Session.GetInt32("UserId");
        _service.DeleteCategory(id, UserId);
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
            return RedirectToAction(nameof(MyProfile));
        }
        return View("~/Views/Home/MyProfile.cshtml", user);
    }
    [HttpPost]
    public IActionResult CreateExpense(HomeVM viewModel) 
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (ModelState.IsValid)
        {
            _service.AddExpense(viewModel, userId);
            return RedirectToAction(nameof(Index));
        }
        return View("~/Views/Home/Index.cshtml", viewModel);
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
        return RedirectToAction(nameof(Index));
    }

    public IActionResult DeleteExpense(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        _service.DeleteExpense(id, userId);
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public IActionResult ExpenseTable(int categoryId)
    {
        var userId = (int)HttpContext.Session.GetInt32("UserId");
        HomeVM viewModel= _service.GetExpenses(categoryId, userId);
       
        return PartialView("_ExpenseTable", viewModel);
    }
}
