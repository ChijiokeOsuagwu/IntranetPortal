using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class ParticipationSummaryReportViewModel:BaseViewModel
    {
        public int Id { get; set; }
        public int Ld { get; set; }
        public int Dd { get; set; }
        public int Ud { get; set; }
        public long TotalNoOfActiveEmployees { get; set; }
        public long NoOfParticipants { get; set; }
        public long NoOfNonParticipants { get; set; }
        public string ParticipantsInPercentage { get; set; }
        public string NonParticipantsInPercentage { get; set; }
        public List<ParticipationSummary> ParticipationSummaryList { get; set; }
    }
}
