using IntranetPortal.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ERM.Models
{
    public class UploadImageViewModel:BaseViewModel
    {
        [Required]
        public string EmployeeID { get; set; }

        [Display(Name="Name")]
        public string EmployeeName { get; set; }

        [Required]
        public IFormFile UploadedImage { get; set; }

        public string OldImagePath { get; set; }
    }
}
