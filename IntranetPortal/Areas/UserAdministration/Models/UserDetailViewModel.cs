using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.UserAdministration.Models
{
    public class UserDetailViewModel : BaseViewModel
    {
        public string UserID { get; set; }

        [Display(Name="Login ID")]
        public string Username { get; set; }

        [Display(Name ="Full Name")]
        public string FullName { get; set; }

        [Display(Name = "User Type")]
        public string UserType { get; set; }

        [Display(Name = "Company Code")]
        public string Company { get; set; }

        [Display(Name ="Account Status")]
        public bool IsLocked { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedDate { get; set; }
    }
}
