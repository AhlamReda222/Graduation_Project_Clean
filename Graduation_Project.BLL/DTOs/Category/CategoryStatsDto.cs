using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Graduation_Project.BLL.DTOs.Category
{
    public class CategoryStatsDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageProductPrice { get; set; }
        public double AverageRating { get; set; }
    }
}