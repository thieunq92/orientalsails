using System;
using System.Text;
using System.Collections.Generic;


namespace Portal.Modules.OrientalSails.Domain {
    
    public class QCruiseGroup
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int NumberOfKeepRoom { get; set; }
        public virtual DateTime DateWarning { get; set; }
        public virtual int AvaiableRoom { get; set; }
        public virtual IList<Cruise> Cruises { get; set; }
    }
}
