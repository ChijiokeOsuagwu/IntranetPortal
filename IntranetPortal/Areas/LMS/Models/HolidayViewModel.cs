using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LMS.Models
{
    public class HolidayViewModel:BaseViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name="Title")]
        [MaxLength(65, ErrorMessage ="Title must not exceed 65 characters.")]
        public string Name { get; set; }

        [Display(Name = "Reason for Holiday")]
        [MaxLength(250, ErrorMessage = "Reason must not exceed 250 characters.")]
        public string Reason { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

        [Required]
        [Display(Name = "Starts On")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Ends On")]
        public DateTime EndDate { get; set; }

        [Display(Name = "No.of Days")]
        public int NoOfDays { get; set; }

        [Display(Name = "Year")]
        public int HolidayYear { get; set; }

    }
}
