using System;
using System.Collections;

namespace CMS.Web.AdminArea.DAL.Domain
{
    public class RoomTypex
    {
        private string name;
 
        public virtual int Id { get; set; }
        public virtual int Capacity { get; set; }
        public virtual int Order { get; set; }
        public virtual bool IsShared { get; set; }
        public virtual bool AllowSingBook { get; set; }

        public virtual string Name
        {
            get { return name; }
            set
            {
                if (value != null && value.Length > 200)
                    throw new ArgumentOutOfRangeException("Invalid value for Name", value, value);
                name = value;
            }
        }
    }
}