using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.Admin.Controls;
using Portal.Modules.OrientalSails.Web.UI;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class TripConfigPriceList : SailsAdminBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pagerProduct.AllowCustomPaging = true;
                pagerProduct.PageSize = 20;
                if (!IsPostBack)
                {
                    LoadListTripPrice();
                }
            }
        }
        private void LoadListTripPrice()
        {
            int count = 0;
            rptTripPrice.DataSource = Module.GetTripPriceConfig(pagerProduct.PageSize, pagerProduct.CurrentPageIndex, out count);
            rptTripPrice.DataBind();
            pagerProduct.VirtualItemCount = count;
        }
    }
}