using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.PmsModels
{
    public class ParticipationSummary
    {
        public int ReviewSessionId { get; set; }
        public string ReviewSessionName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int ReviewStageId { get; set; }
        public string ReviewStageName { get; set; }
        public long NoOfParticipants { get; set; }
        public decimal PercentageOfParticipants { get; set; }
        public string PercentageParticipantsFormatted { get; set; }
    }
}
