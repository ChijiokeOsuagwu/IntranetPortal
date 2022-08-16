using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.UserAdministration.Models
{
    public class EmployeeUserViewModel : BaseViewModel
    {
        [Required]
        public string UserID { get; set; }
        public string CompanyCode { get; set; }

        [Required(ErrorMessage = "Employee Number is required.")]
        [Display(Name = "Employee Number")]
        public string EmployeeNumber { get; set; }

        [Required(ErrorMessage ="Full Name is required.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Login ID is required.")]
        [Display(Name = "Login ID")]
        [MaxLength(150, ErrorMessage = "Login ID must not exceed 150 characters.")]
        public string LoginID { get; set; }

        [Required(ErrorMessage = " Password is required.")]
        [Display(Name = "Password")]
        [MaxLength(250, ErrorMessage = "Password must not exceed 250 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = " Confirm Password is required.")]
        [Display(Name = "Confirm Password")]
        [MaxLength(250, ErrorMessage = "Password must not exceed 250 characters.")]
        [Compare(nameof(Password), ErrorMessage ="Password does not  match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name ="Activation Option")]
        public bool EnableLockOut { get; set; }
    }
}
