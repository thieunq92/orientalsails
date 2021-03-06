using System;
using System.Collections;
using CMS.Core.Domain;
using System.Collections.Generic;

namespace CMS.Web.AdminArea.DAL.Domain
{
    public class Cruise
    {
        private string name;

        public virtual int Id { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual int NumberOfFloors { get; set; }
        public virtual string Name
        {
            set
            {
                name = value;
            }
            get
            {
                return name;
            }
        }

        public virtual string Description { get; set; }
        public virtual string Image { get; set; }
        public virtual string Code { get; set; }
        public virtual string RoomPlan { get; set; }
        public virtual string CruiseCode { set; get; }

        public virtual string GetModifiedCruiseName()
        {
            if (name == "Oriental Sails")
            {
                return name + " " + "1";
            }
            return name;
        }

        public virtual int Order { get; set; }
        public virtual bool IsLock { get; set; }
        public virtual string LockType { get; set; }
        public virtual DateTime? LockFromDate { get; set; }
        public virtual DateTime? LockToDate { get; set; }
    }
}