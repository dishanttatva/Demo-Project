using ExpenseTracker.AuthMIddleware;
using ExpenseTrackerEntity.Models;
using ExpenseTrackerEntity.ViewModel;
using ExpenseTrackerService.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    [CustomeAuthorize()]
    public class RecurrenceController : Controller
    {
        private readonly IExpenseService _service;
        public RecurrenceController(IExpenseService service)
        {
            _service = service;
        }
        [HttpGet("expense-tracker/recurrence")]
        public IActionResult RecurenceExpense()
        {
            try
            {
                int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                List<Category> categories = _service.GetCategories(userId);
                List<Freequency> frequencies = _service.GetFrequencies();
                RecurrenceVM data = new() { Categories = categories, Freequencies = frequencies };
                return View(data);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error", ex.Message);
            }
        }
        [HttpGet]
        public IActionResult ShowRecurrenceModal(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                RecurrenceVM data = _service.GetRecurrenceData(id, userId);
                return PartialView("_EditRecurrence", data);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error", ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CreateRecurenceExpense(RecurrenceVM vm)
        {
            try
            {
                int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                _service.AddRecurrenceExpense(vm, userId);
                TempData["success"] = "Recurrence has been created";
                return RedirectToAction(nameof(RecurenceExpense));
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error", ex.Message);
            }
        }

        [HttpGet]
        public IActionResult RecurrenceTable(int categoryId, int currentPage, int itemsPerPage, bool orderByDate, bool orderByAmount,int frequency, string search)
        {
            try
            {
                var userId = (int?)HttpContext.Session.GetInt32("UserId") ?? 0;
                RecurrenceVM viewModel = _service.GetRecurrences(categoryId, userId, currentPage, itemsPerPage, orderByDate, orderByAmount, frequency, search);
                ViewBag.Page = currentPage;
                return PartialView("_RecurrenceTable", viewModel);
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error", ex.Message);
            }
        }
        [HttpPost]
        public IActionResult EditRecurrence(RecurrenceVM model)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                _service.EditRecurrence(model, userId);
                TempData["success"] = "Recurrence has been Updated";
                return RedirectToAction(nameof(RecurenceExpense));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error", ex.Message);
            }
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
