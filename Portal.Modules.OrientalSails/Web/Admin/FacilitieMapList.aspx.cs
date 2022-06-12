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
    public partial class FacilitieMapList : SailsAdminBasePage
    {
        private IList<FacilitieMap> _facilitieMaps = new List<FacilitieMap>();
        private string _facilitiType = "";
        private string _objId = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            _facilitiType = Request["type"];
            _objId = Request["id"];
            if (!IsPostBack)
            {
                _facilitieMaps = Module.GetAllFacilitiesByType(_facilitiType, _objId);
                rptFacilitie.DataSource = Module.GetAllFacilities();
                rptFacilitie.DataBind();
            }
        }

        protected void rptFacilitie_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var item = e.Item.DataItem as Facilitie;
            if (item != null)
            {
                var mapId = e.Item.FindControl("mapId") as HiddenField;
                var chkMap = e.Item.FindControl("chkMap") as CheckBox;

                if (_facilitieMaps != null)
                {
                    var mapItem =
                        _facilitieMaps.FirstOrDefault(x => x.FacilitieType.ToUpper() == _facilitiType.ToUpper()
                                                           && x.ObjectId == _objId && x.Facilitie.Id == item.Id);
                    if (mapItem != null)
                    {
                        if (mapId != null && chkMap != null)
                        {
                            mapId.Value = mapItem.Id.ToString();
                            chkMap.Checked = true;
                        }
                    }
                }
            }
        }

        protected void btnSave_OnClick(object sender, EventArgs e)
        {
            if (rptFacilitie.Items.Count > 0)
            {
                foreach (RepeaterItem item in rptFacilitie.Items)
                {
                    var mapId = item.FindControl("mapId") as HiddenField;
                    var chkMap = item.FindControl("chkMap") as CheckBox;
                    var facilitiId = item.FindControl("facilitiId") as HiddenField;

                    if (mapId != null && chkMap != null && facilitiId != null)
                    {
                        if (chkMap.Checked)
                        {
                            if (string.IsNullOrEmpty(mapId.Value))
                            {
                                var facmap = new FacilitieMap();
                                facmap.Facilitie = Module.GetById<Facilitie>(Convert.ToInt32(facilitiId.Value));
                                facmap.FacilitieType = _facilitiType.ToUpper();
                                facmap.ObjectId = _objId;
                                Module.SaveOrUpdate(facmap);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(mapId.Value))
                            {
                                var facmap = Module.GetById<FacilitieMap>(Convert.ToInt32(mapId.Value));
                                Module.Delete(facmap);
                            }
                        }
                    }
                }
            }
            Page.ClientScript.RegisterStartupScript(this.GetType(), "RefreshParentPage", "RefreshParentPage();", true);
        }
    }
}