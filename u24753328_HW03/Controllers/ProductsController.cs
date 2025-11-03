using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using u24753328_HW03.Models;

namespace u24753328_HW03.Controllers
{
    public class ProductsController : Controller
    {
        private BikeStoresEntities db = new BikeStoresEntities();
        private const int PageSize = 10;

        
        public async Task<ActionResult> Index(string brandFilter, string categoryFilter, string searchQuery, int? page)
        {
            
            IQueryable<product> productQuery = db.products
                .Include(p => p.brand)
                .Include(p => p.category);

           
            if (!string.IsNullOrEmpty(brandFilter) && brandFilter != "All Brands")
            {
                
                productQuery = productQuery.Where(p => p.brand.brand_name == brandFilter);
            }

           
            if (!string.IsNullOrEmpty(categoryFilter) && categoryFilter != "All Categories")
            {
              
                productQuery = productQuery.Where(p => p.category.category_name == categoryFilter);
            }

            
            if (!string.IsNullOrEmpty(searchQuery))
            {
                
                productQuery = productQuery.Where(p =>
                    p.product_name.Contains(searchQuery) ||
                    p.brand.brand_name.Contains(searchQuery) ||
                    p.category.category_name.Contains(searchQuery));
            }



            int pageNumber = (page ?? 1);

            
            int totalRecords = await productQuery.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

            
            pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));

            
            var pagedProducts = await productQuery
                .OrderBy(p => p.product_name) 
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(p => new ProductViewModel
                {
                    ProductId = p.product_id,
                    BicycleName = p.product_name,
                    Price = p.list_price,
                    Description = p.model_year.ToString(), 
                    BrandName = p.brand.brand_name,
                    CategoryName = p.category.category_name
                })
                .ToListAsync();

         
            ViewBag.BrandList = await db.brands.Select(b => b.brand_name).Distinct().ToListAsync();
            ViewBag.CategoryList = await db.categories.Select(c => c.category_name).Distinct().ToListAsync();

            
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentBrandFilter = brandFilter;
            ViewBag.CurrentCategoryFilter = categoryFilter;
            ViewBag.CurrentSearchQuery = searchQuery;

            return View(pagedProducts);
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
