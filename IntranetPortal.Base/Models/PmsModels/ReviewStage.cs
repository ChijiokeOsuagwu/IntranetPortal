using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.PmsModels
{
    public class ReviewStage
    {
        public int ReviewStageId { get; set; }
        public string ReviewStageName { get; set; }
        public string ActionDescription { get; set; }
        public string PhaseDescription { get; set; }
        public string HelpInstruction { get; set; }
    }
}
