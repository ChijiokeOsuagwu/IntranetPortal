using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.GlobalSettings.Models
{
    public class ManageProgramViewModel:BaseViewModel
    {
        public int ProgramID { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string ProgramCode { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string ProgramTitle { get; set; }

        [Display(Name = "Description")]
        public string ProgramDescription { get; set; }

        [Required]
        [Display(Name = "Frequency")]
        public ProgramFrequency Frequency { get; set; }

        [Required]
        [Display(Name = "Status")]
        public ProgramStatus Status { get; set; }

        [Required]
        [Display(Name = "Host Station")]
        public int HostStationID { get; set; }

        [Display(Name = "Host Station")]
        public string HostStationName { get; set; }

        [Required]
        [Display(Name="Start Time (GMT)")]
        public DateTime? StartTime { get; set; }

        [Required]
        [Display(Name = "End Time (GMT)")]
        public DateTime? EndTime { get; set; }

        [Display(Name = "Start Time (GMT)")]
        public string StartTimeFormatted { get; set; }

        [Display(Name = "End Time (GMT)")]
        public string EndTimeFormatted { get; set; }

        [Display(Name = "Platform")]
        public string Platform { get; set; }

        [Required]
        [Display(Name = "Programme Belt")]
        public string ProgramBelt { get; set; }

        [Required]
        [Display(Name = "Programme Type")]
        public string ProgramType { get; set; }

        public Programme ConvertToProgram()
        {
            return new Programme
            {
                Code = ProgramCode,
                Description = ProgramDescription,
                EndTime = EndTime.Value.ToLongTimeString(),
                Frequency = Frequency,
                HostStationId = HostStationID,
                HostStationName = HostStationName,
                Id = ProgramID,
                Platform = Platform,
                ProgramBelt = ProgramBelt,
                ProgramType = ProgramType,
                StartTime = StartTime.Value.ToLongTimeString(),
                Status = Status,
                Title = ProgramTitle,
            };
        }
    }
}
