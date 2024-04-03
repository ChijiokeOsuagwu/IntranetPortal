using IntranetPortal.Models;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class ManageFeedbackViewModel : BaseViewModel
    {
        [Required]
        public int ReviewHeaderID { get; set; }
        public int ReviewSessionID { get; set; }

        [Required]
        [Display(Name = "Problem(s) Encountered")]
        [MaxLength(5000)]
        public string ProblemDescription { get; set; }

        [Required]
        [Display(Name = "Recommendations")]
        [MaxLength(5000)]
        public string SolutionDescription { get; set; }
    }

}
