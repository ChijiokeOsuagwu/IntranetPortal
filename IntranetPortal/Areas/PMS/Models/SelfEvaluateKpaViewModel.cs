using IntranetPortal.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
	public class SelfEvaluateKpaViewModel : BaseViewModel
	{
		[Required]
		public int ReviewHeaderID { get; set; }

		[Required]
		public int ReviewSessionID { get; set; }

		[Required]
		public string AppraiseeID { get; set; }
		public int ReviewResultID { get; set; }
		public string ReviewSessionName { get; set; }
		public int ReviewYearID { get; set; }
		public string ReviewYearName { get; set; }
		public string AppraiseeName { get; set; }
		public string AppraiserID { get; set; }
		public string AppraiserName { get; set; }

		[Required]
		public int AppraiserRoleID { get; set; }
		public string AppraiserRoleName { get; set; }

		[Required]
		public int AppraiserTypeID { get; set; }
		public string AppraiserTypeDescription { get; set; }

		[Required]
		public int ReviewMetricID { get; set; }
		public int ReviewMetricTypeID { get; set; }
		public string ReviewMetricTypeDescription { get; set; }
		public decimal ReviewMetricWeightage { get; set; }

		[Display(Name = "Description")]
		public string ReviewMetricDescription { get; set; }

		[Display(Name = "Measurement")]
		public string ReviewMetricMeasurement { get; set; }

		[Display(Name = "Target Achievement")]
		public string ReviewMetricTarget { get; set; }

		[Display(Name = "Actual Achievement")]
		public string ActualAchievement { get; set; }

		[Required]
		[Display(Name = "Score")]
		public decimal AppraiserScore { get; set; }
		public string AppraiserScoreDescription { get; set; }

		[Display(Name = "Score")]
		public int ScoreGradeID { get; set; }

		public DateTime? ScoreTime { get; set; }
		public string AppraiserComment { get; set; }
		public string AppraiseeAchievement { get; set; }
		public string AppraiseeScore { get; set; }
	}
}
