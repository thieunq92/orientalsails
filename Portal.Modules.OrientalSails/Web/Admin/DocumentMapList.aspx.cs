using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.UI;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class DocumentMapList : SailsAdminBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                rptDocument.DataSource = Module.GetAllDocumentByType(Request["type"], Request["Id"]);
                rptDocument.DataBind();
            }
        }
        protected void rptDocument_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var doc = e.Item.DataItem as DocumentCategory;
            if (doc != null)
            {
                var hplView = e.Item.FindControl("hplView") as HyperLink;
                var hplDownload = e.Item.FindControl("hplDownload") as HyperLink;
                if (hplView != null)
                    hplView.NavigateUrl = string.Format("DocumentViewer.aspx?NodeId={0}&SectionId={1}&docid={2}",
                        Node.Id,
                        Section.Id, doc.Id);
                if (hplDownload != null) hplDownload.NavigateUrl = doc.Url;
            }
        }
    }
}