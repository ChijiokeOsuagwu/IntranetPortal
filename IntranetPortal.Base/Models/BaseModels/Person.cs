using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.BaseModels
{
    public class Person
    {
        public string PersonID { get; set; }
        public string Title { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string OtherNames { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string PhoneNo1 { get; set; }
        public string PhoneNo2 { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string MaritalStatus { get; set; }
        public int? BirthDay { get; set; }
        public int? BirthMonth { get; set; }
        public int? BirthYear { get; set; }
        public string DateOfBirth { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedTime { get; set; }
        public string ImagePath { get; set; }
    }
}
