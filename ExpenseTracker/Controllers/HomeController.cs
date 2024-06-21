using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ExpenseTrackerEntity.Models;
using ExpenseTrackerService.Interface;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracker.AuthMIddleware;
using ExpenseTrackerEntity.ViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using iText.Kernel.Pdf;
using iText.Layout.Properties;
using iText.IO.Image;

using iText.Layout.Element;
using Microsoft.AspNetCore.Hosting.Server;

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
    public IActionResult ExpenseTable(int categoryId,int currentPage, int ItemsPerPage,bool OrderByDate,bool OrderByAmount,string search)
    {
        var userId = (int?)HttpContext.Session.GetInt32("UserId")??0;
        HomeVM viewModel= _service.GetExpenses(categoryId, userId,currentPage,ItemsPerPage,OrderByDate,OrderByAmount,search);
        ViewBag.Page = currentPage;
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
    public IActionResult BarGraph(int id)
    {
        var UserId = (int?)HttpContext.Session.GetInt32("UserId") ?? 0;
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
        return PartialView("_BarGraph", salesData);
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
        HomeVM viewModel = _service.GetExpenses(0, userId, number, 5, false, false,"");
        return View(viewModel);
    }

    public IActionResult ExpenseTableWithLazyLoading(int categoryId, int currentPage, int ItemsPerPage, bool OrderByDate, bool OrderByAmount,string search)
    {
        var userId = (int?)HttpContext.Session.GetInt32("UserId")??0;
        HomeVM viewModel = _service.GetExpenses(categoryId, userId, currentPage, ItemsPerPage, false, false,search);
       // ViewBag.Page = CurrentPage;
        return Json(viewModel);
    }

    [HttpPost]
    public  IActionResult SaveChartImages(string dataURL, string dataURL2)
    {
        var base64Data = dataURL.Split(',')[1];
        var imageBytes = Convert.FromBase64String(base64Data);

        // Save the image to a file (optional: adjust the file path and format)
        var imagePath = Path.Combine("wwwroot", "images", "chart2.png");
         System.IO.File.WriteAllBytes(imagePath, imageBytes);

        // Return a success message or the image file path
        return RedirectToAction(nameof(DownloadPdf), new { imagePath = imagePath });
    }
    [HttpGet]
    public IActionResult DownloadPdf(string imagePath)
    {
        byte[] data = _service.GeneratePdf(imagePath);
        
       // Response.Headers.Add("Content-Disposition", "inline; filename=Chart.pdf");
        return File(data, "application/pdf", "Chart.pdf");
    }
}
