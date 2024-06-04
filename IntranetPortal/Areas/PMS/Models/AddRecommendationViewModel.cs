using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class AddRecommendationViewModel : BaseViewModel
    {
        [Required]
        public int ReviewHeaderID { get; set; }
        public int ReviewSessionID { get; set; }
        public string AppraiseeID { get; set; }
        public string RecommenderID { get; set; }

        [Display(Name = "Name:")]
        public string RecommenderName { get; set; }

        [Required]
        [Display(Name = "Recommended By:")]
        public string RecommenderRole { get; set; }

        [Display(Name = "Recommended By:")]
        public string RecommenderRoleDescription { get; set; }

        [Required]
        [Display(Name = "Recommended For:")]
        [MaxLength(100)]
        public string RecommendedAction { get; set; }

        [Required]
        [Display(Name = "Justification")]
        [MaxLength(10000)]
        public string Remarks { get; set; }
    }

}
