using System;
using System.Collections;
using CMS.Core.Domain;
using Portal.Modules.OrientalSails.Web.Util;
using System.Collections.Generic;

namespace Portal.Modules.OrientalSails.Domain
{
    public class BookingRoom
    {
        private IList<Customer> _customers;
        private int _adult;
        private int _virtualAdult;
        private double _total;
        private IList<Customer> _realCustomers;

        public BookingRoom() { }

        public BookingRoom(Booking book, Room room, RoomClass roomClass, RoomTypex roomType)
        {
            Book = book;
            Room = room;
            RoomClass = roomClass;
            RoomType = roomType;
        }

        public virtual int Id { get; set; }
        public virtual Booking Book { get; set; }
        public virtual Room Room { get; set; }
        public virtual RoomClass RoomClass { get; set; }
        public virtual RoomTypex RoomType { get; set; }
        public virtual BookingType BookingType { get; set; }
        public virtual bool HasChild { get; set; }
        public virtual bool HasBaby { get; set; }
        public virtual bool IsSingle { get; set; }

        public virtual IList<Customer> Customers
        {
            get
            {
                if (_customers == null)
                {
                    _customers = new List<Customer>();
                }
                return _customers;
            }
            set { _customers = value; }
        }

        public virtual int Booked { get; set; }
        public virtual bool IsLocked { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual User ModifiedBy { get; set; }
        public virtual string TourComment { get; set; }
        public virtual string RoomComment { get; set; }
        public virtual string FoodComment { get; set; }
        public virtual string StaffComment { get; set; }
        public virtual string GuideComment { get; set; }
        public virtual string CustomerIdea { get; set; }
        public virtual string RoomNumber { get; set; }

        public virtual int Adult
        {
            get
            {
                if (_adult == 0)
                {
                    if (IsSingle)
                    {
                        // Neu la book single thi kiem tra xem co phai la child single khong thong qua client 1 (vi single luon chi co client 1)
                        if (Customers.Count > 0)
                        {
                            if (((Customer)Customers[0]).IsChild)
                            {
                                _adult = 0;
                            }
                            _adult = 1;
                        }
                    }
                    else
                    {
                        // Nếu là book bình thường thì phải đếm
                        foreach (Customer customer in RealCustomers)
                        {
                            if (customer.Type == CustomerType.Adult && !customer.IsChild)
                            {
                                _adult += 1;
                            }
                        }
                    }
                }
                return _adult;
            }
        }

        public virtual double Total
        {
            get { return _total; }
            set { _total = value; }
        }

        // Số khách ảo (dùng cho việc tính chỗ trống)
        public virtual int VirtualAdult
        {
            get
            {
                if (_virtualAdult == 0)
                {
                    foreach (Customer customer in Customers)
                    {
                        if (customer.Type == CustomerType.Adult)
                        {
                            _virtualAdult += 1;
                        }
                    }
                }
                return _virtualAdult;
            }
        }

        public virtual int Child
        {
            get
            {
                int child = 0;
                //if (IsSingle)
                //{
                //    child = 1 - Adult;
                //}
                //else
                //{
                //    child = 3 - Adult;
                //}

                if (HasChild)
                {
                    return child + 1;
                }
                return child;
            }
        }

        public virtual int Baby
        {
            get
            {
                if (HasBaby)
                {
                    return 1;
                }
                return 0;
            }
        }

        // Danh sách khách thực
        public virtual IList<Customer> RealCustomers
        {
            get
            {
                if (_realCustomers == null)
                {
                    _realCustomers = new List<Customer>();
                    int space = 0;
                    int maxSpace = 3;
                    if (IsSingle) maxSpace = 1;
                    foreach (Customer customer in Customers)
                    {
                        switch (customer.Type)
                        {
                            case CustomerType.Adult:
                                //if (space < maxSpace)
                                //{
                                _realCustomers.Add(customer);
                                //space += 1;
                                //}
                                break;
                            case CustomerType.Children:
                                if (HasChild)
                                {
                                    _realCustomers.Add(customer);
                                }
                                break;
                            case CustomerType.Baby:
                                if (HasBaby)
                                {
                                    _realCustomers.Add(customer);
                                }
                                break;
                        }
                    }
                }
                return _realCustomers;
            }
        }

        public virtual string CustomerName
        {
            get
            {
                string name = string.Empty;
                foreach (Customer customer in RealCustomers)
                {
                    if (!string.IsNullOrEmpty(customer.Fullname))
                    {
                        name = name + customer.Fullname + "<br/>";
                    }
                }
                return name;
            }
        }

        public virtual string RoomTypeName
        {
            get { return string.Format("{0} {1}", RoomClass.Name, RoomType.Name); }
        }

        public virtual string RoomTypeKey
        {
            get { return string.Format("{0}", RoomClass.Name.ToLower()); }
        }

        public virtual bool HasAddExtraBed { get; set; }
        public virtual bool HasAddAdult { get; set; }

        public virtual double Calculate(SailsModule Module, IList policies, double childPrice, double agencySup, Agency agency)
        {
            // Lấy về kiểu phòng
            RoomClass rclass = RoomClass;
            RoomTypex rtype = RoomType;

            int adult = Adult;
            int child = Child;

            double price = Calculate(rclass, rtype, adult, child, IsSingle, Book.Trip, Book.Cruise, Book.TripOption, Book.StartDate, Module,
                      policies, childPrice, agencySup, agency);

            return price;
        }

        public static double Calculate(RoomClass rclass, RoomTypex rtype, int adult, int child, bool isSingle, SailsTrip trip, Cruise cruise, TripOption option, DateTime startDate, SailsModule Module, IList policies, double childPrice, double agencySup, Agency agency)
        {
            // Lấy về bảng giá áp dụng cho thời điểm xuất phát
            if (trip == null) return 0;
            SailsPriceConfig priceConfig = Module.SailsPriceConfigGet(rclass, rtype, trip, cruise, option, startDate,
                                                                      BookingType.Double, agency);

            if (priceConfig == null)
            {
                return 0;
                //throw new PriceException(string.Format("There isn't any price for {0} {1} room in trip {2} on {3}", rclass.Name, rtype.Name, trip.Name, startDate));
            }
            #region -- Giá phòng --
            double price;
            // Biến để lưu giá trị single supplement nếu là booking single
            double singlesup = 0;
            if (isSingle)
            {
                if (agencySup > 0)
                {
                    singlesup = agencySup;
                }
                else
                {
                    singlesup = priceConfig.SpecialPrice; //Module.ApplyPriceFor(priceConfig.SpecialPrice, policies);
                }
            }

            // Tính giá phòng theo người lớn

            // Đơn giá của phòng (đã áp dụng chính sách)
            double unitPrice;
            if (priceConfig.Table.Agency == null)
            {
                unitPrice = Module.ApplyPriceFor(priceConfig.NetPrice, policies);
            }
            else
            {
                unitPrice = priceConfig.NetPrice;
            }
            if (rtype.IsShared)
            {
                // Giá phòng twin phòng đơn giá x số lượng người lớn / 2 + đơn giá x tỉ lệ dành cho child x số child / 2
                // (Thực ra adult = 1/2, child =0/1)
                price = unitPrice * adult / 2 + unitPrice * child * childPrice / 100 / 2;
                // Cộng thêm singlesup (nếu không phải single thì là + 0)
                price += singlesup;
            }
            else
            {
                // Giá phòng double phòng đơn giá x số lượng người lớn / 2 + đơn giá x tỉ lệ dành cho child x số child / 2
                price = unitPrice * adult / 2 + unitPrice * child * childPrice / 100 / 2;
                // Cộng thêm singlesup (nếu không phải single thì là + 0)
                price += singlesup;
            }
            #endregion

            return price;
        }

        private const double MARK_UP = 1.2;

        public static double RoomPrice(SailsModule module, RoomClass rclass, RoomTypex rtype, SailsTrip trip, Cruise cruise, TripOption option, DateTime startDate)
        {
            if (trip == null) return 0;
            SailsPriceConfig rolePrice = module.SailsPriceConfigGet(rclass, rtype, trip, cruise, option, startDate,
                                                                      BookingType.Double, null);
            if (rolePrice != null)
            {
                return rolePrice.NetPrice;
            }

            SailsPriceConfig priceConfig = module.SailsPriceConfigGet2(rclass, rtype, trip, cruise, option, startDate, BookingType.Double, null);
            if (priceConfig != null)
            {
                return priceConfig.NetPrice * MARK_UP;
            }

            return -1;
        }
    }

    public class BookingTypeClass : NHibernate.Type.EnumStringType
    {
        public BookingTypeClass()
            : base(typeof(BookingType), 50)
        {
        }
    }
}