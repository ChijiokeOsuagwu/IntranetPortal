using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Models
{
    public class BaseViewModel
    {
        public string SourceKey { get; set; }
        public string SourcePage { get; set; }
        public string src { get; set; }
        public string psp { get; set; }
        public string ViewModelErrorMessage { get; set; } = string.Empty;
        public string ViewModelSuccessMessage { get; set; } = string.Empty;
        public string ViewModelWarningMessage { get; set; } = string.Empty;
        public bool OperationIsCompleted { get; set; } = false;
        public bool OperationIsSuccessful { get; set; } = false;
    }
}
