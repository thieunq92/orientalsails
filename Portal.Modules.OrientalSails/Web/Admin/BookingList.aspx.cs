using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using NHibernate.Criterion;
using log4net;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.UI;
using Portal.Modules.OrientalSails.Web.Util;
using Portal.Modules.OrientalSails.BusinessLogic;
using System.Web;
using Portal.Modules.OrientalSails.Utils;
using Portal.Modules.OrientalSails.BusinessLogic.Share;
using Portal.Modules.OrientalSails.Enums;
using CMS.Core.Domain;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class BookingList : SailsAdminBase
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(BookingList));

        private UserBLL userBLL;
        public UserBLL UserBLL
        {
            get
            {
                if (userBLL == null)
                    userBLL = new UserBLL();
                return userBLL;
            }
        }

        private BookingListBLL bookingListBLL;

        public BookingListBLL BookingListBLL
        {
            get
            {
                if (bookingListBLL == null)
                {
                    bookingListBLL = new BookingListBLL();
                }
                return bookingListBLL;
            }
        }

        private PermissionBLL permissionBLL;
        public PermissionBLL PermissionBLL
        {
            get
            {
                if (permissionBLL == null)
                    permissionBLL = new PermissionBLL();
                return permissionBLL;
            }
        }

        public User CurrentUser
        {
            get
            {
                return UserBLL.UserGetCurrent();
            }
        }

        public bool CanViewTotal
        {
            get
            {
                return PermissionBLL.UserCheckPermission(CurrentUser.Id, (int)PermissionEnum.VIEW_TOTAL_BY_DATE);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ControlLoadData();
                BookingLoadData();
            }
        }
        protected void Page_Unload(object sender, EventArgs e)
        {
            if (bookingListBLL != null)
            {
                bookingListBLL.Dispose();
                bookingListBLL = null;
            }
        }

        protected void rptBookingList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Booking booking = e.Item.DataItem as Booking;
            if (booking != null)
            {
                Literal ltrNumberPax = e.Item.FindControl("ltrNumberPax") as Literal;
                if (ltrNumberPax != null)
                {
                    ltrNumberPax.Text = BookingListBLL.CustomerCountPaxByBookingId(booking.Id).ToString();
                }

                Literal ltrCustomerName = e.Item.FindControl("ltrCustomerName") as Literal;
                if (ltrCustomerName != null)
                {
                    ltrCustomerName.Text = BookingListBLL.CustomerGetNameByBookingId(booking.Id);
                }

                HtmlTableRow row = (HtmlTableRow)e.Item.FindControl("trItem");
                if (booking.Status == StatusType.Pending)
                {
                    row.Attributes.Add("class", "--pending");
                }

                if (booking.Status == StatusType.Approved)
                {
                    row.Attributes.Add("class", "--approved");
                }

                var plhNote = e.Item.FindControl("plhNote") as PlaceHolder;
                plhNote.Visible = !String.IsNullOrEmpty(booking.Note) ? true : false;
            }
        }

        protected void buttonSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.Url.GetLeftPart(UriPartial.Path) + QueryStringBuildByCriterion());
        }

        public void ControlLoadData()
        {
            IList<SailsTrip> trips = BookingListBLL.TripGetAll();
            ddlTrip.DataSource = trips;
            ddlTrip.DataTextField = "Name";
            ddlTrip.DataValueField = "Id";
            ddlTrip.DataBind();
            ddlTrip.Items.Insert(0, "-- Trip --");

            IEnumerable<Cruise> cruises = BookingListBLL.CruiseGetAll();
            ddlCruises.DataSource = cruises;
            ddlCruises.DataTextField = "Name";
            ddlCruises.DataValueField = "Id";
            ddlCruises.DataBind();
            ddlCruises.Items.Insert(0, "-- Cruise --");

            if (!string.IsNullOrEmpty(Request.QueryString["ti"]))
            {
                ddlTrip.SelectedValue = Request.QueryString["ti"];
            }

            if (!string.IsNullOrEmpty(Request.QueryString["sd"]))
            {
                txtStartDate.Text = Request.QueryString["sd"];
            }

            if (!string.IsNullOrEmpty(Request.QueryString["bi"]))
            {
                txtBookingId.Text = Request.QueryString["bi"];
            }

            if (!string.IsNullOrEmpty(Request.QueryString["cn"]))
            {
                txtCustomerName.Text = Request.QueryString["cn"];
            }

            if (!string.IsNullOrEmpty(Request.QueryString["ci"]))
            {
                ddlCruises.SelectedValue = Request.QueryString["ci"];
            }

        }

        public void BookingLoadData()
        {
            int count = 0;
            var bookings = bookingListBLL.BookingGetByQueryString(CurrentUser, Request.QueryString, pagerBookings.PageSize,
                pagerBookings.CurrentPageIndex, out count);
            rptBookingList.DataSource = bookings;
            pagerBookings.AllowCustomPaging = true;
            pagerBookings.VirtualItemCount = count;
            rptBookingList.DataBind();
        }

        public string QueryStringBuildByCriterion()
        {
            NameValueCollection nvcQueryString = new NameValueCollection();
            nvcQueryString.Add("NodeId", "1");
            nvcQueryString.Add("SectionId", "15");

            if (ddlTrip.SelectedIndex > 0)
            {
                nvcQueryString.Add("ti", ddlTrip.SelectedValue);
            }

            if (!string.IsNullOrEmpty(txtStartDate.Text))
            {
                nvcQueryString.Add("sd", txtStartDate.Text);
            }

            if (!string.IsNullOrEmpty(txtBookingId.Text))
            {
                nvcQueryString.Add("bi", txtBookingId.Text);
            }

            if (!string.IsNullOrEmpty(txtCustomerName.Text))
            {
                nvcQueryString.Add("cn", txtCustomerName.Text);
            }

            if (ddlCruises.SelectedIndex > 0)
            {
                nvcQueryString.Add("ci", ddlCruises.SelectedValue);
            }

            var criterions = (from key in nvcQueryString.AllKeys
                              from value in nvcQueryString.GetValues(key)
                              select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value))).ToArray();

            return "?" + string.Join("&", criterions);
        }

        public string QueryStringBuildByStatus(StatusType? status)
        {
            NameValueCollection nvcQueryString = new NameValueCollection(Request.QueryString);
            nvcQueryString.Remove("page");
            nvcQueryString.Remove("s");
            if (status != null)
                nvcQueryString.Add("s", ((int)status).ToString());

            var criterions = (from key in nvcQueryString.AllKeys
                              from value in nvcQueryString.GetValues(key)
                              select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value))).ToArray();

            return "?" + string.Join("&", criterions);
        }

        public string QueryStringBuildByStatus()
        {
            return QueryStringBuildByStatus(null);
        }

        public string UrlGetByAll()
        {
            return Request.Url.GetLeftPart(UriPartial.Path) + QueryStringBuildByStatus();
        }

        public string UrlGetByApproved()
        {
            return Request.Url.GetLeftPart(UriPartial.Path) + QueryStringBuildByStatus(StatusType.Approved);
        }

        public string UrlGetByPending()
        {
            return Request.Url.GetLeftPart(UriPartial.Path) + QueryStringBuildByStatus(StatusType.Pending);
        }

        public string UrlGetByCancelled()
        {
            return Request.Url.GetLeftPart(UriPartial.Path) + QueryStringBuildByStatus(StatusType.Cancelled);
        }

        public string BookingCountApproved()
        {
            return BookingListBLL.BookingCountByStatusAndDate(CurrentUser, StatusType.Approved, DateTime.Today).ToString();
        }

        public string BookingCountPending()
        {
            return bookingListBLL.BookingCountByStatusAndDate(CurrentUser, StatusType.Pending, DateTime.Today).ToString();
        }

        public string BookingCountCancelled()
        {
            return bookingListBLL.BookingCountByStatusAndDate(CurrentUser, StatusType.Cancelled, DateTime.Today).ToString();
        }

    }
}