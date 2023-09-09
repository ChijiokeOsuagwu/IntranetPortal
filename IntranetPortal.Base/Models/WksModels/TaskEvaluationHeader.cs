using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WksModels
{
    public class TaskEvaluationHeader
    {
        public int Id { get; set; }
        public int TaskListId { get; set; }
        public string TaskListName { get; set; }
        public string EvaluatorId { get; set; }
        public string EvaluatorName { get; set; }
        public string EvaluatorDesignation { get; set; }
        public string TaskOwnerId { get; set; }
        public string TaskOwnerName { get; set; }
        public string TaskOwnerDesignation { get; set; }
        public int TaskOwnerUnitId { get; set; }
        public string TaskOwnerUnitName { get; set; }
        public int TaskOwnerDeptId { get; set; }
        public string TaskOwnerDeptName { get; set; }
        public int TaskOwnerLocationId { get; set; }
        public string TaskOwnerLocationName { get; set; }
        public DateTime? EvaluationDate { get; set; }
        public long TotalNumberOfTasks { get; set; }
        public long NoOfCompletedTasks { get; set; }
        public long NoOfUncompletedTasks { get; set; }
        public long TotalPercentCompletion { get; set; }
        public decimal AveragePercentCompletion { get; set; }
        public long TotalQualityRating { get; set; }
        public decimal AverageQualityRating { get; set; }
    }
}
