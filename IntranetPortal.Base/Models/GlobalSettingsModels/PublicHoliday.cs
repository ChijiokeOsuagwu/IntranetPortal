using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.GlobalSettingsModels
{
    public class PublicHoliday
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Reason { get; set; }
        public string  Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NoOfDays { get; set; }
        public int HolidayYear { get; set; }
    }
}
