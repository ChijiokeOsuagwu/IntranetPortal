using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.BAMS.Models
{
    public class AssignmentUpdatesViewModel:BaseViewModel
    {
        public int AssignmentEventID { get; set; }

        [Required]
        [Display(Name = "Description:")]
        [MaxLength(500, ErrorMessage = "Must not exceed 500 characters.")]
        public string UpdateDescription { get; set; }

        [Required]
        [Display(Name = "Type:")]
        public AssignmentUpdateType UpdateType { get; set; }
    }
}
