using System;
using System.Collections;
using CMS.Web.AdminArea.DAL.Domain;

namespace CMS.Web.AdminArea.DAL.Domain
{
    public class Room
    {
        private bool isAvailable = true;
        private string name;
        public virtual int Id { get; set; }
        public virtual int Order { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual RoomClass RoomClass { get; set; }
        public virtual RoomTypex RoomType { get; set; }
        public virtual int Adult { get; set; }
        public virtual int Child { get; set; }
        public virtual int Baby { get; set; }
        public virtual Cruise Cruise { get; set; }
        public virtual int Floor { get; set; }

        public virtual bool IsAvailable
        {
            get { return isAvailable; }
            set { isAvailable = value; }
        }
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

        public virtual string RoomName
        {
            get { return string.Format("{0} {1}", RoomClass.Name, RoomType.Name); }
        }

        public virtual bool IsAvailable3D { get; set; }
    }
}