using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.Admin.Controls;
using Portal.Modules.OrientalSails.Web.UI;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class TripConfigPriceAdd : SailsAdminBasePage
    {
        private TripConfigPrice _tripConfigPrice = new TripConfigPrice();
        private IList _cruises = new List<Cruise>();
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadCruise();
            if (!string.IsNullOrWhiteSpace(Request["tid"]))
            {
                _tripConfigPrice = Module.GetById<TripConfigPrice>(Convert.ToInt32(Request["tid"]));
            }
            if (!IsPostBack)
            {
                LoadTrip();
                if (_tripConfigPrice.Id > 0)
                {
                    FillInfoTripPrice();
                }
                ddlTrips_OnSelectedIndexChanged(null, null);
            }
        }

        private void FillInfoTripPrice()
        {
            if (_tripConfigPrice.Trip != null) ddlTrips.SelectedValue = _tripConfigPrice.Trip.Id.ToString();
            txtFromDate.Text = _tripConfigPrice.FromDate.ToString("dd/MM/yyyy");
            txtToDate.Text = _tripConfigPrice.ToDate.ToString("dd/MM/yyyy");
        }

        private void LoadCruise()
        {
            _cruises = Module.CruiseGetAll();
        }

        private void LoadTrip()
        {
            var trips = Module.TripGetAll(false);
            ddlTrips.DataSource = trips; // Danh sách trip luôn được get về trước khi gọi tới hàm bind trips
            ddlTrips.DataTextField = "Name";
            ddlTrips.DataValueField = "Id";
            ddlTrips.DataBind();
        }

        protected void rptCruise_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var cruise = e.Item.DataItem as Cruise;
            if (cruise != null && cruise.Id > 0)
            {
                CruisePriceConfigCtrl cruisePriceConfigCtrl = (CruisePriceConfigCtrl)e.Item.FindControl("CruisePriceConfig");
                cruisePriceConfigCtrl.ShowTablePrice(Module, cruise, _tripConfigPrice);
            }
        }

        protected void ddlTrips_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var list = new List<Cruise>();
            foreach (Cruise cruise in _cruises)
            {
                if (cruise.Trips.Any(x => x.Id.ToString() == ddlTrips.SelectedValue))
                {
                    if (!list.Contains(cruise)) list.Add(cruise);
                }
            }
            rptCruise.DataSource = list;
            rptCruise.DataBind();
        }

        protected void btnSave_OnClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ddlTrips.SelectedValue))
                _tripConfigPrice.Trip = Module.GetById<SailsTrip>(Convert.ToInt32(ddlTrips.SelectedValue));
            if (!string.IsNullOrWhiteSpace(txtFromDate.Text))
            {
                _tripConfigPrice.FromDate = DateTime.ParseExact(txtFromDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrWhiteSpace(txtToDate.Text))
            {
                _tripConfigPrice.ToDate = DateTime.ParseExact(txtToDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            if (string.IsNullOrWhiteSpace(Request["tid"]))
            {
                _tripConfigPrice.CreatedBy = UserIdentity;
                _tripConfigPrice.CreatedDate = DateTime.Now;
            }
            _tripConfigPrice.ModifiedBy = UserIdentity;
            _tripConfigPrice.ModifyDate = DateTime.Now;
            _tripConfigPrice.Enable = false;
            Module.SaveOrUpdate(_tripConfigPrice);
            if (rptCruise.Items.Count > 0)
            {
                foreach (RepeaterItem item in rptCruise.Items)
                {
                    CruisePriceConfigCtrl cruisePriceConfigCtrl = (CruisePriceConfigCtrl)item.FindControl("CruisePriceConfig");
                    cruisePriceConfigCtrl.SaveConfigPrice(Module, _tripConfigPrice);
                }
            }
            _tripConfigPrice.Enable = true;
            Module.SaveOrUpdate(_tripConfigPrice);
            Response.Redirect("TripConfigPriceList.aspx" + GetBaseQueryString());
        }
    }
}