using IntranetPortal.Base.Enums;
using System;

namespace IntranetPortal.Base.Models.PmsModels
{
    public class ReviewGrade
    {
        public int ReviewGradeId { get; set; }
        public string ReviewGradeDescription { get; set; }
        public int GradeHeaderId { get; set; }
        public string GradeHeaderName { get; set; }
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
