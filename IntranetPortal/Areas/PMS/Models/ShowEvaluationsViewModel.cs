﻿using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class ShowEvaluationsViewModel : BaseViewModel
    {
        public int ReviewHeaderID { get; set; }
        public int ReviewSessionID { get; set; }
        public string ReviewSessionName { get; set; }
        public string AppraiseeID { get; set; }
        public string AppraiseeName { get; set; }
        public List<EvaluationHeader> Evaluations { get; set; }
    }
}
