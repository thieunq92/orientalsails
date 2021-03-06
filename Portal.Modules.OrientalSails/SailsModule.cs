using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using CMS.Core;
using CMS.Core.Communication;
using CMS.Core.DataAccess;
using CMS.Core.Domain;
using CMS.Core.Service.Email;
using NHibernate.Criterion;
using Portal.Modules.OrientalSails.DataAccess;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.Util;
using CMS.Core.Service;
using CMS.Web.Util;
using CMS.Web.Components;
using System.Web;

namespace Portal.Modules.OrientalSails
{
    public partial class SailsModule : ModuleBase, INHibernateModule, IActionProvider, IActionConsumer
    {
        #region -- Const --

        #region -- Cấu hình cứng --
        public const int TRIP1 = 2;
        public const int TRIP2 = 3;
        public const int DOUBLE = 2;
        public const int TWIN = 3;
        public const int DOUBLE_COUNT = 6;
        public const int TWIN_COUNT = 10;
        public const int TRANSFER = 1;
        #endregion

        #region -- Thông tin chung --

        public const string ANONYMOUS = "Anonymous";
        public const string NOAGENCY = "Oriental sails";
        public const string IMPORTANT = "#FF7F7F";
        public const string WARNING = "#FFFF00";
        public const string GOOD = "#92D050";

        public const string TICKET = "TICKET";
        public const string MEAL = "MEAL";
        public const string KAYAKING = "KAYAKING";
        public const string SERVICE = "SERVICE";
        public const string RENT = "RENT";

        public const string ADD_BK_CUSTOMPRICE = "ADD_BK_CUSTOMPRICE";
        public const string ROOM_CUSTOMPRICE = "ROOM_CUSTOMPRICE";
        public const string PARTNERSHIP = "PARTNERSHIP";
        public const string ACCOUNT_STATUS = "ACCOUNT_STATUS";
        public const string CHECK_CHARTER = "CHECK_CHARTER";
        public const string SHOW_EXPENSE_BY_DATE = "SHOW_EXPENSE_BY_DATE";
        public const string BAR_REVENUE = "BAR_REVENUE";
        public const string NO_AGENCY_BK = "NO_AGENCY_BK";
        public const string DETAIL_SERVICE = "DETAIL_SERVICE";
        public const string OVERALL_EXPENSE = "OVERALL_EXPENSE";
        public const string USE_VND_EXPENSE = "USE_VND_EXPENSE";
        public const string APPROVED_DEFAULT = "APPROVED_DEFAULT";
        public const string PUREQUIRED = "PUREQUIRED";
        public const string PERIOD_EXPENSE_AVG = "PERIOD_EXPENSE_AVG";
        public const string APPROVED_LOCK = "APPROVED_LOCK";
        public const string CUSTOMER_PRICE = "CUSTOMER_PRICE";

        public const int GUIDE_COST = 1;
        public const int OPERATOR = 4;
        public const int TRANSPORT = 2;
        public const int DAYBOAT = 3;
        public const int HAIPHONG = 11;
        #endregion

        #region -- Thông tin tham số --
        public const string ACTION_VIEW_TRIP_PARAM = "Trip";
        private const string ACTION_VIEW_TRIP_DETAIL_PATH = "Modules/Sails/TripDetail.ascx";
        private const string ACTION_VIEW_TRIP_LIST_PATH = "Modules/Sails/TripList.ascx";
        private const string ACTION_SELECT_ROOM_PATH = "Modules/Sails/SelectRooms.ascx";
        private const string ACTION_CUSTOMER_INFO_PATH = "Modules/Sails/CustomersInfo.ascx";
        private const string ACTION_BOOKING_FINISH_PATH = "Modules/Sails/BookingFinish.ascx";
        private const string ACTION_PREFERED_ROOM_PATH = "Modules/Sails/PreferedRooms.ascx";
        public const string ACTION_ORDER_PARAM = "SelectRoom";
        public const string ACTION_CUSTOMER_INFO_PARAM = "CustomerInfo";
        public const string ACTION_PREFERED_ROOM_PARAM = "PreferedRooms";
        public const string ACTION_BOOKING_FINISH_PARAM = "Finish";
        private string CURRENT_ACTION_PARAM;
        #endregion
        #endregion

        #region -- Private Member --

        private readonly ICommonDao _commonDao;
        private readonly ISailsDao _sailsDao;
        private readonly IEmailSender _emailSender;
        private int _tripId;
        private int _optionId;

        #endregion

        #region -- Constructor --
        public static SailsModule GetInstance()
        {
            CoreRepository CoreRepository = HttpContext.Current.Items["CoreRepository"] as CoreRepository;
            int nodeId = 1;
            Node node = (Node)CoreRepository.GetObjectById(typeof(Node), nodeId);
            int sectionId = 15;
            Section section = (Section)CoreRepository.GetObjectById(typeof(Section), sectionId);
            SailsModule module = (SailsModule)ContainerAccessorUtil.GetContainer().Resolve<ModuleLoader>().GetModuleFromSection(section);
            return module;

        }

