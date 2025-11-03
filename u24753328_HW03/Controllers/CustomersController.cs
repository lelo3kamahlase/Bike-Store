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
    public class CustomersController : Controller
    {
        private BikeStoresEntities db = new BikeStoresEntities();
        private const int PageSize = 10;

        public async Task<ActionResult> Index(int? page)
        {
            var customerQuery = db.customers.AsQueryable();

            int pageNumber = (page ?? 1);
            int totalRecords = await customerQuery.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

            var pagedCustomers = await customerQuery
                                            .OrderBy(c => c.last_name)
                                            .Skip((pageNumber - 1) * PageSize)
                                            .Take(PageSize)
                                            .ToListAsync();

            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = totalPages;

            return View(pagedCustomers);
        }

        
        public ActionResult Create()
        {
            return PartialView("_CreateCustomerModal");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "first_name,last_name,phone,email,street,city,state,zip_code")] customer customer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.customers.Add(customer);
                    await db.SaveChangesAsync();

                    return Json(new { success = true, message = "Customer created successfully." });
                }
                catch (Exception ex)
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json(new { success = false, message = "Error saving data: " + ex.Message });
                }
            }

           
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return PartialView("_CreateCustomerModal", customer);
        }

        
        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            customer customer = await db.customers.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return PartialView("_EditCustomerModal", customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "customer_id,first_name,last_name,phone,email,street,city,state,zip_code")] customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return Json(new { success = true, message = "Customer updated successfully." });
            }

            
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Content(RenderPartialViewToString("_EditCustomerModal", customer));
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            customer customer = await db.customers.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return PartialView("_DeleteCustomerModal", customer);
        }

      
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteConfirmed(int id)
        {
            customer customer = await db.customers.FindAsync(id);
            if (customer == null)
            {
                return Json(new { success = false, message = "Customer not found." });
            }

            db.customers.Remove(customer);
            await db.SaveChangesAsync();

            return Json(new { success = true, message = "Customer deleted successfully." });
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