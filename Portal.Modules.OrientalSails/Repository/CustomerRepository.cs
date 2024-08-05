using NHibernate;
using Portal.Modules.OrientalSails.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Criterion;
using NHibernate.Transform;
using Portal.Modules.OrientalSails.Web.Util;
using CMS.Core.Domain;

namespace Portal.Modules.OrientalSails.Repository
{
    public class CustomerRepository : RepositoryBase<Customer>
    {
        public CustomerRepository() : base() { }

        public CustomerRepository(ISession session) : base(session) { }

        public string CustomerGetNameByBookingId(int bookingId)
        {
            BookingRoom bookingRoomAlias = null;
            var customer = _session.QueryOver<Customer>()
                .JoinAlias(x => x.BookingRooms, () => bookingRoomAlias)
                .JoinQueryOver(() => bookingRoomAlias.Book)
                .Where(x => x.Id == bookingId)
                .Take(1).SingleOrDefault<Customer>();

            if (customer == null)
                return "";

            return customer.Fullname;
        }

        public int CustomerCountPaxByBookingId(int bookingId)
        {
            BookingRoom bookingRoomAlias = null;
            Booking bookingAlias = null;
            var booking = _session.QueryOver<Booking>().Where(x => x.Id == bookingId).SingleOrDefault();
            var customerCounting = 0;
            if (booking.Cruise.CruiseType == Web.Admin.Enums.CruiseType.Seating)
            {
                customerCounting = _session.QueryOver<Customer>().Fetch(x => x.Booking).Eager
                .JoinAlias(x => x.Booking, () => bookingAlias).Where(() => bookingAlias.Id == bookingId).Select(Projections.RowCount())
                .SingleOrDefault<int>();
            }
            else if (booking.Cruise.CruiseType == Web.Admin.Enums.CruiseType.Cabin)
            {
                customerCounting = _session.QueryOver<Customer>()
                .Fetch(x => x.BookingRooms).Eager
                .JoinAlias(x => x.BookingRooms, () => bookingRoomAlias)
                .Fetch(x => bookingRoomAlias.Book).Eager
                .JoinQueryOver(() => bookingRoomAlias.Book)
                .Where(x => x.Id == bookingId)
                .Select(Projections.RowCount())
                .SingleOrDefault<int>();
            }

            return customerCounting;
        }

        public int CustomerCountAdult(int bookingId)
        {
            BookingRoom bookingRoomAlias = null;
            var customerAdultCounting = _session.QueryOver<Customer>()
                .Where(x => x.IsChild == false)
                .Fetch(x => x.BookingRooms).Eager
                .JoinAlias(x => x.BookingRooms, () => bookingRoomAlias)
                .Fetch(x => bookingRoomAlias.Book).Eager
                .JoinQueryOver(() => bookingRoomAlias.Book)
                .Where(x => x.Id == bookingId)
                .Select(Projections.RowCount())
                .SingleOrDefault<int>();

            return customerAdultCounting;
        }

        public void CustomerSaveOrUpdate(Customer customer)
        {
            SaveOrUpdate(customer);
        }

        public int CustomerGetRowCountByCriterion(Cruise cruise, DateTime? date)
        {
            var query = _session.QueryOver<Customer>();
            if (cruise.CruiseType == Web.Admin.Enums.CruiseType.Cabin)
            {
                query = query.Where(x => x.Type == CustomerType.Adult || x.Type == CustomerType.Children);
            }
            else if (cruise.CruiseType == Web.Admin.Enums.CruiseType.Seating)
            {
                query = query.Where(x => x.Type == CustomerType.Adult || x.Type == CustomerType.Children || x.Type == CustomerType.Baby);
            }
            BookingRoom bookingRoomAlias = null;
            Booking bookingAlias = null;
            if (cruise.CruiseType == Web.Admin.Enums.CruiseType.Cabin)
            {
                query = query.JoinAlias(x => x.BookingRooms, () => bookingRoomAlias);
                query = query.JoinAlias(() => bookingRoomAlias.Book, () => bookingAlias);
            }
            else if (cruise.CruiseType == Web.Admin.Enums.CruiseType.Seating)
            {
                query = query.JoinAlias(x => x.Booking, () => bookingAlias);
            }


            if (cruise != null)
            {
                query = query.Where(() => bookingAlias.Cruise == cruise);
            }

            if (date != null)
            {
                if (cruise.CruiseType == Web.Admin.Enums.CruiseType.Cabin)
                {
                    query = query.Where(() => (bookingAlias.EndDate > date && bookingAlias.StartDate > date.Value.AddDays(-1) && bookingAlias.StartDate < date.Value.AddDays(1)) || (bookingAlias.StartDate < date && bookingAlias.EndDate > date));
                }
                else if (cruise.CruiseType == Web.Admin.Enums.CruiseType.Seating)
                {
                    query = query.Where(() => bookingAlias.EndDate == date && bookingAlias.StartDate == date);
                }
            }
            query = query.Where(() => bookingAlias.Deleted == false);
            query = query.Where(() => bookingAlias.Status != StatusType.Cancelled && bookingAlias.Status != StatusType.CutOff);
            query = query.Select(Projections.RowCount());
            return query.SingleOrDefault<int>();
        }

