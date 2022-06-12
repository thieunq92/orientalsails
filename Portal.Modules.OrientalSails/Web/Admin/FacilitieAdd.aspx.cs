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
    /// <summary>
    /// thêm sửa kho
    /// </summary>
    public partial class FacilitieAdd : SailsAdminBase
    {
        private Facilitie _facilitie = new Facilitie();
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = "Facilitie";
            if (!string.IsNullOrWhiteSpace(Request["id"]))
            {
                _facilitie = Module.GetById<Facilitie>(Convert.ToInt32(Request["id"]));
            }
            if (!IsPostBack)
            {
                if (_facilitie.Id > 0) FillInfo();
            }
        }

        private void FillInfo()
        {
            txtName.Text = _facilitie.Name;
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            _facilitie.Name = txtName.Text;
            Module.SaveOrUpdate(_facilitie);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "RefreshParentPage", "RefreshParentPage();", true);
        }
    }
}