using IntranetPortal.Base.Models.SecurityModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.UserAdministration.Models
{
    public class LocationPermissionViewModel:BaseViewModel
    {
        public int LocationPermissionID { get; set; }

        [Required]
        public string UserID { get; set; }

        [Display(Name = "Full Name")]
        public string UserFullName { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [Display(Name = "Location")]
        public int LocationID { get; set; }

        [Display(Name = "Location")]
        public string LocationName { get; set; }

        public LocationPermission ConvertToLocationPermission()
        {
            return new LocationPermission
            {
                LocationPermissionId = LocationPermissionID,
                LocationId = LocationID,
                LocationName = LocationName,
                PermissionTypeId = (int)Base.Enums.EntityPermissionType.AssetLocation,
                UserId = UserID,
            };
        }
    }
}
