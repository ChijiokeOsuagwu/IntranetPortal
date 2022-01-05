using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ContentManager.Models
{
    public class ArticlesDeleteViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string ImagePath { get; set; }

    }
}
