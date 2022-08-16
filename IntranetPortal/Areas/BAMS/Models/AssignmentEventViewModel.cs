using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.BAMS.Models
{
    public class AssignmentEventViewModel : BaseViewModel
    {
        public int? ID { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage ="Event Title must not exceed 100 characters.")]
        [Display(Name ="Event Title")]
        public string Title { get; set; }

        [MaxLength(5000, ErrorMessage = "Event Description must not exceed 5000 characters.")]
        [Display(Name = "Event Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Event Type")]
        public int EventTypeID { get; set; }

        [Display(Name = "Event Type")]
        public string EventTypeName { get; set; }

        [Display(Name = "Event Starts")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime? StartTime { get; set; }

        [Display(Name = "Event Ends")]
        [DataType(DataType.DateTime)]
        public DateTime? EndTime { get; set; }

        [Display(Name = "Station")]
        [Required]
        public int StationID { get; set; }

        [Display(Name = "Station")]
        [MaxLength(100, ErrorMessage = "Station must not exceed 100 characters.")]
        public string StationName { get; set; }

        [Required]
        [Display(Name = "Event Venue")]
        [MaxLength(250, ErrorMessage ="Event Venue must not exceed 250 characters.")]
        public string Venue { get; set; }

        [Required]
        [Display(Name = "State")]
        [MaxLength(100, ErrorMessage = "State must not exceed 100 characters.")]
        public string State { get; set; }

        [Display(Name ="Customer")]
        public string CustomerID { get; set; }

        [Display(Name ="Customer")]
        [Required]
        [MaxLength(250, ErrorMessage = "Customer must not exceed 250 characters.")]
        public string CustomerName { get; set; }

        [Display(Name ="Contact(s) Name(s)")]
        [MaxLength(300, ErrorMessage ="Contact Name must not exceed 300 characters.")]
        public string LiaisonName { get; set; }

        [Display(Name ="Contact(s) Phone(s)")]
        [MaxLength(150, ErrorMessage ="Contact Phone(s) must not exceed 150 characters.")]
        public string LiaisonPhone { get; set; }

        [Required]
        [Display(Name = "Assignment Status")]
        public int StatusID { get; set; }

        [Display(Name = "Assignment Status")]
        public string StatusDescription { get; set; }

        [Required]
        [Display(Name = "This is a Paid Assignment")]
        public bool IsPaid { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedTime { get; set; }

        public AssignmentEvent ConvertToAssignmentEvent()
        {
            return new AssignmentEvent
            {
                CreatedBy = CreatedBy,
                CreatedTime = CreatedTime,
                CustomerID = CustomerID,
                CustomerName = CustomerName,
                Description = Description,
                EndTime = EndTime,
                EventTypeID = EventTypeID,
                EventTypeName = EventTypeName,
                ID = ID,
                IsPaid = IsPaid,
                LiaisonName = LiaisonName,
                LiaisonPhone = LiaisonPhone,
                ModifiedBy = ModifiedBy,
                ModifiedTime = ModifiedTime,
                StartTime = StartTime,
                State = State,
                StationID = StationID,
                StationName = StationName,
                StatusDescription = StatusDescription,
                StatusID = StatusID,
                Title = Title,
                Venue = Venue
            };
        }
    }
}
