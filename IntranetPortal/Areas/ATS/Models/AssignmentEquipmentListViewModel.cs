using IntranetPortal.Base.Models.AtsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ATS.Models
{
    public class AssignmentEquipmentListViewModel : BaseViewModel
    {
        public long AssignmentID { get; set; }
        public string AssignmentTitle { get; set; }
        public string AssignmentState { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public string NewAssetName { get; set; }
        public string NewAssetTypeId { get; set; }
        public string NewAssetGroupId { get; set; }
        public string NewAssetClassId { get; set; }
        public string NewAssignedToEmployeeId { get; set; }
        public string NewAssignedToEmployeeName { get; set; }
        public List<AssignmentEquipment> EquipmentList { get; set; }
    }
}
