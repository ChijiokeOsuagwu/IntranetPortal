using IntranetPortal.Base.Models.SecurityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.UserAdministration.Models
{
    public class LoginHistoryListViewModel
    {
        [Range(2022, 2099)]
        public int? yy { get; set; }
        public int? mm { get; set; }

        [Range(1, 31)]
        public int? dd { get; set; }
        public string nm { get; set; }

        public IList<UserLoginHistory> UserLoginHistories { get; set; }
    }
}
