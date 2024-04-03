using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class AppraisalGradeSettingsListViewModel : BaseViewModel
    {
        public int ReviewSessionID { get; set; }
        public List<AppraisalGrade> AppraisalPerformanceGradeList { get; set; }
        public List<AppraisalGrade> AppraisalCompetencyGradeList { get; set; }
    }
}
