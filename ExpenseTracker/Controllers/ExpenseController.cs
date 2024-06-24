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

    [HttpGet("expense-tracker/my-trends")]
    public IActionResult Chart()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ChartGraph(int id)
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

        return PartialView("_ChartGraph", salesData);
    }

    [HttpGet]
    public IActionResult PieGraph(int id)
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

    [HttpPost]
    public IActionResult SaveChartImages(string dataURL, string dataURL2)
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
        return File(data, "application/pdf", "Chart.pdf");
    }
    [HttpGet]
    public IActionResult SplitExpense()
    {
        return View();
    }
    public IActionResult ValidateEmail(string email)
    {
        bool isValidateEmail = _service.ValidateEmail(email);
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
        if(!email.Contains("@") || !email.Contains('.'))
        {
            TempData["error"] = "Email is not valid";
            return Ok();
        }
        _service.QuickRegister(email, password, firstname, dateofbirth);
        _service.SendMailForCreateAccount(email, password);
        TempData["success"] = "Account created successfully";
        return RedirectToAction(nameof(SplitExpense));
    }
    [HttpPost]
    public IActionResult SplitExpense(SpliteExpenseVM vm)
    {

        bool isValidate = _service.ValidateEmails(vm);
        if (vm.SplittedAmount == 0)
        {
            TempData["error"] = "Split amount should not 0";
            return RedirectToAction(nameof(SplitExpense));
        }
        if(isValidate)
        {
            _service.SendMailForSplitAmount(vm.Emails,vm.SplittedAmount,vm.Totals);
            _service.SplitExpense(vm.Emails, vm.SplittedAmount,vm.Totals);
            TempData["success"]="Split Expense has been created";
        }
        else
        {
            TempData["error"] = "Please Validate all the users";
        }
        return RedirectToAction(nameof(SplitExpense));
    }
}
