using System.Collections;
using CMS.Core.Domain;

namespace Portal.Modules.OrientalSails.Domain
{
    #region Customer

    /// <summary>
    /// Customer object for NHibernate mapped table 'os_Customer'.
    /// </summary>
    public class ExpenseService
    {
        #region Static Columns Name

        public static string BIRTHDAY = "Birthday";
        public static string BOOKING = "Booking";
        public static string COUNTRY = "Country";
        public static string FULLNAME = "Fullname";
        public static string PASSPORT = "Passport";
        public static string BOOKINGROOM = "BookingRoom";

        #endregion

        #region Member Variables

        protected int _id;
        protected CostType _type;
        protected Expense _expense;
        protected string _name;
        protected string _phone;
        protected Agency _supplier;
        protected double _cost;
        protected double _paid;
        protected bool _isOwnService;
        protected bool _isRemoved;
        protected int _group;

        #endregion

        #region Constructors

        public ExpenseService()
        {
            _id = -1;
        }

        #endregion

        #region Public Properties

        public virtual int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual CostType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public virtual Expense Expense
        {
            get { return _expense; }
            set { _expense = value; }
        }

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        public virtual Agency Supplier
        {
            get { return _supplier; }
            set { _supplier = value; }
        }

        public virtual double Cost
        {
            get { return _cost; }
            set { _cost = value; }
        }

        public virtual double Paid
        {
            get { return _paid; }
            set { _paid = value; }
        }

        public virtual bool IsOwnService
        {
            get { return _isOwnService; }
            set { _isOwnService = value; }
        }

        public virtual bool IsRemoved
        {
            get { return _isRemoved; }
            set { _isRemoved = value; }
        }

        public virtual int Group
        {
            get
            {
                if (_group == 0)
                {
                    _group = 1;
                }
                return _group;
            }
            set { _group = value; }
        }

        public virtual int ExpenseIdRef { get; set; }
        #endregion
    }

    #endregion
}
