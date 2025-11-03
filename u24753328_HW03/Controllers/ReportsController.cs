using System.Web.Mvc;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using u24753328_HW03.Models;

namespace u24753328_HW03.Controllers
{
    
    public class ReportsController : Controller
    {
        private BikeStoresEntities db = new BikeStoresEntities();

      
        public async Task<ActionResult> Index()
        {
            
            var staffPerformanceQuery = await db.orders
                .Include(o => o.staff)
                .SelectMany(o => o.order_items, (o, oi) => new { o.staff, oi.quantity })
                .GroupBy(x => new { x.staff.staff_id, x.staff.first_name, x.staff.last_name })
                .Select(g => new
                {
                    StaffName = g.Key.first_name + " " + g.Key.last_name,
                    TotalUnitsSold = g.Sum(x => x.quantity)
                })
                .OrderByDescending(x => x.TotalUnitsSold)
                .ToListAsync();

            var staffRanking = staffPerformanceQuery
                .Select((x, index) => new StaffPerformanceViewModel
                {
                    Rank = index + 1,
                    StaffName = x.StaffName,
                    TotalUnitsSold = x.TotalUnitsSold
                })
                .ToList();



            int targetYear = 2018;
            var brandSalesData = await db.order_items
                .Include(oi => oi.order)
                .Include(oi => oi.product.brand)
                .Where(oi => oi.order.order_date.Year == targetYear)
                .GroupBy(oi => oi.product.brand.brand_name)
                .Select(g => new BrandSalesChartData
                {
                    BrandName = g.Key,
                    TotalRevenue = g.Sum(oi => (decimal)oi.quantity * oi.list_price * (1 - (decimal)oi.discount))
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToListAsync();


          
            List<SavedReport> documentArchive = new List<SavedReport>();
            try
            {
                
                documentArchive = await db.SavedReport.ToListAsync();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading archive: {ex.Message}");
            }

            
            var viewModel = new ReportIndexViewModel
            {
                StaffRanking = staffRanking,
                BrandSalesData = brandSalesData,
                DocumentArchive = documentArchive,
            };

            return View(viewModel);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveReport(string fileName, string fileType)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(fileType))
            {
                TempData["Message"] = "Error: File name and type are required.";
                return RedirectToAction("Index");
            }

            var report = new SavedReport
            {
                FileName = fileName,
                FileType = fileType,
                DateSaved = System.DateTime.Now,
                Description = "No description added yet.",
            };

            db.SavedReport.Add(report);
            await db.SaveChangesAsync();

            TempData["Message"] = $"Report '{fileName}' saved successfully as {fileType}.";
            return RedirectToAction("Index");
        }

       
        public ActionResult DownloadReport(int id)
        {
            TempData["Message"] = "Placeholder: Report downloaded successfully.";
            return RedirectToAction("Index");
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteReport(int id)
        {
            var report = await db.SavedReport.FindAsync(id);
            if (report == null)
            {
                TempData["Message"] = "Error: Report not found.";
                return RedirectToAction("Index");
            }

            db.SavedReport.Remove(report);
            await db.SaveChangesAsync();

            TempData["Message"] = $"Report '{report.FileName}' deleted successfully.";
            return RedirectToAction("Index");
        }

        //  (BONUS MARKS)
        [HttpPost]
        public async Task<JsonResult> UpdateReportDescription(int id, string description)
        {
            var report = await db.SavedReport.FindAsync(id);

            if (report == null)
            {
                return Json(new { success = false, message = "Report not found." });
            }

            report.Description = description;
            db.Entry(report).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (System.Exception)
            {
                return Json(new { success = false, message = "Database save failed." });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}