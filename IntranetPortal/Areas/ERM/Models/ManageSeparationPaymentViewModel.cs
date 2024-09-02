using System;
using System.ComponentModel.DataAnnotations;
using IntranetPortal.Models;
using IntranetPortal.Base.Models.EmployeeRecordModels;

namespace IntranetPortal.Areas.ERM.Models
{
    public class ManageSeparationPaymentViewModel:BaseViewModel
    {
        public int Id { get; set; }
        public int OutstandingId { get; set; }
        public int EmployeeSeparationId { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        [Display(Name="Item")]
        public string ItemDescription { get; set; }

        [Display(Name = "Item")]
        public string ItemDescriptionFormatted { get; set; }

        [Display(Name ="Type")]
        public string ItemTypeDescription { get; set; }

        [Required]
        [Display(Name="Amount Paid")]
        public decimal PaymentAmount { get; set; }

        [Display(Name = "Amount Paid")]
        public string PaymentAmountFormatted { get; set; }

        [Required]
        [Display(Name ="Currency")]
        public string Currency { get; set; }

        [Required]
        [Display(Name = "Payment Date")]
        public DateTime? PaymentDate { get; set; }

        [Display(Name = "Payment Date")]
        public string PaymentDateFormatted { get; set; }

        [Display(Name = "Payment Details")]
        public string PaymentDetails { get; set; }

        [Required]
        [Display(Name = "Paid By")]
        public string PaidBy { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }

        public EmployeeSeparationPayments Convert()
        {
            return new EmployeeSeparationPayments
            {
                Currency = Currency,
                EmployeeId = EmployeeId,
                EmployeeName = EmployeeName,
                EmployeeSeparationId = EmployeeSeparationId,
                EnteredBy = EnteredBy,
                EnteredDate = EnteredDate,
                Id = Id,
                ItemDescription = ItemDescription,
                ItemTypeDescription = ItemTypeDescription,
                OutstandingId = OutstandingId,
                PaidBy = PaidBy,
                PaymentAmount = PaymentAmount,
                PaymentDate = PaymentDate,
                PaymentDetails = PaymentDetails,
            };
        }

        public ManageSeparationPaymentViewModel Extract(EmployeeSeparationPayments p)
        {
            return new ManageSeparationPaymentViewModel
            {
                Currency = p.Currency,
                EmployeeId = p.EmployeeId,
                EmployeeName = p.EmployeeName,
                EmployeeSeparationId = p.EmployeeSeparationId,
                EnteredBy = p.EnteredBy,
                EnteredDate = p.EnteredDate,
                Id = p.Id,
                ItemDescription = p.ItemDescription,
                ItemDescriptionFormatted = $"{p.ItemDescription} ({p.ItemTypeDescription})",
                ItemTypeDescription = p.ItemTypeDescription,
                OutstandingId = p.OutstandingId,
                PaidBy = p.PaidBy,
                PaymentAmount = p.PaymentAmount,
                PaymentAmountFormatted = $"{p.Currency} {p.PaymentAmount.ToString("N")}",
                PaymentDate = p.PaymentDate,
                PaymentDateFormatted = p.PaymentDate == null? "[Not Specified]" : p.PaymentDate.Value.ToLongDateString(),
                PaymentDetails = p.PaymentDetails,
            };
        }
    }
}
