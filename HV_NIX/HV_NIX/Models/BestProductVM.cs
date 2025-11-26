using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HV_NIX.ViewModels
{
    public class BestProductVM
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Sold { get; set; }
        public string Thumbnail { get; set; }
    }
}
