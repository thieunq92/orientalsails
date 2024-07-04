using Portal.Modules.OrientalSails.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Modules.OrientalSails.BusinessLogic.Share
{
    public class BookingBLL
    {
        public BookingRepository BookingRepository { get; set; }

        public BookingBLL()
        {
            BookingRepository = new BookingRepository();
        }

        public void UpdatePendingBooking()
        {
            var bookings = BookingRepository.GetAll().Where(x => x.Status == Web.Util.StatusType.Pending).ToList();
            foreach (var booking in bookings)
            {
                if (booking.StartDate < DateTime.Today.AddDays(7))
                {
                    if (booking.CreatedDate > DateTime.Today.AddDays(1))
                    {
                        booking.Status = Web.Util.StatusType.Cancelled;
                    }
                }

                if (booking.StartDate > DateTime.Today.AddDays(7) && booking.StartDate < DateTime.Today.AddDays(30))
                {
                    if (booking.CreatedDate > DateTime.Today.AddDays(3))
                    {
                        booking.Status = Web.Util.StatusType.Cancelled;
                    }
                }

                if (booking.StartDate >= DateTime.Today.AddDays(30))
                {
                    if (booking.CreatedDate > DateTime.Today.AddDays(7))
                    {
                        booking.Status = Web.Util.StatusType.Cancelled;
                    }
                }
                BookingRepository.Update(booking);
            }
        }

        public void Dispose()
        {
            if (BookingRepository != null)
            {
                BookingRepository.Dispose();
                BookingRepository = null;
            }
        }
    }
}