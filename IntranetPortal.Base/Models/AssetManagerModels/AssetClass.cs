﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.AssetManagerModels
{
    public class AssetClass
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }

    }
}
