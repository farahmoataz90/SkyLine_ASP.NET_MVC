using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkyLine.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage ="You have to provide a valid full name.")]
        [MinLength(10, ErrorMessage = "Full name mustn't be less than 10 characters.")]
        [MaxLength(50, ErrorMessage = "Full name mustn't exceed 50 characters.")]
        public string FullName { get; set; }

        [DisplayName("Occupation")]
        [Required(ErrorMessage ="You have to provide a valid position.")]
        [MinLength(2, ErrorMessage = "Position mustn't be less than 2 characters.")]
        [MaxLength(20, ErrorMessage = "Position mustn't exceed 20 characters.")]
        public string Position { get; set; }

        [Range(3500, 35000, ErrorMessage = "Salary must be between 3500 EGP and 35000 EGP.")]
        public decimal Salary { get; set; }

        [Range(0, 100, ErrorMessage = "Appraisal must be between 0 and 100.")]
        public byte Appraisal { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [DisplayName("Join Date and Time")]
        public DateTime JoinDateTime { get; set;}
        public bool IsActive { get; set; }

        [DataType(DataType.Time)]
        public DateTime AttendanceTime { get; set; }

        [DisplayName("Phone")]
        [RegularExpression("^0\\d{10}$", ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Password)]
        public string Code { get; set; }

        [NotMapped]
        [Compare("Code", ErrorMessage = "Code and confirm code do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmCode { get; set; }

        [DisplayName("Department")]
        [Range(1, int.MaxValue, ErrorMessage = "Choose a valid department.")]
        public int DepartmentId { get; set; }

        // Navigation Property
        [ValidateNever]
        public Department Department { get; set; }

        [DisplayName("Image")]
        [ValidateNever]
        public string? ImagePath { get; set; }
    }
}
