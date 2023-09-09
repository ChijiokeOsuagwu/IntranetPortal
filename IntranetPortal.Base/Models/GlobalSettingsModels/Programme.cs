using IntranetPortal.Base.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.GlobalSettingsModels
{
    public class Programme
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ProgramFrequency Frequency { get; set; }
        public ProgramStatus Status { get; set; }
        public int? HostStationId { get; set; }
        public string HostStationName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Platform { get; set; }
        public string ProgramBelt { get; set; }
        public string ProgramType { get; set; }
    }
}
