using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.SecurityModels
{
    public class UserLoginHistory
    {
        public long? ID { get; set; }
        public DateTime? LoginTime { get; set; }
        public string LoginTimeFormatted { get; set; }
        public string LoginUserName{get;set;}
        public string LoginUserID{get;set;}
        public bool LoginIsSucceful { get; set; }
        public string LoginSourceInfo { get; set; }
        public LoginType UserLoginType { get; set; }

    }

    public enum LoginType
    {
        None,
        LogIn,
        LogOut
    }
}
