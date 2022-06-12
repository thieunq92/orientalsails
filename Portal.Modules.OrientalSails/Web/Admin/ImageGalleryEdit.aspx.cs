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
    public partial class ImageGalleryEdit : SailsAdminBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["imgid"] != null)
                {
                    var gallery = Module.GetById<ImageGallery>(Convert.ToInt32(Request.QueryString["imgid"]));
                    txtName.Text = gallery.Name;
                    imgGallery.ImageUrl = gallery.ImageUrl;
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
            ImageGallery img = new ImageGallery();
            if (Request.QueryString["imgid"] != null)
            {
                img = Module.GetById<ImageGallery>(Convert.ToInt32(Request.QueryString["imgid"]));
            }
            img.Name = txtName.Text;
            if (imgUpload.HasFile)
            {
                img.ImageUrl = FileHelper.Upload(imgUpload, "ImageGallery/");
            }
            if (!string.IsNullOrEmpty(Request["type"]) && !string.IsNullOrEmpty(Request["ObjId"]))
            {
                img.ImageType = Request["type"].ToUpper();
                img.ObjectId = Request["ObjId"].ToUpper();
            }
            Module.SaveOrUpdate(img, UserIdentity);
            ClientScript.RegisterStartupScript(this.GetType(), "RefreshParentPage", "<script>window.parent.location.href = window.parent.location.href;</script>");
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var doc = Module.GetById<ImageGallery>(Convert.ToInt32(Request.QueryString["imgid"]));
            Module.Delete(doc);
            ClientScript.RegisterStartupScript(this.GetType(), "RefreshParentPage", "<script>window.parent.location.href = window.parent.location.href;</script>");
        }
    }
}