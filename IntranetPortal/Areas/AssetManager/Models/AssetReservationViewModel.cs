using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.AssetManager.Models
{
    public class AssetReservationViewModel : BaseViewModel
    {
        public int? AssetReservationID { get; set; }

        public string AssetID { get; set; }

        [Display(Name ="Equipment Name")]
        [Required]
        public string AssetName { get; set; }
        public string AssetDescription { get; set; }
        public int? AssetTypeID { get; set; }
        public string AssetTypeName { get; set; }

        [Display(Name ="From")]
        [DataType(DataType.DateTime)]
        [Required]
        public DateTime? EventStartTime { get; set; }

        [Display(Name = "From")]
        public string EventStartTimeFormatted { get; set; }

        [Display(Name = "To")]
        [DataType(DataType.DateTime)]
        [Required]
        public DateTime? EventEndTime { get; set; }

        [Display(Name = "To")]
        public string EventEndTimeFormatted { get; set; }

        [Required]
        [Display(Name ="Event Description")]
        [MaxLength(200, ErrorMessage ="Event Description must no exceed 200 characters.")]
        public string EventDescription { get; set; }

        [Display(Name ="Booked By")]
        public string ReservedBy { get; set; }

        [Display(Name ="Booked On")]
        public DateTime? ReservedOn { get; set; }

        public string ReservedOnFormatted { get; set; }

        [Required]
        [Display(Name = "Event Location")]
        [MaxLength(100, ErrorMessage = "Event Location must no exceed 100 characters.")]
        public string EventLocation { get; set; }

        [Required]
        [Display(Name ="Status")]
        public string ReservationStatus { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedTime { get; set; }

        public AssetReservation ConvertToAssetReservation()
        {
            return new AssetReservation
            {
                AssetDescription = AssetDescription,
                AssetID = AssetID,
                AssetName = AssetName,
                AssetReservationID = AssetReservationID ?? 0,
                AssetTypeID = AssetTypeID ?? 0,
                AssetTypeName = AssetTypeName,
                EventDescription = EventDescription,
                EventEndTime = EventEndTime,
                EventLocation = EventLocation,
                EventStartTime = EventStartTime,
                LastModifiedBy = LastModifiedBy,
                LastModifiedTime = LastModifiedTime,
                ReservationStatus = ReservationStatus,
                ReservedBy = ReservedBy,
                ReservedOn = ReservedOn,
            };
        }
    }
}
