using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class TaskEvaluationHeaderSummary
    {
        public string TaskOwnerId { get; set; }
        public string TaskOwnerName { get; set; }
        public int TaskOwnerUnitId { get; set; }
        public string TaskOwnerUnitName { get; set; }
        public int TaskOwnerDepartmentId { get; set; }
        public string TaskOwnerDepartmentName { get; set; }
        public int TaskOwnerLocationId { get; set; }
        public string TaskOwnerLocationName { get; set; }
        public decimal TotalNoOfTasks { get; set; }
        public decimal TotalNoOfCompletedTasks { get; set; }
        public decimal TotalNoOfUncompletedTasks { get; set; }
        public decimal AveragePercentCompletion { get; set; }
        public decimal AverageQualityRating { get; set; }
    }
}
