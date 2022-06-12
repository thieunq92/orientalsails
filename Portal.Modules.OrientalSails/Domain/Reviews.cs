using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CMS.Core.Domain;


namespace Portal.Modules.OrientalSails.Domain {
    
    public class Reviews {
        public virtual int Id { get; set; }
        public virtual string ReviewType { get; set; }
        public virtual string Body { get; set; }
        public virtual string FullName { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual bool? Disable { get; set; }
        public virtual DateTime? CreateDate { get; set; }
        public virtual float? Rating { get; set; }
        public virtual string ObjectId { get; set; }
    }
}
