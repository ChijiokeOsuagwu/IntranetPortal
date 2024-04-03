using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class SelectCompetencyViewModel : BaseViewModel
    {
        public int ReviewHeaderID { get; set; }
        public int cd { get; set; }
        public int vd { get; set; }
        public int dd { get; set; }
        public List<Competency> CompetenciesList { get; set; }
    }
}
