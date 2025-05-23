﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Extensions
{
    public interface IDateDifference
    {
        void SetDates(DateTime start, DateTime end);
        int GetYears();
        int GetMonths();
        int GetDays();
    }
}
