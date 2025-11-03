using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u24753328_HW03.Models
{
	public class OrderReportViewModel
    {
        public int OrderId { get; set; }
        public System.DateTime OrderDate { get; set; }
        public int CustomerName { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
    }
}