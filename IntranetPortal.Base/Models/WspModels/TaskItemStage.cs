using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public enum TaskItemStage
    {
        NotYetApproved,
        SubmittedForApproval,
        ApprovedForExecution,
        SubmittedForEvaluation,
        EvaluationCompleted,
        Cancelled
    }
}
