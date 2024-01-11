using IntranetPortal.Base.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.PmsModels
{
    public class AppraisalGrade
    {
        public int AppraisalGradeId { get; set; }
        public string AppraisalGradeDescription { get; set; }
        public int ReviewSessionId { get; set; }
        public string ReviewSessionName { get; set; }
        public ReviewGradeType GradeType { get; set; }
        public decimal LowerBandScore { get; set; }
        public decimal UpperBandScore { get; set; }
        public int GradeRank { get; set; }
        public string GradeRankDescription { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
    }
}
