using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class CompetencyLevelsListViewModel:BaseListViewModel
    {
        public List<CompetencyLevel> CompetencyLevelList { get; set; }
    }
}
