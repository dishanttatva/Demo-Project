using ExpenseTrackerEntity.ViewModel;
using ExpenseTrackerService.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    public class ReportController : Controller
    {
        private readonly IExpenseService _service;
        public ReportController(IExpenseService service)
        {
            _service = service;
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
    }
}
