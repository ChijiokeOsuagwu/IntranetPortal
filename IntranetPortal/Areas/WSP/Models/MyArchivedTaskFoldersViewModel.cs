using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class MyArchivedTaskFoldersViewModel:BaseViewModel
    {
        public string Id { get; set; }
        public DateTime? fd { get; set; }
        public DateTime? td { get; set; }
        public List<WorkItemFolder> ArchivedFolders { get; set; }
    }
}
