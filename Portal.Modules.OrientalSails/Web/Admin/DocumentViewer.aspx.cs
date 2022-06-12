using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.UI;
using Portal.Modules.OrientalSails.Web.Util;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class DocumentViewer : SailsAdminBase
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["docid"] != null)
            {
                var doc = Module.DocumentGetById(Convert.ToInt32(Request.QueryString["docid"]));
                if (DocUtil.IsMediaFile(doc.Url))
                {
                    iframeDoc.Visible = false;
                    iframePdf.Visible = false;
                    lblMsg.Text = "Không thể xem file này ";
                    lblMsg.Visible = true;
                }
                if (!doc.Url.Contains(".pdf"))
                {
                    iframeDoc.Visible = true;
                    iframeDoc.Src = "https://view.officeapps.live.com/op/embed.aspx?src=http://" + HttpContext.Current.Request.Url.Host + doc.Url;
                }
                else
                {
                    iframePdf.Visible = true;
                    iframePdf.Src = "/ViewerJS/#.." + doc.Url;
                }
            }
        }
        
    }
}