        public int CustomerGetRowCountByCriterion(SailsTrip trip, DateTime? date)
        {

            var bookingQuery = _session.QueryOver<Booking>();
            bookingQuery = bookingQuery.Where((x) => ((x.EndDate > date && x.StartDate > date.Value.AddDays(-1) && x.StartDate < date.Value.AddDays(1)) || (x.StartDate < date && x.EndDate > date)) || (x.EndDate == date && x.StartDate == date));
            if (trip != null)
            {
                bookingQuery = bookingQuery.Where((x) => x.Trip == trip);
            }
            bookingQuery = bookingQuery.Where((x) => x.Deleted == false);
            bookingQuery = bookingQuery.Where((x) => x.Status != StatusType.Cancelled && x.Status != StatusType.CutOff);

            int numberOfCustomers = 0;

            foreach (var booking in bookingQuery.List())
            {
                var query = _session.QueryOver<Customer>();
                if (booking.Cruise.CruiseType == Web.Admin.Enums.CruiseType.Cabin)
                {
                    query = query.Where(x => x.Type == CustomerType.Adult || x.Type == CustomerType.Children);
                }
                else if (booking.Cruise.CruiseType == Web.Admin.Enums.CruiseType.Seating)
                {
                    query = query.Where(x => x.Type == CustomerType.Adult || x.Type == CustomerType.Children || x.Type == CustomerType.Baby);
                }
                BookingRoom bookingRoomAlias = null;
                Booking bookingAlias = null;
                if (booking.Cruise.CruiseType == Web.Admin.Enums.CruiseType.Cabin)
                {
                    query = query.JoinAlias(x => x.BookingRooms, () => bookingRoomAlias);
                    query = query.Where((x) => bookingRoomAlias.Book == booking);
                }
                else if (booking.Cruise.CruiseType == Web.Admin.Enums.CruiseType.Seating)
                {
                    query = query.Where(x => x.Booking == booking);
                }
                query = query.Select(Projections.RowCount());
                numberOfCustomers += query.SingleOrDefault<int>();
            }
            return numberOfCustomers;
        }

        public int CustomerGetNumberOfCustomersInMonth(int month, int year, User user)
        {
            var firstDateOfMonth = new DateTime(year, month, 1);
            var lastDateOfMonth = firstDateOfMonth.AddMonths(1).AddDays(-1);
            var query = _session.QueryOver<Customer>();
            query = query.Where(x => x.Type == CustomerType.Adult || x.Type == CustomerType.Children);
            BookingRoom bookingRoomAlias = null;
            query = query.JoinAlias(x => x.BookingRooms, () => bookingRoomAlias);
            Booking bookingAlias = null;
            query = query.JoinAlias(() => bookingRoomAlias.Book, () => bookingAlias);
            BookingSale bookingSalesAlias = null;
            query = query.JoinAlias(() => bookingAlias.BookingSale, () => bookingSalesAlias);
            if (user != null)
            {
                query = query.Where(() => bookingSalesAlias.Sale == user);
            }
            query = query.Where(() => bookingAlias.Deleted == false);
            query = query.Where(() => bookingAlias.StartDate >= firstDateOfMonth && bookingAlias.StartDate <= lastDateOfMonth);
            query = query.Where(() => bookingAlias.Status == StatusType.Approved);
            query = query.Select(Projections.RowCount());
            return query.SingleOrDefault<int>();
        }
    }
}