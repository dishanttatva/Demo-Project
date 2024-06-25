using ExpenseTrackerEntity.ViewModel;
using ExpenseTrackerService.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    
    public class RecurrenceController : Controller
    {
        private readonly IExpenseService _service;
        public RecurrenceController(IExpenseService service)
        {
            _service = service;
        }
        [HttpGet]
        public IActionResult RecurenceExpense()
        {
            int UserId = HttpContext.Session.GetInt32("UserId") ?? 0;
            HomeVM data = _service.GetCategories(UserId);
            return View(data);
        }
        [HttpGet]
        public IActionResult ShowRecurrenceModal(int id)
        {
            var UserId = HttpContext.Session.GetInt32("UserId");
            HomeVM data = _service.GetRecurrenceData(id, UserId);
            return PartialView("_EditRecurrence", data);
        }

        [HttpPost]
        public IActionResult CreateRecurenceExpense(HomeVM vm)
        {
            int UserId = HttpContext.Session.GetInt32("UserId") ?? 0;
            _service.AddRecurrenceExpense(vm, UserId);
            TempData["success"] = "Recurrence has been created";
            return RedirectToAction(nameof(RecurenceExpense));
        }

        [HttpGet]
        public IActionResult RecurrenceTable(int categoryId, int currentPage, int ItemsPerPage, bool OrderByDate, bool OrderByAmount, string search)
        {
            var userId = (int?)HttpContext.Session.GetInt32("UserId") ?? 0;
            HomeVM viewModel = _service.GetRecurrences(categoryId, userId, currentPage, ItemsPerPage, OrderByDate, OrderByAmount, search);
            ViewBag.Page = currentPage;
            return PartialView("_RecurrenceTable", viewModel);
        }
        [HttpPost]
        public IActionResult EditRecurrence(HomeVM model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            _service.EditRecurrence(model, userId);
            TempData["success"] = "Recurrence has been Updated";
            return RedirectToAction(nameof(RecurenceExpense));
        }
        public IActionResult ShowDeleteRecurrenceModal(int id)
        {
            return PartialView("_DeleteRecurrence", id);
        }
        [HttpGet]
        public IActionResult DeleteRecurrence(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            _service.DeleteRecurrence(id, userId);
            TempData["success"] = "Recurrence has been Deleted";
            return RedirectToAction(nameof(RecurenceExpense));
        }
    }
}
