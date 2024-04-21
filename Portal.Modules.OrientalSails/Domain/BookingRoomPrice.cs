using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Modules.OrientalSails.Domain
{
    public class BookingRoomPrice
    {
        public virtual int BookingRoomPriceId { get; set; }
        public virtual Booking Booking { get; set; }
        public virtual RoomClass RoomClass { get; set; }
        public virtual RoomTypex RoomType { get; set; }
        public virtual double PriceOfRoom { get; set; }
        public virtual double PriceOfAddAdult { get; set; }
        public virtual double PriceOfAddChild { get; set; }
        public virtual double PriceOfAddBaby { get; set; }
        public virtual double PriceOfExtrabed { get; set; }

    }
}