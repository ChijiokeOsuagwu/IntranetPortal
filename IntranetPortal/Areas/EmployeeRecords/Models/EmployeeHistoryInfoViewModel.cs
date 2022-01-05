using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.EmployeeRecords.Models
{
    public class EmployeeHistoryInfoViewModel : BaseViewModel
    {
        public string EmployeeID { get; set; }
        public string EmployeeNo1 { get; set; }
        public string EmployeeName { get; set; }

        [Display(Name = "Years of Experience")]
        public int? YearsOfExperience { get; set; }

        [Display(Name = "Length of Service (Years)")]
        public int? LengthOfService { get; set; }

        [Display(Name = "Place of Engagement")]
        public string PlaceOfEngagement { get; set; }

        [Display(Name = "Confirmation Date")]
        [DataType(DataType.Date)]
        public DateTime? ConfirmationDate { get; set; }

        [Display(Name ="Current Designation")]
      
        public string CurrentDesignation { get; set; }

        [Display(Name = "Last Promotion Date")]
        [DataType(DataType.Date)]
        public DateTime? DateOfLastPromotion { get; set; }
        public string EmployeeModifiedBy { get; set; }
        public string EmployeeModifiedDate { get; set; }

        public EmployeeHistoryInfo ConvertToEmployeeHistoryInfo() => new EmployeeHistoryInfo
        {
            EmployeeID = EmployeeID,
            EmployeeName = EmployeeName,
            EmployeeNo1 = EmployeeNo1,
            ConfirmationDate = ConfirmationDate,
            CurrentDesignation = CurrentDesignation,
            DateOfLastPromotion = DateOfLastPromotion,
            EmployeeModifiedBy = EmployeeModifiedBy,
            EmployeeModifiedDate = EmployeeModifiedDate,
            PlaceOfEngagement = PlaceOfEngagement,
            LengthOfService = LengthOfService,
            YearsOfExperience = YearsOfExperience,
        };
    }
}
