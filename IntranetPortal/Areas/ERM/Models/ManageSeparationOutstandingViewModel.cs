using IntranetPortal.Models;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IntranetPortal.Areas.ERM.Models
{
    public class ManageSeparationOutstandingViewModel:BaseViewModel
    {
        public int Id { get; set; }
        public int EmployeeSeparationId { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        [Required]
        [Display(Name = "Type")]
        public int TypeId { get; set; }

        [Display(Name = "Type")]
        public string TypeDescription { get; set; }

        [Required]
        [Display(Name = "Item")]
        public string ItemDescription { get; set; }

        [Required]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Amount")]
        public string AmountFormatted { get; set; }

        [Required]
        [Display(Name = "Currency")]
        public string Currency { get; set; }

        [Display(Name = "Total Amount Paid")]
        public decimal AmountPaid { get; set; }

        [Display(Name = "Total Amount Paid")]
        public string AmountPaidFormatted { get; set; }

        [Display(Name = "Balance")]
        public decimal AmountBalance { get; set; }

        [Display(Name = "Balance")]
        public string AmountBalanceFormatted { get; set; }

        public EmployeeSeparationOutstanding Convert()
        {
            return new EmployeeSeparationOutstanding
            {
                Amount = Amount,
                AmountBalance = AmountBalance,
                AmountFormatted = AmountFormatted,
                AmountPaid = AmountPaid,
                Currency = Currency,
                EmployeeId = EmployeeId,
                EmployeeName = EmployeeName,
                EmployeeSeparationId = EmployeeSeparationId,
                ItemDescription = ItemDescription,
                Id = Id,
                TypeId = TypeId,
                TypeDescription = TypeDescription,
            };
        }

        public ManageSeparationOutstandingViewModel Extract(EmployeeSeparationOutstanding e)
        {
            return new ManageSeparationOutstandingViewModel
            {
                Amount = e.Amount,
                AmountFormatted = $"{e.Currency} {e.Amount.ToString("N")}",
                AmountPaid = e.AmountPaid,
                AmountPaidFormatted = $"{e.Currency} {e.AmountPaid.ToString("N")}",
                AmountBalance = e.AmountBalance,
                AmountBalanceFormatted = $"{e.Currency} {e.AmountBalance.ToString("N")}",
                Currency = e.Currency,
                EmployeeId = e.EmployeeId,
                EmployeeName = e.EmployeeName,
                EmployeeSeparationId = e.EmployeeSeparationId,
                ItemDescription = e.ItemDescription,
                Id = e.Id,
                TypeId = e.TypeId,
                TypeDescription = e.TypeDescription,
            };
        }
    }
}
