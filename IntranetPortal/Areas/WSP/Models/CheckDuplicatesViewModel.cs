using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class CheckDuplicatesViewModel:BaseViewModel
    {
        public string id { get; set; }
        public string kw { get; set; }
        public DateTime sd { get; set; }
        public DateTime ed { get; set; }
        public string TaskOwnerName { get; set; }
        public string TaskOwnerUnit { get; set; }
        public string TaskOwnerLocation { get; set; }
        public List<TaskItem> TaskItemList { get; set; }
    }
}
