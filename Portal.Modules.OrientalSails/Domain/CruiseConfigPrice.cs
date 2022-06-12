using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace Portal.Modules.OrientalSails.Domain {
    
    public class CruiseConfigPrice {
        public virtual int Id { get; set; }
        public virtual int? TripConfigPriceId { get; set; }
        public virtual int? CruiseId { get; set; }
        public virtual int? RoomClassId { get; set; }
        public virtual int? RoomTypeId { get; set; }
        public virtual string RoomTypeName { get; set; }
        public virtual decimal? Price { get; set; }
        public virtual int? CusFrom { get; set; }
        public virtual int? CusTo { get; set; }
        public virtual bool IsCharter { get; set; }
        public virtual bool IsDeleted { get; set; }
    }
}
