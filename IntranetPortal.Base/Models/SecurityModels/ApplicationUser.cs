using IntranetPortal.Base.Models.BaseModels;
//using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.SecurityModels
{
    public class ApplicationUser : Person
    {
        public int AccessFailedCount { get; set; }
        public string ConcurrencyStamp { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Id { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public string NormalizedEmail { get; set; }
        public string NormalizedUserName { get; set; }
        public string PasswordHash { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string SecurityStamp { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string CompanyCode { get; set; }
    }
}
