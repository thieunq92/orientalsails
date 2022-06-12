using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.Web.Util;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.UI;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class ReviewMapEdit : SailsAdminBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {


                if (Request.QueryString["rid"] != null)
                {
                    var doc = Module.GetById<Reviews>(Convert.ToInt32(Request.QueryString["rid"]));
                    txtBody.Text = doc.Body;
                    txtFullName.Text = doc.FullName;
                    btnDelete.Visible = true;
                }
                else
                {
                    btnDelete.Visible = false;
                }
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            Reviews review;
            if (Request.QueryString["rid"] != null)
            {
                review = Module.GetById<Reviews>(Convert.ToInt32(Request.QueryString["rid"]));
            }
            else
            {
                review = new Reviews();
            }
            review.FullName = txtFullName.Text;
            review.Body = txtBody.Text;

            if (!string.IsNullOrEmpty(Request["type"]) && !string.IsNullOrEmpty(Request["ObjId"]))
            {
                review.ReviewType = Request["type"].ToUpper();
                review.ObjectId = Request["ObjId"].ToUpper();
            }
            Module.SaveOrUpdate(review, UserIdentity);
            ClientScript.RegisterStartupScript(this.GetType(), "RefreshParentPage", "<script>window.parent.location.href = window.parent.location.href;</script>");
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var doc = Module.GetById<Reviews>(Convert.ToInt32(Request.QueryString["rid"]));
            Module.Delete(doc);
            ClientScript.RegisterStartupScript(this.GetType(), "RefreshParentPage", "<script>window.parent.location.href = window.parent.location.href;</script>");
        }
    }
}