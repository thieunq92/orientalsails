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
    public partial class CampaignConfigPriceAdd : SailsAdminBasePage
    {
        private TripConfigPrice _tripConfigPrice = new TripConfigPrice();
        private Campaign _campaign = new Campaign();
        private IList _cruises = new List<Cruise>();
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!string.IsNullOrWhiteSpace(Request["tid"]))
            {
                _tripConfigPrice = Module.GetById<TripConfigPrice>(Convert.ToInt32(Request["tid"]));
            }
            if (!string.IsNullOrWhiteSpace(Request["cid"]))
            {
                _campaign = Module.GetById<Campaign>(Convert.ToInt32(Request["cid"]));
                if (_campaign != null)
                {
                    _tripConfigPrice = Module.GetConfigPriceByCampaign(_campaign);
                    if (_tripConfigPrice == null || _tripConfigPrice.Id <= 0)
                    {
                        _tripConfigPrice = new TripConfigPrice();
                    }
                }
            }
            if (!IsPostBack)
            {
                LoadCruise();
                LoadCampaign();
                if (_tripConfigPrice.Id > 0)
                {
                    FillInfoTripPrice();
                }
            }
        }

        private void FillInfoTripPrice()
        {

        }

        private void LoadCruise()
        {
            _cruises = Module.CruiseGetAll();
            rptCruise.DataSource = _cruises;
            rptCruise.DataBind();
        }

        private void LoadCampaign()
        {
            txtName.Text = _campaign.Name;
            txtVoucherCode.Text = _campaign.VoucherCode;
            txtVoucherTotal.Text = _campaign.VoucherTotal.ToString();
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

        protected void btnSave_OnClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Request["cid"]))
            {
                _campaign = Module.GetById<Campaign>(Convert.ToInt32(Request["cid"]));
            }
            _campaign.Name = txtName.Text;
            _campaign.VoucherCode = txtVoucherCode.Text;
            if (!string.IsNullOrWhiteSpace(txtVoucherTotal.Text))
            {
                _campaign.VoucherTotal = Convert.ToInt32(txtVoucherTotal.Text);
            }
            Module.SaveOrUpdate(_campaign);
            _tripConfigPrice.Campaign = _campaign;
            _tripConfigPrice.FromDate = new DateTime(_campaign.Year, _campaign.Month, 1);
            _tripConfigPrice.ToDate = _tripConfigPrice.FromDate.AddMonths(1).AddDays(-1);
            if (string.IsNullOrWhiteSpace(Request["tid"]) || _tripConfigPrice.Id <= 0)
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
            Response.Redirect("GoldenDayListCampaign.aspx" + GetBaseQueryString());
        }
    }
}