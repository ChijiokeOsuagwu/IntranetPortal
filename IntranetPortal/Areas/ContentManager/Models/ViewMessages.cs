using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ContentManager.Models
{
    public class ViewMessages
    {
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public string WarningMessage { get; set; }
        public string InfoMessage { get; set; }
    }
}
