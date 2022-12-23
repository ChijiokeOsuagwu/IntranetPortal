using IntranetPortal.Base.Models.BaseModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.EmployeeRecordModels
{
    public class Employee : Person
    {
        public string EmployeeID { get; set; }
        public string EmployeeNo1 { get; set; }
        public string EmployeeNo2 { get; set; }
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int? LocationID { get; set; }
        public string LocationName { get; set; }
        public string LocationType { get; set; }
        public string LocationState { get; set; }
        public string LocationCountry { get; set; }
        public string LocationHead1 { get; set; }
        public string LocationHead2 { get; set; }
        public int? DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentHead1 { get; set; }
        public string DepartmentHead2 { get; set; }
        public int? UnitID { get; set; }
        public string UnitName { get; set; }
        public string UnitHead1 { get; set; }
        public string UnitHead2 { get; set; }
        public DateTime? StartUpDate { get; set; }
        public string StartUpDateFormatted { get; set; }
        public string StartUpDesignation { get; set; }
        public int? YearsOfExperience { get; set; }
        public int? LengthOfService { get; set; }
        public string PlaceOfEngagement { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public string ConfirmationDateFormatted { get; set; }
        public string CurrentDesignation { get; set; }
        public string JobGrade { get; set; }
        public string ReportsTo1_EmployeeID { get; set; }
        public string ReportsTo1_EmployeeName { get; set; }
        public string ReportsTo2_EmployeeID { get; set; }
        public string ReportsTo2_EmployeeName { get; set; }
        public string ReportsTo3_EmployeeID { get; set; }
        public string ReportsTo3_EmployeeName { get; set; }
        public string EmploymentStatus { get; set; }
        public DateTime? DateOfLastPromotion { get; set; }
        public string DateOfLastPromotionFormatted { get; set; }
        public string OfficialEmail { get; set; }
        public string StateOfOrigin { get; set; }
        public string LgaOfOrigin { get; set; }
        public string Religion { get; set; }
        public string GeoPoliticalRegion { get; set; }
        public string NextOfKinName { get; set; }
        public string NextOfKinAddress { get; set; }
        public string NextOfKinEmail { get; set; }
        public string NextOfKinPhone { get; set; }
        public string NextOfKinRelationship { get; set; }
        public string EmployeeModifiedBy { get; set; }
        public string EmployeeModifiedDate { get; set; }
        public string EmployeeCreatedBy { get; set; }
        public string EmployeeCreatedDate { get; set; }
        public bool IsDeactivated { get; set; }
        public string DeactivationTime { get; set; }
        public string DeactivatedBy { get; set; }

        public Person ToPerson()
        {
            Person person = new Person
            {
                Address = Address,
                BirthDay = BirthDay,
                BirthMonth = BirthMonth,
                BirthYear = BirthYear,
                CreatedBy = CreatedBy,
                CreatedTime = CreatedTime,
                Email = Email,
                FirstName = FirstName,
                FullName = FullName,
                ImagePath = ImagePath,
                MaritalStatus = MaritalStatus,
                ModifiedBy = ModifiedBy,
                ModifiedTime = ModifiedTime,
                OtherNames = OtherNames,
                PersonID = PersonID,
                PhoneNo1 = PhoneNo1,
                PhoneNo2 = PhoneNo2,
                Sex = Sex,
                Surname = Surname,
                Title = Title,
                DateOfBirth = DateOfBirth,
            };

            return person;
        }
    }
}
