using ExpenseTrackerEntity.Models;
using ExpenseTrackerEntity.ViewModel;
using ExpenseTrackerService.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IExpenseService _service;
        public CategoryController(IExpenseService service)
        {
            _service = service;
        }
        [HttpGet("expense-tracker/categories")]
        public IActionResult Categories()
        {
            
            return View();
        }
        [HttpGet]
        public IActionResult CategoryTable(int currentPage, int itemsPerPage,string search) 
        {
            var userId = (int?)HttpContext.Session.GetInt32("UserId") ?? 0;
            CategoryVM categories = _service.GetCategoriesForTable(userId,currentPage,itemsPerPage,search);
            ViewBag.Page = currentPage;
            return PartialView("_CategoryTable", categories);
        }

        [HttpPost("expense-tracker/create-categories")]
        public IActionResult CreateCategory(CategoryVM viewModel)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            bool flag = _service.CreateCategory(viewModel.CategoryName, userId);
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
            var userId = HttpContext.Session.GetInt32("UserId");
            _service.DeleteCategory(id, userId);
            TempData["success"] = "Category has been deleted";
            return RedirectToAction(nameof(Categories));
        }

        [HttpGet]
        public IActionResult ShowEditCategoryModal(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            CategoryVM category = _service.GetCategory(id, userId);
            return PartialView("_EditCategory", category);
        }

        [HttpPost]
        public IActionResult EditCategory(CategoryVM model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            bool flag = _service.EditCategory(model, userId);
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
    }
}
