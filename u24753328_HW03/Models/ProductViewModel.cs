using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using u24753328_HW03.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace u24753328_HW03.Models

{ 
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        [Display(Name = "Bicycle Name")]
        public string BicycleName { get; set; }

        [Display(Name = "List Price")]
        public decimal Price { get; set; }

        [Display(Name = "Model Year")] 
        public string Description { get; set; } 

        [Display(Name = "Brand")]
        public string BrandName { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }
    }
    
}
