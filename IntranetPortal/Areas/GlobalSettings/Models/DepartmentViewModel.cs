using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.GlobalSettings.Models
{
    public class DepartmentViewModel : BaseViewModel
    {
        public int DepartmentID { get; set; }

        [Display(Name ="Name")]
        [MaxLength(100,ErrorMessage ="Name must not exceed 100 characters.")]
        [Required(ErrorMessage="Name is required.")]
        public string DepartmentName { get; set; }

        [Display(Name = "Department Head")]
        public string DepartmentHeadID1 { get; set; }

        [Display(Name ="Department Head")]
        public string DepartmentHeadName1 { get; set; }

        [Display(Name = "Asst. Department Head")]
        public string DepartmentHeadID2 { get; set; }

        [Display(Name = "Asst. Department Head")]
        public string DepartmentHeadName2 { get; set; }

        public Department ConvertToDepartment() => new Department
        {
            DepartmentID = DepartmentID,
            DepartmentHeadID1 = DepartmentHeadID1,
            DepartmentHeadID2 = DepartmentHeadID2,
            DepartmentName = DepartmentName,
            DepartmentHeadName1 = DepartmentHeadName1,
            DepartmentHeadName2 = DepartmentHeadName2
        };
    }
}
