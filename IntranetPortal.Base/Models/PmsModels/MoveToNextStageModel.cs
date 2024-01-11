using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.PmsModels
{
    public class MoveToNextStageModel
    {
        public int ReviewHeaderID { get; set; }
        public string AppraiseeID { get; set; }
        public string PrincipalAppraiserID { get; set; }
        public int ReviewSessionID { get; set; }
        public int CurrentStageID { get; set; }
        public int NextStageID { get; set; }
        public bool IsQualifiedToMove { get; set; }
        public string NextStageDescription { get; set; }
        public List<string> ErrorMessages { get; set; }
        public List<string> WarningMessages { get; set; }
    }
}
