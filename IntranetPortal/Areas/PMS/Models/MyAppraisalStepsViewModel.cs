using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class MyAppraisalStepsViewModel : BaseViewModel
    {
        public List<ReviewStage> AppraisalStageList { get; set; }
        public string ReviewSessionName { get; set; }
        public int ReviewSessionId { get; set; }
        public string AppraiseeId { get; set; }
        public int? ReviewHeaderId { get; set; }
        public int CurrentReviewStageId { get; set; }
        public string AppraiseeName { get; set; }
        public bool IsActive { get; set; }
        public bool AllActivitiesScheduled { get; set; }
        public bool ContractDefinitionScheduled { get; set; }
        public bool PerformanceEvaluationScheduled { get; set; }
    }
}
