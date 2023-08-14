using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal.Modules.OrientalSails.Web.Admin.Enums
{
    public enum CruiseType
    {
        [Display(Name = "Cabin Cruise")]
        Cabin = 1,
        [Display(Name = "Seating Cruise")]
        Seating = 2
    }
}