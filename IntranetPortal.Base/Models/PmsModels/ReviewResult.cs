using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.PmsModels
{
	public class ReviewResult
	{
		public int ReviewResultId { get; set; }
		public int ReviewSessionId { get; set; }
		public string ReviewSessionName { get; set; }
		public int ReviewHeaderId { get; set; }
		public int ReviewYearId { get; set; }
		public string ReviewYearName { get; set; }
		public string AppraiseeId { get; set; }
		public string AppraiseeName { get; set; }
		public string AppraiserId { get; set; }
		public string AppraiserName { get; set; }
		public int? AppraiserRoleId { get; set; }
		public string AppraiserRoleName { get; set; }
		public int AppraiserTypeId { get; set; }
		public string AppraiserTypeDescription { get; set; }
		public int ReviewMetricId { get; set; }
		public string ReviewMetricDescription { get; set; }
		public int ReviewMetricTypeId { get; set; }
		public string ReviewMetricTypeDescription { get; set; }
		public string ReviewMetricMeasurement { get; set; }
		public string ReviewMetricTarget { get; set; }
		public string ActualAchievement { get; set; }
		public decimal ReviewMetricWeightage { get; set; }
		public decimal AppraiserScore { get; set; }
		public string AppraiserScoreDescription { get; set; }
		public decimal AppraiserScoreValue { get; set; }
		public DateTime? ScoreTime { get; set; }
		public string AppraiserComment { get; set; }
		public string AppraiseeAchievement { get; set; }
		public string AppraiseeScore { get; set; }
	}
}
