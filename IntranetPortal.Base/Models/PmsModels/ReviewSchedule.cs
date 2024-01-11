using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.PmsModels
{
    public class ReviewSchedule
    {
        public bool AllActivitiesScheduled { get; set; }
        public bool ContractDefinitionScheduled { get; set; }
        public bool PerformanceEvaluationScheduled { get; set; }
    }
}
