using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.GlobalSettings.Models
{
    public class ProgramListViewModel:BaseViewModel
    {
        public string bt { get; set; }
        public string tp { get; set; }
        public string st { get; set; }
        public List<Programme> ProgramList { get; set; }
    }
}
