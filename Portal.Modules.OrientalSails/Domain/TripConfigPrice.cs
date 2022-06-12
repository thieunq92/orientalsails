using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CMS.Core.Domain;


namespace Portal.Modules.OrientalSails.Domain
{

    public class TripConfigPrice
    {
        public virtual int Id { get; set; }
        public virtual SailsTrip Trip { get; set; }
        public virtual QAgentLevel AgentLevel { get; set; }
        public virtual Campaign Campaign { get; set; }
        public virtual DateTime FromDate { get; set; }
        public virtual DateTime ToDate { get; set; }
        public virtual User CreatedBy { get; set; }
        public virtual User ModifiedBy { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime ModifyDate { get; set; }
        public virtual bool Enable { get; set; }
    }
}
