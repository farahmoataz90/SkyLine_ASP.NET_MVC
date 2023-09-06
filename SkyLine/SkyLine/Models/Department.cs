using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SkyLine.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal AnnualBudget { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation Property
        [ValidateNever]
        public List<Employee> Employees { get; set; }
    }
}
