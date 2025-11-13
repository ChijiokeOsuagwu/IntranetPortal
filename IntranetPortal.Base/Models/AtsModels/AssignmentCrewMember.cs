using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.AtsModels
{
    public class AssignmentCrewMember
    {
        public long? Id { get; set; }
        public long AssignmentId { get; set; }
        public string AssignmentTitle { get; set; }
        public string CrewMemberId { get; set; }
        public string CrewMemberName { get; set; }
        public string CrewMemberRole1 { get; set; }
        public string CrewMemberRole2 { get; set; }
        public string CrewMemberRole3 { get; set; }
        public bool IsTeamLead { get; set; }
        public int? UnitId { get; set; }
        public string UnitName { get; set; }
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int? LocationId { get; set; }
        public string LocationName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }

        public string AssignedByEmployeeId { get; set; }
        public string AssignedByEmployeeName { get; set; }
    }
}
