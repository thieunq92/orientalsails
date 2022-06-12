using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.Admin;

namespace Portal.Modules.OrientalSails.Web.Controls
{
    public partial class QuotationIssueSelect : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public string QuotationName { get; set; }
        public void SetQuotationSelect(QuotationSelect quotationSelect, SailsModule module)
        {
            if (!string.IsNullOrWhiteSpace(quotationSelector.Value))
                quotationSelect.QQuotation = module.GetById<QQuotation>(Convert.ToInt32(quotationSelector.Value));
            quotationSelect.AgencyLevelCode = ddlAgentLevel.SelectedValue;
        }
        public void DisplayQuotationSelect(QuotationSelect quotationSelect, SailsModule module)
        {
            if (quotationSelect.QQuotation != null && quotationSelect.QQuotation.Id > 0)
            {
                var name = string.Format("{0:dd/mm/yyyy}-{1:dd/mm/yyyy} {2}", quotationSelect.QQuotation.Validfrom, quotationSelect.QQuotation.Validto, quotationSelect.QQuotation.GroupCruise.Name);
                QuotationName = name;
                quotationSelector.Value = quotationSelect.QQuotation.Id.ToString();
            }
            ddlAgentLevel.SelectedValue = quotationSelect.AgencyLevelCode;
        }
    }
}