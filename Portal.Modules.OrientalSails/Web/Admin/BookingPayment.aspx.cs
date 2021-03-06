using System;
using System.Collections;
using System.Web.UI.WebControls;
using CMS.Web.Util;
using NHibernate.Criterion;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.UI;
using Portal.Modules.OrientalSails.Web.Util;
using Portal.Modules.OrientalSails.BusinessLogic;
using Portal.Modules.OrientalSails.Utils;
using CMS.Core.Domain;
using Portal.Modules.OrientalSails.BusinessLogic.Share;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class BookingPayment : SailsAdminBase
    {
        private BookingPaymentBLL bookingPaymentBLL;
        private UserBLL userBLL;
        private User currentUser;

        private double paid;
        private double paidBase;
        private double total;

        private double _totalUSD;
        private double _totalVND;

        public BookingPaymentBLL BookingPaymentBLL
        {
            get
            {
                if (bookingPaymentBLL == null)
                    bookingPaymentBLL = new BookingPaymentBLL();
                return bookingPaymentBLL;
            }
        }

        public UserBLL UserBLL
        {
            get
            {
                if (userBLL == null)
                {
                    userBLL = new UserBLL();
                }
                return userBLL;
            }
        }
        public User CurrentUser
        {
            get
            {
                if (currentUser == null)
                {
                    currentUser = UserBLL.UserGetCurrent();
                }
                return currentUser;
            }
        }

        public Booking Booking
        {
            get
            {
                Booking booking = null;
                try
                {
                    if (Request.QueryString["bi"] != null)
                        booking = BookingPaymentBLL.BookingGetById(Convert.ToInt32(Request.QueryString["bi"]));
                }
                catch (Exception) { }
                return booking;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ltrBookingCode.Text = string.Format("OS{0:00000}", Booking.Id);

                if (Booking.Agency != null)
                    ltrAgency.Text = Booking.Agency.Name;

                ltrStartDate.Text = Booking.StartDate.ToString("dd/MM/yyyy");

                ltrService.Text = Booking.Trip.Name;

                if (Booking.Trip.NumberOfOptions > 1)
                    ltrService.Text += Booking.TripOption.ToString();

                if (Booking.IsTotalUsd)
                {
                    ltrRevenueUSD.Text = Booking.Value.ToString("#,0.#") + " USD";
                }
                else
                {
                    ltrRevenueUSD.Text = Booking.Value.ToString("#,0.#") + " VND";
                }

                if (Booking.CurrencyRate == 0)
                {
                    if (Booking.IsTotalUsd)
                    {
                        txtAppliedRate.Text = BookingPaymentBLL.USDRateGetByDate(DateTime.Now).Rate.ToString("#,0.#");
                    }
                }
                else
                {
                    txtAppliedRate.Text = Booking.CurrencyRate.ToString("#,0.#");
                }

                if (Booking.IsTotalUsd == false)
                {
                    txtAppliedRate.Text = "1";
                }
                txtPaid.Text = 0.ToString("#,0.#");
                txtPaidBase.Text = 0.ToString("#,0.#");

                total = Booking.Total;
                paid = Booking.Paid;
                paidBase = Booking.PaidBase;

                chkPaid.Checked = Booking.IsPaid;

                if (Booking.CurrencyRate == 0)
                {
                    if (Booking.IsTotalUsd)
                    {
                        Booking.CurrencyRate = BookingPaymentBLL.USDRateGetByDate(DateTime.Now).Rate;
                    }
                }
                if (Booking.IsTotalUsd == false && Booking.CurrencyRate <= 0)
                {
                    Booking.CurrencyRate = 1;
                }

                if (Booking.IsTotalUsd)
                {
                    ltrRemainVND.Text = "(" + Booking.AgencyReceivable.ToString("#,0.#") + " VND)";
                    ltrRemainUSD.Text = (Booking.AgencyReceivable / Booking.CurrencyRate).ToString("#,0.##") + " USD";
                }
                else
                {
                    ltrRemainVND.Text = Booking.AgencyReceivable.ToString("#,0.#") + " VND";
                    txtPaid.Visible = false;
                    trUsdPaid.Visible = false;
                    txtAppliedRate.Attributes.Add("readonly", "true");
                }

                rptPaymentHistory.DataSource = Booking.Transactions;
                rptPaymentHistory.DataBind();
            }
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            if (bookingPaymentBLL != null)
            {
                bookingPaymentBLL.Dispose();
                bookingPaymentBLL = null;
            }

            if (userBLL != null)
            {
                userBLL.Dispose();
                userBLL = null;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            double appliedRate;
            double currentRate = BookingPaymentBLL.USDRateGetByDate(DateTime.Now).Rate;
            if (txtAppliedRate.Text != string.Empty)
            {
                appliedRate = Convert.ToDouble(txtAppliedRate.Text);
            }
            else
            {
                appliedRate = 0;
            }


            double usdPaid = Convert.ToDouble(txtPaid.Text);
            double vndPaid = Convert.ToDouble(txtPaidBase.Text);

            usdPaid += Booking.Paid;
            vndPaid += Booking.PaidBase;
            try
            {
                if (Booking.IsTotalUsd && Convert.ToDouble(txtAppliedRate.Text) > 1)
                {
                    Booking.CurrencyRate = Convert.ToDouble(txtAppliedRate.Text);
                }
            }
            catch (Exception) { }

            double paidUSD = 0.0;
            try
            {
                paidUSD = Convert.ToDouble(txtPaid.Text);
            }
            catch (Exception) { }

            double paidVND = 0.0;
            try
            {
                paidVND = Convert.ToDouble(txtPaidBase.Text);
            }
            catch (Exception) { }

            double totalPaidUSD = 0;
            double totalPaidVND = 0;

            foreach (var t in Booking.Transactions)
            {
                if (t.TransactionType == Transaction.BOOKING)
                {
                    totalPaidUSD += t.USDAmount;
                    totalPaidVND += t.VNDAmount;
                }
            }

            var transaction = new Transaction()
            {
                Agency = Booking.Agency,
                Booking = Booking,
                CreatedBy = CurrentUser,
                CreatedDate = DateTime.Now,
                IsExpense = false,
                TransactionType = Transaction.BOOKING,
                USDAmount = paidUSD,
                VNDAmount = paidVND,
                Note = txtNote.Text,
            };
            if (Convert.ToDouble(txtAppliedRate.Text) > 1)
            {
                transaction.AppliedRate = Convert.ToDouble(txtAppliedRate.Text);
            }
            BookingPaymentBLL.TransactionSaveOrUpdate(transaction);

            totalPaidUSD += transaction.USDAmount;
            totalPaidVND += transaction.VNDAmount;

            Booking.Paid = totalPaidUSD;
            Booking.PaidBase = totalPaidVND;

            bool temp;
            temp = Booking.IsPaid;
            Booking.IsPaid = false;
            if (Booking.AgencyReceivable == 0)
            {
                chkPaid.Checked = true;
            }
            Booking.IsPaid = temp;

            if (Booking.IsPaid != chkPaid.Checked)
            {
                Booking.IsPaid = chkPaid.Checked;
                if (Booking.IsPaid)
                {
                    Booking.PaidDate = DateTime.Now;
                }
            }
            Booking.AgencyConfirmed = false;
            BookingPaymentBLL.BookingSaveOrUpdate(Booking);
            ClientScript.RegisterStartupScript(this.GetType(), "RefreshParentPage", "<script>window.parent.location.href = window.parent.location.href;</script>");

        }

        protected void rptPaymentHistory_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem is Transaction)
            {
                var transaction = (Transaction)e.Item.DataItem;
                ValueBinder.BindLiteral(e.Item, "litTime", transaction.CreatedDate.ToString("dd/MM/yyyy HH:mm"));
                ValueBinder.BindLiteral(e.Item, "litPayBy", transaction.AgencyName);
                ValueBinder.BindLiteral(e.Item, "litAmountUSD", transaction.USDAmount.ToString("#,0.#"));
                ValueBinder.BindLiteral(e.Item, "litAmountVND", transaction.VNDAmount.ToString("#,0.#"));
                ValueBinder.BindLiteral(e.Item, "litCreatedBy", transaction.CreatedBy.UserName);
                ValueBinder.BindLiteral(e.Item, "litNote", transaction.Note);

                _totalUSD += transaction.USDAmount;
                _totalVND += transaction.VNDAmount;
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                ValueBinder.BindLiteral(e.Item, "litTotalUSD", _totalUSD.ToString("#,0.#"));
                ValueBinder.BindLiteral(e.Item, "litTotalVND", _totalVND.ToString("#,0.#"));
            }
        }


    }
}