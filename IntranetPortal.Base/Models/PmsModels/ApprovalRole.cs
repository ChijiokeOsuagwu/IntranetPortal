using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.PmsModels
{
    public class ApprovalRole
    {
        public int ApprovalRoleId { get; set; }
        public string ApprovalRoleName { get; set; }
        public bool MustApproveContract { get; set; }
        public bool MustApproveEvaluation { get; set; }
    }
}
