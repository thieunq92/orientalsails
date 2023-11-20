using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal.Modules.OrientalSails.Web.Admin.Enums
{
    public enum TransferLocationType
    {
        None = 0,
        [Display(Name = "Hà Nội")]
        HaNoi = 1,
        [Display(Name = "Hạ Long")]
        HaLong = 2
    }
}