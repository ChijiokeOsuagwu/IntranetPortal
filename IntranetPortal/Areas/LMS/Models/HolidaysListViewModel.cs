using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LMS.Models
{
    public class HolidaysListViewModel:BaseListViewModel
    {
        public int yr { get; set; }
        public List<PublicHoliday> HolidayList { get; set; }
    }
}
