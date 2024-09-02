using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.EmployeeRecordModels
{
    public class EmployeeSeparationOutstanding
    {
        public int Id { get; set; }
        public int EmployeeSeparationId { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int TypeId { get; set; }
        public string TypeDescription { get; set; }
        public string ItemDescription { get; set; }
        public decimal Amount { get; set; }
        public string AmountFormatted { get; set; }
        public string Currency { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountBalance { get; set; }
        public bool HasPayments { get; set; }
    }
}
