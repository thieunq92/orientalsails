using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Castle.Components.DictionaryAdapter;
using Portal.Modules.OrientalSails.Domain;

namespace Portal.Modules.OrientalSails.Web.Admin.Controls
{
    public partial class CruisePriceConfigCtrl : System.Web.UI.UserControl
    {
        private Cruise _cruise;
        private List<RoomClass> _roomClass = new List<RoomClass>();
        private List<RoomTypex> _roomTypes = new List<RoomTypex>();
        private IList<CruiseConfigPrice> _listPrice = new List<CruiseConfigPrice>();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void ShowTablePrice(SailsModule module, Cruise cruise, TripConfigPrice tripConfigPrice)
        {
            _cruise = cruise;
            litCruise.Text = cruise.Name;
            hidCruiseId.Value = cruise.Id.ToString();
            GetRoom(cruise);
            
            var listRoomPrice = _roomTypes.Select(roomTypex => new CruiseConfigPrice() { RoomTypeName = roomTypex.Name }).ToList();
            var listPriceCharter = new List<CruiseConfigPrice>();
            if (tripConfigPrice.Id > 0)
            {
                _listPrice = module.GetCruiseConfigPrice(tripConfigPrice.Id, cruise);
                if (_listPrice != null && _listPrice.Count > 0)
                {
                    foreach (CruiseConfigPrice configPrice in _listPrice)
                    {
                        if (configPrice.IsCharter)
                        {
                            listPriceCharter.Add(configPrice);
                        }
                    }
                }
            }
            rptRoomClass.DataSource = _roomClass;
            rptRoomClass.DataBind();

            rptRoomPriceHeader.DataSource = listRoomPrice;
            rptRoomPriceHeader.DataBind();

            rptCharterRanger.DataSource = listPriceCharter;
            rptCharterRanger.DataBind();
        }

        private void GetRoom(Cruise cruise)
        {
            foreach (Room room in cruise.Rooms)
            {
                if (!_roomClass.Contains(room.RoomClass)) _roomClass.Add(room.RoomClass);
                if (!_roomTypes.Contains(room.RoomType)) _roomTypes.Add(room.RoomType);
            }
        }

