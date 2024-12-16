using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.PmsModels
{
    public class EvaluationHeader
    {
        public int ReviewHeaderId { get; set; }
        public int ReviewSessionId { get; set; }
        public string ReviewSessionName { get; set; }
        public string AppraiseeId { get; set; }
        public string AppraiseeName { get; set; }
        public string AppraiserId { get; set; }
        public string AppraiserName { get; set; }
        public int AppraiserTypeId { get; set; }
        public string AppraiserTypeDescription { get; set; }
        public int? AppraiserRoleId { get; set; }
        public string AppraiserRoleName { get; set; }
        public DateTime? TimeEvaluated { get; set; }
    }
}
