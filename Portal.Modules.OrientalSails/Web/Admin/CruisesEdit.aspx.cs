using System;
using System.Globalization;
using System.Web.UI.WebControls;
using CMS.Web.Util;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.Admin.Enums;
using Portal.Modules.OrientalSails.Web.UI;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class CruisesEdit : SailsAdminBase
    {
        private Cruise _cruise;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGroup();
            }
            
            if (Request.QueryString["cruiseid"] != null)
            {
                int cruiseid = Convert.ToInt32(Request.QueryString["cruiseid"]);
                _cruise = Module.CruiseGetById(cruiseid);
                if (!IsPostBack)
                {
                    BindCruise();
                }
            }
            else
            {
                _cruise = new Cruise();
            }
            if (!IsPostBack)
            {
                BindTrips();
                BindCruiseType();
            }
        }

        private void BindGroup()
        {
            ddlGroup.DataSource = Module.GetCruiseGroup();
            ddlGroup.DataTextField = "Name";
            ddlGroup.DataValueField = "Id";
            ddlGroup.DataBind();
        }

        private void BindCruiseType()
        {
            ddlCruiseType.Items.Insert(0, new ListItem(CruiseType.Cabin.ToString(), 1.ToString()));
            ddlCruiseType.Items.Insert(1, new ListItem(CruiseType.Seating.ToString(), 2.ToString()));
            ddlCruiseType.SelectedValue = ((int)_cruise.CruiseType).ToString();
        }

        private void BindCruise()
        {
            txtCode.Text = _cruise.Code;
            txtCruiseCode.Text = _cruise.CruiseCode;
            textBoxName.Text = _cruise.Name;
            txtDescription.Text = _cruise.Description;
            txtFloor.Text = _cruise.NumberOfFloors.ToString();
            txtNumberOfSeat.Text = _cruise.NumberOfSeat.ToString();
            if (_cruise.Group != null) ddlGroup.SelectedValue = _cruise.Group.Id.ToString();
            if (!string.IsNullOrEmpty(_cruise.RoomPlan))
            {
                hplRoomPlan.Visible = true;
                litRoomPlan.Visible = false;
                hplRoomPlan.NavigateUrl = _cruise.RoomPlan;
            }
            else
            {
                hplRoomPlan.Visible = false;
                litRoomPlan.Visible = true;
            }
            chkLock.Checked = _cruise.IsLock;
            ddlLockType.SelectedValue = _cruise.LockType;
            if (_cruise.IsLock)
            {

                ddlLockType.Enabled = true;
                txtLockFrom.ReadOnly = true;
                if (_cruise.LockFromDate != null) txtLockFrom.Text = _cruise.LockFromDate.Value.ToString("dd/MM/yyyy");
                if (_cruise.LockType == "From")
                {
                    txtLockTo.ReadOnly = false;
                }
                else if (_cruise.LockType == "FromTo")
                {
                    txtLockTo.ReadOnly = true;
                    if (_cruise.LockToDate != null) txtLockTo.Text = _cruise.LockToDate.Value.ToString("dd/MM/yyyy");
                }
            }
        }

        private void BindTrips()
        {
            rptTrips.DataSource = Module.TripGetAll(true);
            rptTrips.DataBind();
        }

        private void GetCruise()
        {
            _cruise.Code = txtCode.Text;
            _cruise.CruiseCode = txtCruiseCode.Text;
            _cruise.Name = textBoxName.Text;
            _cruise.Description = txtDescription.Text;
            _cruise.NumberOfSeat = Convert.ToInt32(txtNumberOfSeat.Text);
            if (!string.IsNullOrEmpty(ddlGroup.SelectedValue)) _cruise.Group = Module.GetById<QCruiseGroup>(Convert.ToInt32(ddlGroup.SelectedValue));
            if (!string.IsNullOrWhiteSpace(txtFloor.Text)) _cruise.NumberOfFloors = Convert.ToInt32(txtFloor.Text);
            if (fileRoomPlan.HasFile)
            {
                _cruise.RoomPlan = FileHelper.Upload(fileRoomPlan);
            }
            _cruise.IsLock = chkLock.Checked;
            _cruise.LockType = ddlLockType.SelectedValue;
            _cruise.CruiseType = (CruiseType)Convert.ToInt32(ddlCruiseType.SelectedValue);
            if (!string.IsNullOrEmpty(txtLockFrom.Text)) _cruise.LockFromDate = DateTime.ParseExact(txtLockFrom.Text, "dd/MM/yyyy",
                  CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(txtLockTo.Text)) _cruise.LockToDate = DateTime.ParseExact(txtLockTo.Text, "dd/MM/yyyy",
                CultureInfo.InvariantCulture);
        }

        protected void buttonSave_Click(object sender, EventArgs e)
        {
            GetCruise();
            Module.SaveOrUpdate(_cruise, UserIdentity);

            foreach (RepeaterItem item in rptTrips.Items)
            {
                CheckBox chkTrip = (CheckBox)item.FindControl("chkTrip");
                HiddenField hiddenId = (HiddenField)item.FindControl("hiddenId");
                SailsTrip trip = Module.TripGetById(Convert.ToInt32(hiddenId.Value));
                CruiseTrip cr = Module.CruiseTripGet(_cruise, trip);
                if (chkTrip.Checked && cr.Id <= 0)
                {
                    Module.SaveOrUpdate(cr);
                }
                if (!chkTrip.Checked && cr.Id > 0)
                {
                    Module.Delete(cr);
                }
            }
            PageRedirect(string.Format("CruisesList.aspx?NodeId={0}&SectionId={1}", Node.Id, Section.Id));
        }

        protected void rptTrips_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem is SailsTrip)
            {
                SailsTrip trip = (SailsTrip)e.Item.DataItem;
                CheckBox chkTrip = e.Item.FindControl("chkTrip") as CheckBox;
                if (chkTrip != null)
                {
                    chkTrip.Text = trip.Name;
                    if (_cruise.Trips.Contains(trip))
                    {
                        chkTrip.Checked = true;
                    }
                }
            }
        }
    }
}
