using System;
using System.Data.Entity;
using System.IO; 
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using u24753328_HW03.Models;

namespace u24753328_HW03.Controllers
{
    public class StaffsController : Controller
    {
        private BikeStoresEntities db = new BikeStoresEntities();
        private const int PageSize = 10;

  
        public async Task<ActionResult> Index(int? page)
        {
            var staffQuery = db.staffs.AsQueryable();

            int pageNumber = (page ?? 1);
            int totalRecords = await staffQuery.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

            var pagedStaff = await staffQuery
                                            .OrderBy(s => s.last_name)
                                            .Skip((pageNumber - 1) * PageSize)
                                            .Take(PageSize)
                                            .ToListAsync();

            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = totalPages;

            return View(pagedStaff);
        }

       
        [HttpGet]
        public ActionResult Create()
        {
            
            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name");
            ViewBag.manager_id = new SelectList(db.staffs.Where(s => s.active == 1), "staff_id", "last_name");

            return PartialView("_CreateStaffModal");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "first_name,last_name,email,phone,manager_id,store_id")] staff staff)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    staff.active = 1; 
                    db.staffs.Add(staff);
                    await db.SaveChangesAsync();

                    return Json(new { success = true, message = "Staff member created successfully." });
                }
                catch (Exception ex)
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json(new { success = false, message = "Error saving data: " + ex.Message });
                }
            }

            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name", staff.store_id);
            ViewBag.manager_id = new SelectList(db.staffs.Where(s => s.active == 1), "staff_id", "last_name", staff.manager_id);

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return PartialView("_CreateStaffModal", staff);
        }

       
        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            staff staff = await db.staffs.FindAsync(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
         
            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name", staff.store_id);
            ViewBag.manager_id = new SelectList(db.staffs.Where(s => s.active == 1), "staff_id", "last_name", staff.manager_id);

            return PartialView("_EditStaffModal", staff);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "staff_id,first_name,last_name,email,phone,active,manager_id,store_id")] staff staff)
        {
            if (ModelState.IsValid)
            {
                db.Entry(staff).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return Json(new { success = true, message = "Staff member updated successfully." });
            }

           
            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name", staff.store_id);
            ViewBag.manager_id = new SelectList(db.staffs.Where(s => s.active == 1), "staff_id", "last_name", staff.manager_id);

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Content(RenderPartialViewToString("_EditStaffModal", staff));
        }

        
        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            staff staff = await db.staffs.FindAsync(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return PartialView("_DeleteStaffModal", staff);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteConfirmed(int id)
        {
            staff staff = await db.staffs.FindAsync(id);
            if (staff == null)
            {
                return Json(new { success = false, message = "Staff member not found." });
            }

            db.staffs.Remove(staff);
            await db.SaveChangesAsync();

            return Json(new { success = true, message = "Staff member deleted successfully." });
        }

        
        private string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
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