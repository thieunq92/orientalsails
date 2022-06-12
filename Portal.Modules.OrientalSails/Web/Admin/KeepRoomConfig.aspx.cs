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
    public partial class KeepRoomConfig : SailsAdminBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadGroup();
            }
        }

        private void LoadGroup()
        {
            rptGroup.DataSource = Module.GetCruiseGroup();
            rptGroup.DataBind();
        }

        protected void btnSave_OnClick(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in rptGroup.Items)
            {
                var ddlNumberKeepRoom = item.FindControl("ddlNumberKeepRoom") as DropDownList;
                if (ddlNumberKeepRoom != null)
                {
                    var cid = item.FindControl("cid") as HiddenField;
                    if (cid != null)
                    {
                        var group = Module.GetById<QCruiseGroup>(Convert.ToInt32(cid.Value));
                        if (!string.IsNullOrWhiteSpace(ddlNumberKeepRoom.SelectedValue))
                            group.NumberOfKeepRoom = Convert.ToInt32(ddlNumberKeepRoom.SelectedValue);
                        Module.SaveOrUpdate(group);
                    }
                }
            }
        }
        protected void rptGroup_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var group = e.Item.DataItem as QCruiseGroup;
            if (group != null)
            {
                var ddlNumberKeepRoom = e.Item.FindControl("ddlNumberKeepRoom") as DropDownList;
                if (ddlNumberKeepRoom != null)
                {
                    for (int i = 1; i <= 20; i++)
                    {
                        ddlNumberKeepRoom.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    }
                    if (group.NumberOfKeepRoom > 0)
                    {
                        ddlNumberKeepRoom.SelectedValue = group.NumberOfKeepRoom.ToString();
                    }
                }
            }
        }
    }
}