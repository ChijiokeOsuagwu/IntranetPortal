using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Models
{
    public class ChangePasswordViewModel : BaseViewModel
    {
        [Required]
        public string UserID { get; set; }

        [Display(Name="Full Name")]
        public string UserFullName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name="Enter New Password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword))]
        [Display(Name="Confirm New Password")]
        public string ConfirmPassword { get; set; }
    }
}
