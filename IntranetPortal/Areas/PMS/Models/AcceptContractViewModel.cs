using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class AcceptContractViewModel : BaseViewModel
    {
        [Required]
        public int ReviewHeaderID { get; set; }
        public int ReviewSessionID { get; set; }
        public string ReviewSessionName { get; set; }
        public string ReviewYearName { get; set; }
        [Required]
        public string AppraiseeID { get; set; }
        public string AppraiseeName { get; set; }
        public DateTime? SignedOffTime { get; set; }
        public string SignedOffTimeFormatted { get; set; }
        public bool IsNotAccepted { get; set; }
    }
}
