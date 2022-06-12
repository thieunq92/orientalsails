using System;
using System.Collections;
using CMS.Core.Domain;

namespace Portal.Modules.OrientalSails.Domain
{
    public class FacilitieMap
    {
        public virtual int Id { get; set; }
        
        public virtual String FacilitieType { get; set; }
        public virtual String ObjectId { get; set; }
        public virtual Facilitie Facilitie { get; set; }
    }
}
