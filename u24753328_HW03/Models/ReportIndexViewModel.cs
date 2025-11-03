using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace u24753328_HW03.Models
{
    // === 1. Staff Performance Report Data (Required) ===
    public class StaffPerformanceViewModel
    {
        [Display(Name = "Rank")]
        public int Rank { get; set; }

        [Display(Name = "Staff Name")]
        public string StaffName { get; set; }

        [Display(Name = "Total Units Sold")]
        public int TotalUnitsSold { get; set; }
    }

    // === 2. Sales by Brand Chart Data (Bar Chart - Best Sellers) ===
    public class BrandSalesChartData
    {
        public string BrandName { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    // === 3. Document Archive Entity (Database Model) ===
    public class SavedReport
    {
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FileType { get; set; }

        public DateTime DateSaved { get; set; }

    
        public string Description { get; set; }
    }

    
    public class ReportIndexViewModel
    {
        public List<StaffPerformanceViewModel> StaffRanking { get; set; }
        public List<BrandSalesChartData> BrandSalesData { get; set; }
        public List<SavedReport> DocumentArchive { get; set; }
    }
}