using NHibernate;
using Portal.Modules.OrientalSails.BusinessLogic.Share;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Repository;
using Portal.Modules.OrientalSails.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Modules.OrientalSails.BusinessLogic
{
    public class BookingViewBLL
    {
        public BookingRepository BookingRepository { get; set; }
        public TripRepository TripRepository { get; set; }
        public CruiseRepository CruiseRepository { get; set; }
        public AgencyRepository AgencyRepository { get; set; }
        public BookingRoomRepository BookingRoomRepository { get; set; }
        public BookingHistoryRepository BookingHistoryRepository { get; set; }
        public LockedRepository LockedRepository { get; set; }
        public CustomerRepository CustomerRepository { get; set; }
        public UserBLL UserBLL { get; set; }
        public SeriesRepository SeriesRepository { get; set; }
        public BusTypeRepository BusTypeRepository { get; set; }
        public CommissionRepository CommissionRepository { get; set; }
        public ServiceOutsideRepository ServiceOutsideRepository { get; set; }
        public ServiceOutsideDetailRepository ServiceOutsideDetailRepository { get; set; }

        public BookingViewBLL()
        {
            BookingRepository = new BookingRepository();
            TripRepository = new TripRepository();
            CruiseRepository = new CruiseRepository();
            AgencyRepository = new AgencyRepository();
            BookingRoomRepository = new BookingRoomRepository();
            BookingHistoryRepository = new BookingHistoryRepository();
            LockedRepository = new LockedRepository();
            CustomerRepository = new CustomerRepository();
            UserBLL = new UserBLL();
            SeriesRepository = new SeriesRepository();
            BusTypeRepository = new BusTypeRepository();
            CommissionRepository = new CommissionRepository();
            ServiceOutsideRepository = new ServiceOutsideRepository();
            ServiceOutsideDetailRepository = new ServiceOutsideDetailRepository();
        }

        public void Dispose()
        {
            if (BookingRepository != null)
            {
                BookingRepository.Dispose();
                BookingRepository = null;
            }
            if (TripRepository != null)
            {
                TripRepository.Dispose();
                TripRepository = null;
            }
            if (CruiseRepository != null)
            {
                CruiseRepository.Dispose();
                CruiseRepository = null;
            }
            if (AgencyRepository != null)
            {
                AgencyRepository.Dispose();
                AgencyRepository = null;
            }
            if (BookingRoomRepository != null)
            {
                BookingRoomRepository.Dispose();
                BookingRoomRepository = null;
            }
            if (BookingHistoryRepository != null)
            {
                BookingHistoryRepository.Dispose();
                BookingHistoryRepository = null;
            }
            if (LockedRepository != null)
            {
                LockedRepository.Dispose();
                LockedRepository = null;
            }
            if (CustomerRepository != null)
            {
                CustomerRepository.Dispose();
                CustomerRepository = null;
            }
            if (UserBLL != null)
            {
                UserBLL.Dispose();
                UserBLL = null;
            }
            if (SeriesRepository != null)
            {
                SeriesRepository.Dispose();
                SeriesRepository = null;
            }
            if (BusTypeRepository != null)
            {
                BusTypeRepository.Dispose();
                BusTypeRepository = null;
            }
            if (CommissionRepository != null)
            {
                CommissionRepository.Dispose();
                CommissionRepository = null;
            }
            if (ServiceOutsideRepository != null)
            {
                ServiceOutsideRepository.Dispose();
                ServiceOutsideRepository = null;
            }
            if (ServiceOutsideDetailRepository != null)
            {
                ServiceOutsideDetailRepository.Dispose();
                ServiceOutsideDetailRepository = null;
            }
        }

        public Booking BookingGetById(int bookingId)
        {
            return BookingRepository.BookingGetById(bookingId);
        }

        public ServiceOutside ServiceOutsideGetById(int serviceOutsideId)
        {
            return ServiceOutsideRepository.GetById(serviceOutsideId);
        }
        public Commission CommissionGetById(int commissionId)
        {
            return CommissionRepository.GetById(commissionId);
        }

        public IList<SailsTrip> TripGetAll()
        {
            return TripRepository.TripGetAll();
        }

        public IEnumerable<Cruise> CruiseGetAll()
        {
            return CruiseRepository.CruiseGetAll();
        }

        public IList<Agency> AgencyGetAll()
        {
            return AgencyRepository.AgencyGetAll();
        }

        public void BookingSaveOrUpdate(Booking booking)
        {
            if (booking.Id > 0)
            {
                booking.ModifiedBy = UserBLL.UserGetCurrent();
                booking.ModifiedDate = DateTime.Now;
            }
            else
            {
                booking.CreatedBy = UserBLL.UserGetCurrent();
                booking.CreatedDate = DateTime.Now;
            }
            booking.RoomCount = booking.BookingRooms.Count();
            booking.CustomerCount = booking.Customers.Count();         

            BookingRepository.SaveOrUpdate(booking);
        }

        public void BookingRoomSaveOrUpdate(BookingRoom bookingRoom)
        {
            BookingRoomRepository.SaveOrUpdate(bookingRoom);
        }

        public BookingRoom BookingRoomGetById(int bookingRoomId)
        {
            return BookingRoomRepository.BookingRoomGetById(bookingRoomId);
        }

        public void BookingRoomDelete(BookingRoom bookingRoom)
        {
            BookingRoomRepository.Delete(bookingRoom);
        }

        public IList<BookingHistory> BookingHistoryGetByBookingId(int bookingId)
        {
            return BookingHistoryRepository.BookingHistoryGetByBookingId(bookingId);
        }

        public Cruise CruiseGetById(int cruiseId)
        {
            return CruiseRepository.CruiseGetById(cruiseId);
        }

        public SailsTrip TripGetById(int tripId)
        {
            return TripRepository.TripGetById(tripId);
        }

        public IList<Locked> LockedGetBy(DateTime? startDate, DateTime? endDate, int cruiseId)
        {
            return LockedRepository.LockedGetBy(startDate, endDate, cruiseId);
        }

        public void CustomerSaveOrUpdate(Customer customer)
        {
            CustomerRepository.CustomerSaveOrUpdate(customer);
        }

        public Series SeriesGetBySeriesCode(string seriesCode)
        {
            return SeriesRepository.SeriesGetBySeriesCode(seriesCode);
        }

        public IQueryOver<BusType,BusType> BusTypeGetAll()
        {
            return BusTypeRepository.BusTypeGetAll();
        }

        public BusType BusTypeGetById(int busTypeId)
        {
            return BusTypeRepository.BusTypeGetById(busTypeId);
        }

        public void CommissionSaveOrUpdate(Commission commission)
        {
            CommissionRepository.SaveOrUpdate(commission);
        }

        public IList<Commission> CommissionGetAllByBookingId(int restaurantBookingId)
        {
            return CommissionRepository.CommissionGetAllByBookingId(restaurantBookingId);
        }

        public void CommissionDelete(Commission commission)
        {
            CommissionRepository.Delete(commission);
        }

        public void ServiceOutsideSaveOrUpdate(ServiceOutside serviceOutside)
        {
            ServiceOutsideRepository.SaveOrUpdate(serviceOutside);
        }

        public IList<ServiceOutside> ServiceOutsideGetAllByBookingId(int restaurantBookingId)
        {
            return ServiceOutsideRepository.ServiceOutsideGetAllByBookingId(restaurantBookingId);
        }

        public void ServiceOutsideDelete(ServiceOutside serviceOutside)
        {
            ServiceOutsideRepository.Delete(serviceOutside);
        }

        public ServiceOutsideDetail ServiceOutsideDetailGetById(int serviceOutsideDetailId)
        {
            return ServiceOutsideDetailRepository.GetById(serviceOutsideDetailId);
        }

        public void ServiceOutsideDetailSaveOrUpdate(ServiceOutsideDetail serviceOutsideDetail)
        {
            ServiceOutsideDetailRepository.SaveOrUpdate(serviceOutsideDetail);
        }

        public IList<ServiceOutsideDetail> ServiceOutsideDetailGetAllByServiceOutsideId(int serviceOutsideId)
        {
            return ServiceOutsideDetailRepository.ServiceOutsideDetailGetAllByServiceOutsideId(serviceOutsideId);
        }

        public void ServiceOutsideDetailDelete(ServiceOutsideDetail serviceOutsideDetail)
        {
            ServiceOutsideDetailRepository.Delete(serviceOutsideDetail);
        }
    }
}