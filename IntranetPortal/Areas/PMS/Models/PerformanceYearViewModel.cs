using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class PerformanceYearViewModel : BaseViewModel
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        public PerformanceYear ConvertToPerformanceYear()
        {
            return new PerformanceYear
            {
                Id = Id ?? 0,
                Name = Name,
                StartDate = StartDate,
                EndDate = EndDate
            };
        }

        public PerformanceYearViewModel ExtractFromPerformanceYear(PerformanceYear performanceYear)
        {
            return new PerformanceYearViewModel
            {
                Id = performanceYear.Id,
                Name = performanceYear.Name,
                StartDate = performanceYear.StartDate,
                EndDate = performanceYear.EndDate
            };
        }
    }

}
