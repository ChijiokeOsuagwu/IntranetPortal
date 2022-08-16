using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.GlobalSettings.Models
{
    public class TeamViewModel : BaseViewModel
    {
        public string TeamID { get; set; }

        [Display(Name = "Name")]
        [MaxLength(200, ErrorMessage = "Name must not exceed 200 characters.")]
        [Required(ErrorMessage = "Name is required.")]
        public string TeamName { get; set; }

        [Display(Name = "Description")]
        public string TeamDescription { get; set; }

        [Display(Name = "Team Location")]
        public int? TeamLocationID { get; set; }

        [Display(Name = "Team Location")]
        public string TeamLocationName { get; set; }
         public Team ConvertToTeam() => new Team
        {
            TeamID = TeamID,
            TeamName = TeamName,
            TeamDescription = TeamDescription,
            TeamLocationID = TeamLocationID,
            TeamLocationName = TeamLocationName
        };
    }
}
