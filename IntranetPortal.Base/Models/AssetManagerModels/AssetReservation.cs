using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.AssetManagerModels
{
    public class AssetReservation
    {
        public int AssetReservationID { get; set; }
        public string AssetID { get; set; }
        public string AssetName { get; set; }
        public string AssetDescription { get; set; }
        public int AssetTypeID { get; set; }
        public string AssetTypeName { get; set; }
        public DateTime? EventStartTime { get; set; }
        public DateTime? EventEndTime { get; set; }
        public string EventDescription { get; set; }
        public string ReservedBy { get; set; }
        public DateTime? ReservedOn { get; set; }
        public string EventLocation { get; set; }
        public string ReservationStatus { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedTime { get; set; }
    }
}
