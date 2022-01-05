using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.EmployeeRecords.Models
{
    public class EmployeeNextOfKinInfoViewModel : BaseViewModel
    {
        [Required]
        public string EmployeeID { get; set; }

        [Display(Name ="Employee No.")]
        public string EmployeeNo1 { get; set; }

        [Display(Name = "Employee Name.")]
        public string EmployeeName { get; set; }

        [MaxLength(100, ErrorMessage ="Next of Kin Name must not exceed 100 characters.")]
        [Display(Name ="Next of Kin Name")]
        public string NextOfKinName { get; set; }

        [MaxLength(250, ErrorMessage = "Address must not exceed 250 characters.")]
        [Display(Name = "Next of Kin Address")]
        public string NextOfKinAddress { get; set; }

        [MaxLength(250, ErrorMessage = "Email must not exceed 250 characters.")]
        [Display(Name = "Next of Kin Email")]
        public string NextOfKinEmail { get; set; }

        [MaxLength(50, ErrorMessage = "Phone must not exceed 50 characters.")]
        [Display(Name = "Next of Kin Phone")]
        public string NextOfKinPhone { get; set; }

        [MaxLength(100, ErrorMessage = "Relationship must not exceed 100 characters.")]
        [Display(Name = "Next  of Kin Relationship")]
        public string NextOfKinRelationship { get; set; }

        public EmployeeNextOfKinInfo ConvertToEmployeeNextOfKinInfo() => new EmployeeNextOfKinInfo
        {
            EmployeeID = EmployeeID,
            NextOfKinAddress = NextOfKinAddress,
            NextOfKinEmail = NextOfKinEmail,
            NextOfKinRelationship = NextOfKinRelationship,
            NextOfKinName = NextOfKinName,
            NextOfKinPhone = NextOfKinPhone
        };
    }
}
