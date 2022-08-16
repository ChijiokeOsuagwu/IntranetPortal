using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.UserAdministration.Models
{
    public class PasswordResetViewModel : BaseViewModel
    {
        [Required]
        public string UserID { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "User Type")]
        public string UserType { get; set; }

        [Required(ErrorMessage = " Password is required.")]
        [Display(Name = "Password")]
        [MaxLength(250, ErrorMessage = "Password must not exceed 250 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = " Confirm Password is required.")]
        [Display(Name = "Confirm Password")]
        [MaxLength(250, ErrorMessage = "Password must not exceed 250 characters.")]
        [Compare(nameof(Password), ErrorMessage = "Password does not  match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