        protected void rptRoomClass_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var roomClass = e.Item.DataItem as RoomClass;
            if (roomClass != null)
            {
                var list = _roomTypes.Select(roomTypex => new CruiseConfigPrice() { RoomClassId = roomClass.Id, RoomTypeId = roomTypex.Id, RoomTypeName = roomTypex.Name }).ToList();
                if (_listPrice != null && _listPrice.Count > 0)
                {
                    foreach (CruiseConfigPrice configPrice in _listPrice)
                    {
                        if (!configPrice.IsCharter)
                        {

                            var price = list.FirstOrDefault(x => x.RoomTypeId == configPrice.RoomTypeId &&
                                                                 x.RoomClassId == configPrice.RoomClassId);
                            if (price != null)
                            {
                                price.Price = configPrice.Price;
                                price.Id = configPrice.Id;
                            }
                        }
                    }
                }
                var rptRoomPrice = e.Item.FindControl("rptRoomPrice") as Repeater;
                if (rptRoomPrice != null)
                {
                    rptRoomPrice.DataSource = list;
                    rptRoomPrice.DataBind();
                }
            }
        }
        protected void rptCharterRanger_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var charterPrice = e.Item.DataItem as CruiseConfigPrice;
            if (charterPrice != null)
            {
                TextBox txtCusFrom = e.Item.FindControl("txtCusFrom") as TextBox;
                TextBox txtCusTo = e.Item.FindControl("txtCusTo") as TextBox;
                TextBox txtPrice = e.Item.FindControl("txtPrice") as TextBox;
                CheckBox checkIsDelete = e.Item.FindControl("checkIsDelete") as CheckBox;

                if (charterPrice.CusFrom != null)
                {
                    if (txtCusFrom != null) txtCusFrom.Text = charterPrice.CusFrom.ToString();
                }
                if (charterPrice.CusTo != null)
                {
                    if (txtCusTo != null) txtCusTo.Text = charterPrice.CusTo.ToString();
                }
                if (charterPrice.Price != null)
                {
                    if (txtPrice != null) txtPrice.Text = charterPrice.Price.ToString();
                }
                if (checkIsDelete != null) checkIsDelete.Checked = charterPrice.IsDeleted;
            }
        }

        protected void btnAddRange_OnClick(object sender, EventArgs e)
        {
            var data = new List<CruiseConfigPrice>();
            foreach (RepeaterItem item in rptCharterRanger.Items)
            {
                var charterPrice = new CruiseConfigPrice();
                HiddenField hidCharterPriceId = item.FindControl("hidCharterPriceId") as HiddenField;

                TextBox txtCusFrom = item.FindControl("txtCusFrom") as TextBox;
                TextBox txtCusTo = item.FindControl("txtCusTo") as TextBox;
                TextBox txtPrice = item.FindControl("txtPrice") as TextBox;
                CheckBox checkIsDelete = item.FindControl("checkIsDelete") as CheckBox;


                if (hidCharterPriceId != null && (!string.IsNullOrEmpty(hidCharterPriceId.Value) && hidCharterPriceId.Value != "0"))
                {
                    charterPrice.Id = Convert.ToInt32(hidCharterPriceId.Value);
                }
                if (txtCusFrom != null && !string.IsNullOrEmpty(txtCusFrom.Text))
                {
                    charterPrice.CusFrom = Convert.ToInt32(txtCusFrom.Text);
                }
                if (txtCusTo != null && !string.IsNullOrEmpty(txtCusTo.Text))
                {
                    charterPrice.CusTo = Convert.ToInt32(txtCusTo.Text);
                }

                if (txtPrice != null && !string.IsNullOrEmpty(txtPrice.Text))
                {
                    charterPrice.Price = Convert.ToDecimal(txtPrice.Text);
                }
                if (checkIsDelete != null) charterPrice.IsDeleted = checkIsDelete.Checked;
                data.Add(charterPrice);
            }

            data.Add(new CruiseConfigPrice());
            rptCharterRanger.DataSource = data;
            rptCharterRanger.DataBind();
        }

        public void SaveConfigPrice(SailsModule module, TripConfigPrice tripConfigPrice)
        {
            SaveRoomPrice(module, tripConfigPrice);
            SaveCharterPrice(module, tripConfigPrice);

        }

        private void SaveCharterPrice(SailsModule module, TripConfigPrice tripConfigPrice)
        {
            foreach (RepeaterItem item in rptCharterRanger.Items)
            {
                var charterPrice = new CruiseConfigPrice();
                HiddenField hidCharterPriceId = item.FindControl("hidCharterPriceId") as HiddenField;

                TextBox txtCusFrom = item.FindControl("txtCusFrom") as TextBox;
                TextBox txtCusTo = item.FindControl("txtCusTo") as TextBox;
                TextBox txtPrice = item.FindControl("txtPrice") as TextBox;
                CheckBox checkIsDelete = item.FindControl("checkIsDelete") as CheckBox;


                if (hidCharterPriceId != null && (!string.IsNullOrEmpty(hidCharterPriceId.Value) && hidCharterPriceId.Value != "0"))
                {
                    charterPrice.Id = Convert.ToInt32(hidCharterPriceId.Value);
                }
                if (txtCusFrom != null && !string.IsNullOrEmpty(txtCusFrom.Text))
                {
                    charterPrice.CusFrom = Convert.ToInt32(txtCusFrom.Text);
                }
                if (txtCusTo != null && !string.IsNullOrEmpty(txtCusTo.Text))
                {
                    charterPrice.CusTo = Convert.ToInt32(txtCusTo.Text);
                }

                if (txtPrice != null && !string.IsNullOrEmpty(txtPrice.Text))
                {
                    charterPrice.Price = Convert.ToDecimal(txtPrice.Text);
                }
                charterPrice.IsCharter = true;
                charterPrice.CruiseId = Convert.ToInt32(hidCruiseId.Value);
                charterPrice.TripConfigPriceId = tripConfigPrice.Id;

                if (checkIsDelete != null && checkIsDelete.Checked)
                    module.Delete(charterPrice);
                else
                {
                    module.SaveOrUpdate(charterPrice);
                }
            }
        }

        private void SaveRoomPrice(SailsModule module, TripConfigPrice tripConfigPrice)
        {
            foreach (RepeaterItem item in rptRoomClass.Items)
            {
                var rptRoomPrice = item.FindControl("rptRoomPrice") as Repeater;
                if (rptRoomPrice != null)
                {
                    foreach (RepeaterItem roomItem in rptRoomPrice.Items)
                    {
                        HiddenField hidRoomTypeId = roomItem.FindControl("hidRoomTypeId") as HiddenField;
                        HiddenField hidRoomClassId = roomItem.FindControl("hidRoomClassId") as HiddenField;
                        HiddenField hidRomPriceId = roomItem.FindControl("hidRomPriceId") as HiddenField;
                        TextBox txtPrice = roomItem.FindControl("txtPrice") as TextBox;
                        var roomPrice = new CruiseConfigPrice();
                        if (hidRomPriceId != null && !string.IsNullOrEmpty(hidRomPriceId.Value) && hidRomPriceId.Value != "0")
                        {
                            roomPrice = module.GetById<CruiseConfigPrice>(Convert.ToInt32(hidRomPriceId.Value));
                        }
                        roomPrice.CruiseId = Convert.ToInt32(hidCruiseId.Value);
                        roomPrice.TripConfigPriceId = tripConfigPrice.Id;
                        if (hidRoomClassId != null && !string.IsNullOrEmpty(hidRoomClassId.Value)) roomPrice.RoomClassId = Convert.ToInt32(hidRoomClassId.Value);
                        if (hidRoomTypeId != null && !string.IsNullOrEmpty(hidRoomTypeId.Value)) roomPrice.RoomTypeId = Convert.ToInt32(hidRoomTypeId.Value);
                        if (txtPrice != null && !string.IsNullOrEmpty(txtPrice.Text)) roomPrice.Price = Convert.ToDecimal(txtPrice.Text);
                        module.SaveOrUpdate(roomPrice);
                    }
                }
            }
        }
    }
}