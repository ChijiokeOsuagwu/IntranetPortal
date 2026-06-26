using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class AddLeaveAdjustmentViewModel:BaseViewModel
    {
        public long LeaveAdjustmentId { get; set; }
        [Required]
        public long LeaveRequestId { get; set; }
        [Required]
        public string LeaveTypeCode { get; set; }
        public string LeaveTypeName { get; set; }
        [Required]
        public string LeaveEmployeeId { get; set; }
        public string LeaveEmployeeName { get; set; }
        [Required]
        public int LeaveYear { get; set; }
        [Required(ErrorMessage = "Please select Adjustment Type.")]
        public string AdjustmentType { get; set; }
        [Required(ErrorMessage ="Please enter a reason.")]
        [MaxLength(500)]
        public string AdjustmentJustification { get; set; }
        [Required(ErrorMessage ="Number of Days is required.")]
        [Range(1,1000, ErrorMessage ="Please enter a valid number.")]
        public int NumberOfDays { get; set; }
        public string DurationDescription { get; set; }
        public DateTime AdjustmentDate { get; set; }
        public string AdjustmentAddedBy { get; set; }
        public int LeaveUnitId { get; set; }
        public int LeaveDepartmentId { get; set; }
        public int LeaveLocationId { get; set; }

        public LeaveAdjustment Convert()
        {
            return new LeaveAdjustment
            {
                AdjustmentAddedBy = AdjustmentAddedBy,
                AdjustmentDate = AdjustmentDate,
                AdjustmentJustification = AdjustmentJustification,
                AdjustmentType = AdjustmentType,
                LeaveAdjustmentId = LeaveAdjustmentId,
                LeaveDepartmentId = LeaveDepartmentId,
                LeaveEmployeeId = LeaveEmployeeId,
                LeaveEmployeeName = LeaveEmployeeName,
                LeaveLocationId = LeaveLocationId,
                LeaveRequestId = LeaveRequestId,
                LeaveTypeCode = LeaveTypeCode,
                LeaveUnitId = LeaveUnitId,
                LeaveYear = LeaveYear,
                DurationDescription = DurationDescription,
                NumberOfDays = NumberOfDays,
            };
        }

        public AddLeaveAdjustmentViewModel Convert(LeaveAdjustment adjustment)
        {
            return new AddLeaveAdjustmentViewModel
            {
                AdjustmentAddedBy = adjustment.AdjustmentAddedBy,
                AdjustmentDate = adjustment.AdjustmentDate,
                AdjustmentJustification = adjustment.AdjustmentJustification,
                AdjustmentType = adjustment.AdjustmentType,
                LeaveAdjustmentId = adjustment.LeaveAdjustmentId,
                LeaveDepartmentId = adjustment.LeaveDepartmentId,
                LeaveEmployeeId = adjustment.LeaveEmployeeId,
                LeaveEmployeeName = adjustment.LeaveEmployeeName,
                LeaveLocationId = adjustment.LeaveLocationId,
                LeaveRequestId = adjustment.LeaveRequestId,
                LeaveTypeCode = adjustment.LeaveTypeCode,
                LeaveUnitId = adjustment.LeaveUnitId,
                LeaveYear = adjustment.LeaveYear,
                DurationDescription = adjustment.DurationDescription,
                NumberOfDays = adjustment.NumberOfDays,
            };
        }


    }
}
