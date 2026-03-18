using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.GlobalSettings.Models
{
    public class TeamMemberViewModel : BaseViewModel
    {
        public int? TeamMemberID { get; set; }

        [Required(ErrorMessage = "Sorry no Team has been selected!")]
        public string TeamID { get; set; }

        [Display(Name ="Member")]
        public string MemberID { get; set; }

        [Display(Name ="Role")]
        [Required(ErrorMessage ="Role is required!")]
        public string MemberRole { get; set; }

        [Display(Name = "Member")]
        [Required(ErrorMessage = "Please enter a staff name")]
        public string MemberName { get; set; }
        
        public TeamMember ConvertToTeamMember() 
        {
            return new TeamMember
            {
                TeamID = TeamID,
                MemberID = MemberID,
                MemberRole = MemberRole,
                TeamMemberID = TeamMemberID,
                FullName = MemberName
            };
        }
    }
}
