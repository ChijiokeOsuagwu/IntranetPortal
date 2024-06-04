using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class CompetenciesListViewModel : BaseListViewModel
    {
        public int? cd { get; set; }
        public int? ld { get; set; }
        public List<Competency> CompetencyList { get; set; }
    }
}
