using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.GlobalSettings.Models
{
    public class LocationAddViewModel : BaseViewModel
    {
        [Required]
        public int LocationID { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Name cannot be more than 100 characters.")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Location Type is required!")]
        public string LocationType { get; set; }

        [MaxLength(100, ErrorMessage = "State cannot be more than 100 characters.")]
        [Display(Name = "State/County/Province")]
        public string State { get; set; }

        [MaxLength(100, ErrorMessage = "Country cannot be more than 100 characters.")]
        [Display(Name = "Country")]
        public string Country { get; set; }


        [MaxLength(100, ErrorMessage = "Head cannot be more than 100 characters.")]
        [Display(Name = "Head")]
        public string LocationHeadID1 { get; set; }


        [MaxLength(100, ErrorMessage = "Assistant Head cannot be more than 100 characters.")]
        [Display(Name = "Assistant Head")]
        public string LocationHeadID2 { get; set; }

        public Location ConvertToLocation() => new Location
        {
            LocationID = LocationID,
            LocationName = Name,
            LocationCountry = Country,
            LocationState = State,
            LocationHeadID1 = LocationHeadID1,
            LocationHeadID2 = LocationHeadID2,
            LocationType = LocationType,
            ModifiedDate = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()}"
        };
    }
}
