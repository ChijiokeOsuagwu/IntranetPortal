using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.UserAdministration.Models
{
    public class EditUserAccountViewModel : BaseViewModel
    {
        [Required]
        public string UserID { get; set; }

        [Required]
        public string FullName { get; set; }

        public string UserType { get; set; }

        [Required]
        public string OldLoginID { get; set; }

        [Required(ErrorMessage = "Login ID is required.")]
        [Display(Name = "Login ID")]
        [MaxLength(150, ErrorMessage = "Login ID must not exceed 150 characters.")]
        public string LoginID { get; set; }

        [Display(Name = "Lock Status")]
        public bool EnableLockOut { get; set; }

        [Display(Name ="Unlock User On")]
        [DataType(DataType.DateTime)]
        public DateTime? LockOutEndDate { get; set; }
    }
}
