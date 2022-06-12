using CMS.Web.AdminArea.DAL.Domain;
using System;
using System.Collections;

namespace CMS.Web.AdminArea.DAL.Domain
{
    public class RoomClass
    {
        protected string description;
        protected string name;

        public virtual int Id { get; set; }
        public virtual int Order { get; set; }
        public virtual Cruise Cruise { get; set; }
        public virtual string Name
        {
            get { return name; }
            set
            {
                if (value != null && value.Length > 50)
                    throw new ArgumentOutOfRangeException("Invalid value for Name", value, value);
                name = value;
            }
        }

        public virtual string Description
        {
            get { return description; }
            set
            {
                if (value != null && value.Length > 250)
                    throw new ArgumentOutOfRangeException("Invalid value for Name", value, value);
                description = value;
            }
        }
    }
}