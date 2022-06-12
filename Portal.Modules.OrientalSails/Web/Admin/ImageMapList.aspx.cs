using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.UI;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class ImageMapList : SailsAdminBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetListImage();
            }
        }

        private void GetListImage()
        {
            rptImages.DataSource = Module.GetAllImageGalleryByType(Request["type"], Request["Id"]);
            rptImages.DataBind();
        }

        protected void btnSaveFile_Click(object sender, EventArgs e)
        {
            HttpFileCollection httpFileCollection = Request.Files;
            for (int i = 0; i < httpFileCollection.Count; i++)
            {
                HttpPostedFile httpPostedFile = httpFileCollection[i];
                if (httpPostedFile.ContentLength > 0)
                {
                    var fileUrl = "/UserFiles/ImageGallery/" + Path.GetFileName(httpPostedFile.FileName);
                    httpPostedFile.SaveAs(Server.MapPath(fileUrl));
                    var img = new ImageGallery();
                    img.ImageUrl = fileUrl;
                    if (!string.IsNullOrEmpty(Request["type"]) && !string.IsNullOrEmpty(Request["id"]))
                    {
                        img.ImageType = Request["type"].ToUpper();
                        img.ObjectId = Request["id"].ToUpper();
                    }
                    Module.SaveOrUpdate(img);
                }
            }
            GetListImage();
        }
    }
}