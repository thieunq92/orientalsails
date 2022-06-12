using System;
using System.Collections;

namespace Portal.Modules.OrientalSails.Domain
{

    /// <summary>
    /// Room object for NHibernate mapped table 'os_Room'.
    /// </summary>
    public class ImageGallery
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string ImageUrl { get; set; }
        public virtual string ImageType { get; set; }
        public virtual string ObjectId { get; set; }
    }

}