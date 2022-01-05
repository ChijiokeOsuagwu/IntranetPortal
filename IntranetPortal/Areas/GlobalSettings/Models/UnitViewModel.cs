using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.GlobalSettings.Models
{
    public class UnitViewModel : BaseViewModel
    {
        public int UnitID { get; set; }

        [Display(Name="Name")]
        [Required(ErrorMessage ="Name is required.")]
        [MaxLength(100, ErrorMessage ="Name must not exceed 100 characters.")]
        public string UnitName { get; set; }

        [Display(Name ="Unit Head")]
        public string UnitHeadID1 { get; set; }

        [Display(Name = "Unit Head")]
        public string UnitHeadName1 { get; set; }

        [Display(Name = "Asst. Unit Head")]
        public string UnitHeadID2 { get; set; }

        [Display(Name = "Asst. Unit Head")]
        public string UnitHeadName2 { get; set; }

        [Required(ErrorMessage ="Department is required.")]
        [Display(Name ="Department")]
        public int? DepartmentID { get; set; }

        [Display(Name = "Department")]
        public string DepartmentName { get; set; }

        public Unit ConvertToUnit() => new Unit
        {
            UnitHeadID1 = UnitHeadID1,
            DepartmentID = DepartmentID.Value,
            DepartmentName = DepartmentName,
            UnitHeadID2 = UnitHeadID2,
            UnitHeadName1 = UnitHeadName1,
            UnitHeadName2 = UnitHeadName2,
            UnitID = UnitID,
            UnitName = UnitName
        };

    }
}