        public SailsModule(ICommonDao commonDao, ISailsDao sailsDao, IEmailSender emailSender)
        {
            _sailsDao = sailsDao;
            _commonDao = commonDao;
            _emailSender = emailSender;
        }

        #endregion

        #region -- Public Properties --

        public int TripId
        {
            get { return _tripId; }
        }

        public TripOption TripOption
        {
            get
            {
                switch (_optionId)
                {
                    case 2:
                        return TripOption.Option2;
                    case 3:
                        return TripOption.Option3;
                    default:
                        return TripOption.Option1;
                }
            }
        }

        public void SendMail(string to, string subject, string body, string cc = null)
        {
            // Đăng nhập            
            var smtpClient = new SmtpClient("mail.orientalsails.com");
            smtpClient.Credentials = new NetworkCredential("sales.c2@orientalsails.com", "os#@!123");
            //smtpClient.EnableSsl = true;

            // Địa chỉ email người gửi
            //MailAddress fromAddress = new MailAddress(UserIdentity.Email);
            var fromAddress = new MailAddress("sales@orientalsails.com");

            var message = new MailMessage();
            message.From = fromAddress;
            message.To.Add(to);
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.BodyEncoding = Encoding.UTF8;
            message.Body = body;
            if (!string.IsNullOrEmpty(cc))
                message.CC.Add(cc);

            smtpClient.Send(message);
        }

        #endregion

        #region -- Notification --
        public enum ChangeMode
        {
            Nothing,
            Cancel,
            StartDateChanges,
            RoomChanges,
            CustomerChanges
        }

        public void NotifyChanges(Booking booking, User user, ChangeMode mode, string title, List<ChangeReport> changes)
        {
            // người sửa chắc chắn phải là người nhận
            //if (mode == ChangeMode.Nothing) return;
            return;
            //TODO: chỉ tạm thời disable chức năng gửi email

            int days = (booking.StartDate - DateTime.Today).Days;
            string[] cc;

            if (booking.Agency.Sale == null || user.Id == booking.Agency.Sale.Id && mode != ChangeMode.Cancel)
            {
                // nếu là chính mình sửa và không phải cancel thì không cần báo
                return;
            }

            if (days > 15) // Nếu còn tới trên 15 ngày nữa
            {
                cc = new string[] { booking.Agency.Sale.Email };
                if (mode == ChangeMode.Cancel) // Nếu cancel trước 15 ngày
                {
                    cc = new string[] { booking.Agency.Sale.Email, user.Email, "nhan@orientalsails.com", "luong@orientalsails.com" };
                }
            }
            else if (days > 7) // Nếu còn trên 7 ngày
            {
                if (booking.Customers.Count < 10)
                {
                    cc = new string[1] { booking.Agency.Sale.Email };
                }
                else
                {
                    // Thêm cc cho kế toán
                    cc = new string[1] { booking.Agency.Sale.Email };
                }
            }
            else // Trong mọi trường hợp khác
            {
                cc = new string[1] { booking.Agency.Sale.Email };
            }

            string editUrl =
                string.Format(
                    "http://mo.orientalsails.com/Modules/Sails/Admin/BookingView.aspx?NodeId=1&SectionId=15&bi={0}",
                    booking.Id);

            var content = new StringBuilder();
            content.Append("<table></table>");
            content.Append("<table><tr><th>Field</th><th>Change from</th><th>To</th></tr></table>");
            foreach (ChangeReport change in changes)
            {
                content.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", change.Name, change.OldValue,
                                     change.NewValue);
            }

            content.AppendFormat("You can view more detail about this booking at <a href='{0}'>{0}</a>", editUrl);
            _emailSender.Send("mo@orientalsails.com", user.Email, title, content.ToString(), cc, new string[] { });
        }
        #endregion

        #region -- Session Method --
        public void SaveRoomCountData()
        {

        }
        #endregion

        #region Implementation of IActionProvider

        public ActionCollection GetOutboundActions()
        {
            ActionCollection outboundActions = new ActionCollection();
            outboundActions.Add(new CMS.Core.Communication.Action("Sails", new string[0]));
            return outboundActions;
        }

        #endregion

        #region Implementation of IActionConsumer

        public ActionCollection GetInboundActions()
        {
            ActionCollection inboundActions = new ActionCollection();
            inboundActions.Add(new CMS.Core.Communication.Action("Sails", new string[0]));
            return inboundActions;
        }

        #endregion

        /// <summary>
        /// Điều kiện để khóa phòng là: booking approved hoặc đang pending và chưa hết hạn
        /// </summary>
        /// <returns></returns>
        public ICriterion LockCrit()
        {
            //ICriterion criterion = Expression.Eq(Booking.DELETED, false);
            var criterion = Expression.And(Expression.Eq("Status", StatusType.Pending),
                                           Expression.Ge("Deadline", DateTime.Now)); // pending và chưa hết hạn
            criterion = Expression.Or(criterion, Expression.Eq("Status", StatusType.Approved)); // approved, không cần hết hạn
            criterion = Expression.And(criterion, Expression.Eq("Deleted", false));
            return criterion;
        }
    }

    public struct ChangeReport
    {
        public string Name;
        public string OldValue;
        public string NewValue;
    }
}