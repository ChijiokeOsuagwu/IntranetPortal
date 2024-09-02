using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.EmployeeRecordModels
{
    public class EmployeeSeparationPayments
    {
        public int Id { get; set; }
        public int OutstandingId { get; set; }
        public int EmployeeSeparationId { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string ItemDescription { get; set; }
        public string ItemTypeDescription { get; set; }
        public decimal PaymentAmount { get; set; }
        public string Currency { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentDetails { get; set; }
        public string  PaidBy { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
    }
}
