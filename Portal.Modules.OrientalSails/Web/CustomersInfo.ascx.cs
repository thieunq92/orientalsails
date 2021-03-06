using System;
using System.Collections;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CMS.Core.Domain;
using CMS.Web.UI;
using CMS.Web.Util;
using log4net;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.Controls;
using Portal.Modules.OrientalSails.Web.UI;
using Portal.Modules.OrientalSails.Web.Util;

namespace Portal.Modules.OrientalSails.Web
{
    public partial class CustomersInfo : BaseModuleControl
    {
        #region -- Private Member --

        private readonly ILog _logger = LogManager.GetLogger(typeof(CustomersInfo));
        private IList _dataSource;

        private new SailsModule Module
        {
            get { return base.Module as SailsModule; }
        }

        #endregion

        #region -- PAGE EVENTS --
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    LocalizeControls();

                    if (PageEngine.CuyahogaUser != null)
                    {
                        panelCustomer.Visible = true;
                        liAnonymous.Visible = false;
                    }

                    // Lấy về số lượng config
                    int configCount = Convert.ToInt32(Session["ConfigCount"]);

                    // Tạo data source là một arraylist trắng
                    _dataSource = new ArrayList();
                    IList _selectedRooms = new ArrayList();

                    for (int i = 1; i <= configCount; i++)
                    {

                        #region -- Lấy dữ liệu từ session --
                        // Lấy về từng cấu hình class, type, number of room
                        // Lấy về cấu hình thứ i - theo quy tắc định trước key sẽ là Configi
                        string sessionStr = Session["Config" + i].ToString();

                        string[] data = sessionStr.Split(',');

                        // Lấy về ba dữ liệu trong mỗi config là class, type và số lượng
                        int classId = Convert.ToInt32(data[0]);
                        int typeId = Convert.ToInt32(data[1]);
                        int number = Convert.ToInt32(data[2]);

                        // Lấy thông tin các phòng đã prefer select nếu người dùng thông qua một bước chọn phòng
                        if (Session["SelectedRoom"] != null && Session["SelectedRoom"].ToString() != string.Empty)
                        {
                            // Lấy danh sách số phòng đã chọn
                            int roomCount = Convert.ToInt32(Session["SelectedRoom"]);
                            _selectedRooms = new ArrayList();
                            for (int ii = 1; ii <= roomCount; ii++)
                            {
                                Room room = Module.RoomGetById(Convert.ToInt32(Session["Room" + ii]));
                                _selectedRooms.Add(room);
                            }
                        }
                        #endregion

                        // Biến dùng để lưu các booking tạm thời (chưa đủ người để fill full phòng)
                        IList previousBooking = new ArrayList();

                        // Add toàn bộ thông tin cấu hình này vào datasource
                        #region -- Xử lý với từng dữ liệu trong cấu hình --
                        for (int j = 1; j <= number; j++)
                        {
                            BookingRoom bookingRoom = new BookingRoom();
                            bookingRoom.RoomClass = Module.RoomClassGetById(classId);
                            bookingRoom.RoomType = Module.RoomTypexGetById(typeId);
                            bookingRoom.Room = null;

                            // Xử lý nếu là phòng share
                            if (bookingRoom.RoomType.Id != SailsModule.TWIN)
                            {
                                SetRoomForBooking(_selectedRooms, bookingRoom);
                                _dataSource.Add(bookingRoom);
                            }
                            else
                            {
                                // Nếu là phòng share, kiểm tra xem đã có trong danh sách phòng chưa full không
                                // Nếu có thì remove phòng đã có trong danh sách phòng này và insert vào data source
                                bool isFirst = true;
                                foreach (BookingRoom info in previousBooking)
                                {
                                    if (info.RoomClass.Id == bookingRoom.RoomClass.Id)
                                    {
                                        isFirst = false;
                                        previousBooking.Remove(info);
                                        _dataSource.Add(info);
                                        // Chỉ set room khi đã full (do trên trang chọn phòng chỉ cho chọn phòng với các đối tượng full
                                        SetRoomForBooking(_selectedRooms, bookingRoom);
                                        break;
                                    }
                                }
                                if (isFirst)
                                {
                                    previousBooking.Add(bookingRoom);
                                }
                            }
                        }
                        #endregion

                        // Sau khi đã xử lý hết, còn phòng nào cũng đều cho vào datasource
                        foreach (BookingRoom booked in previousBooking)
                        {
                            booked.Booked = 1;// Mới chỉ có một người book
                            _dataSource.Add(booked);
                        }
                    }
                    rptRoomList.DataSource = _dataSource;
                    rptRoomList.DataBind();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error when Page_Load in CustomersInfo control", ex);
                throw;
            }
        }
        #endregion

        private static void SetRoomForBooking(IList _selectedRooms, BookingRoom bookingRoom)
        {
            // Nếu tồn tại số phòng đã chọn session thì add thông tin vào booking
            foreach (Room room in _selectedRooms)
            {
                if (room.IsAvailable && room.RoomClass.Id == bookingRoom.RoomClass.Id && room.RoomType.Id == bookingRoom.RoomType.Id)
                {
                    bookingRoom.Room = room;
                    room.IsAvailable = false;
                    break;
                }
            }
        }

        #region -- CONTROL EVENTS --
        public void rptRoomList_itemDataBound(object sender, RepeaterItemEventArgs e)
        {
            rptRoomList_itemDataBound(sender, e, Module, false, null, null, null);
        }

        /// <summary>
        /// Dùng cho sự kiện data bound của một danh sách booking room
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="module"></param>
        /// <param name="customPrice"></param>
        /// <param name="policies"></param>
        /// <param name="page"></param>
        /// <param name="roomTypes"></param>
        public static void rptRoomList_itemDataBound(object sender, RepeaterItemEventArgs e, SailsModule module, bool customPrice, IList policies, SailsAdminBasePage page, ListItemCollection roomTypes)
        {
            if (e.Item.ItemType != ListItemType.Header)
            {
                BookingRoom item = e.Item.DataItem as BookingRoom;
                if (item != null)
                {

                    #region -- Thông tin thường --
                    //var hplChangRoom = e.Item.FindControl("hplChangRoom") as HyperLink;
                    //if (hplChangRoom != null && item.Room != null)
                    //{
                    //    var editRoom = string.Format("editRoom({0},{1})", item.Book.Id, item.Room.Id);
                    //    hplChangRoom.Attributes.Add("onclick", editRoom);
                    //}
                    var hplRoomName = (HyperLink)e.Item.FindControl("hplRoomName");
                    if (item.Room != null)
                    {
                        Label label_RoomId = (Label)e.Item.FindControl("label_RoomId");
                        label_RoomId.Text = item.Room.Id.ToString();
                        if (item.Room != null)
                        {
                            hplRoomName.Text = string.Format("{3}. {2}: &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0} {1}",
                                                             item.Room.RoomClass.Name, item.Room.RoomType.Name,
                                                             item.Room.Name, e.Item.ItemIndex + 1);
                        }
                        else
                        {
                            hplRoomName.Text = string.Format("{2}. {0} {1}",
                                                             item.RoomClass.Name, item.RoomType.Name, e.Item.ItemIndex + 1);
                        }
                        var editRoom = string.Format("editRoom({0},{1})", item.Book.Id, item.Room.Id);
                        hplRoomName.Attributes.Add("onclick", editRoom);
                    }
                    else
                    {
                        //hplRoomName.Text = string.Format("Room {2}: &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0} {1}",
                        //                                 item.RoomClass.Name, item.RoomType.Name,
                        //                                 e.Item.ItemIndex + 1);
                        hplRoomName.Text = string.Format("{3}. Room {2}: &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0} {1}",
                            item.RoomClass.Name, item.RoomType.Name,
                            "N/A", e.Item.ItemIndex + 1);
                        var editRoom = string.Format("selectRoom({0},{1})", item.Book.Id, item.Id);
                        hplRoomName.Attributes.Add("onclick", editRoom);
                    }

                    HiddenField hiddenRoomClassId = (HiddenField)e.Item.FindControl("hiddenRoomClassId");
                    HiddenField hiddenRoomTypeId = (HiddenField)e.Item.FindControl("hiddenRoomTypeId");
                    HiddenField hiddenRoomId = (HiddenField)e.Item.FindControl("hiddenRoomId");

                    hiddenRoomClassId.Value = item.RoomClass.Id.ToString();
                    hiddenRoomTypeId.Value = item.RoomType.Id.ToString();
                    if (item.Room != null) hiddenRoomId.Value = item.Room.Id.ToString();

                    if (item.Booked == 1 && item.RoomType.Id == SailsModule.TWIN)
                    {
                        e.Item.FindControl("trCustomer2").Visible = false;
                        e.Item.FindControl("trExtra").Visible = false;
                    }
                    HtmlInputCheckBox checkboxAddExtraBed = (HtmlInputCheckBox)e.Item.FindControl("checkboxAddExtraBed");


                    HtmlControl trExtraBed = (HtmlControl)e.Item.FindControl("trExtraBed");

                    var checkBoxAddAdult = (HtmlInputCheckBox)e.Item.FindControl("checkBoxAddAdult");

                    trExtraBed.Attributes.CssStyle["display"] = "none";
                    //if (item.RoomType.Id != SailsModule.TWIN && item.RoomType.Id != SailsModule.DOUBLE)
                    //{
                    //    e.Item.FindControl("trExtraBed").Visible = false;
                    //    checkBoxAddAdult.Visible = false;
                    //}
                    //else
                    //{
                        string scriptExtraBed = string.Format(@"toggleVisible('{0}');",
                            trExtraBed.ClientID);
                        checkBoxAddAdult.Attributes.Add("onclick", scriptExtraBed);
                    //}


                    HtmlInputCheckBox checkBoxAddChild = (HtmlInputCheckBox)e.Item.FindControl("checkBoxAddChild");
                    HtmlControl trChild = (HtmlControl)e.Item.FindControl("trChild");

                    string scriptChild = string.Format(@"toggleVisible('{0}');",
                                                       trChild.ClientID);
                    checkBoxAddChild.Attributes.Add("onclick", scriptChild);

                    HtmlInputCheckBox checkBoxAddBaby = (HtmlInputCheckBox)e.Item.FindControl("checkBoxAddBaby");
                    HtmlControl trBaby = (HtmlControl)e.Item.FindControl("trBaby");
                    string scriptBaby = string.Format(@"toggleVisible('{0}');", trBaby.ClientID);
                    checkBoxAddBaby.Attributes.Add("onclick", scriptBaby);

                    HtmlInputCheckBox checkBoxSingle = (HtmlInputCheckBox)e.Item.FindControl("checkBoxSingle");
                    HtmlControl trCustomer2 = (HtmlControl)e.Item.FindControl("trCustomer2");
                    string scriptCustomer = string.Format(@"toggleVisible('{0}');",
                                                          trCustomer2.ClientID);
                    checkBoxSingle.Attributes.Add("onclick", scriptCustomer);

                    #endregion

                    #region -- (back-end) --

                    bool isSecond = false;
                    // Load customer info đã có

                    #region -- customer info --

                    foreach (Customer customer in item.Customers)
                    {
                        if (customer != null && customer.HasAddAdult)
                        {
                            CustomerExtraInfoRowInput customerExtraBed = e.Item.FindControl("customerExtraBed") as CustomerExtraInfoRowInput;
                            customerExtraBed.GetCustomer(customer, module);

                        }
                        else
                        {
                            if (customer != null && customer.Type == CustomerType.Adult)
                            {
                                if (!isSecond)
                                {
                                    CustomerInfoRowInput customer1 = e.Item.FindControl("customer1") as CustomerInfoRowInput;
                                    Repeater rptService1 = e.Item.FindControl("rptServices1") as Repeater;
                                    if (customer1 != null && rptService1 != null)
                                    {
                                        customer1.GetCustomer(customer, module);
                                        if (page.DetailService)
                                        {
                                            CustomerServiceRepeaterHandler handler =
                                                new CustomerServiceRepeaterHandler(customer, module);
                                            rptService1.DataSource = module.CustomerServices;
                                            rptService1.ItemDataBound += handler.ItemDataBound;
                                            rptService1.DataBind();
                                        }
                                        else
                                        {
                                            rptService1.Visible = false;
                                        }
                                        isSecond = true;
                                    }
                                }
                                else
                                {
                                    CustomerInfoRowInput customer2 = e.Item.FindControl("customer2") as CustomerInfoRowInput;
                                    Repeater rptService2 = e.Item.FindControl("rptServices2") as Repeater;
                                    if (customer2 != null && rptService2 != null)
                                    {
                                        customer2.GetCustomer(customer, module);
                                        if (page.DetailService)
                                        {
                                            CustomerServiceRepeaterHandler handler =
                                                new CustomerServiceRepeaterHandler(customer, module);
                                            rptService2.DataSource = module.CustomerServices;
                                            rptService2.ItemDataBound += handler.ItemDataBound;
                                            rptService2.DataBind();
                                        }
                                        else
                                        {
                                            rptService2.Visible = false;
                                        }
                                    }
                                }
                            }


                            if (customer.Type == CustomerType.Children)
                            {
                                CustomerInfoRowInput customerChild =
                                    e.Item.FindControl("customerChild") as CustomerInfoRowInput;
                                Repeater rptServicesChild = e.Item.FindControl("rptServicesChild") as Repeater;
                                if (customerChild != null && rptServicesChild != null)
                                {
                                    customerChild.GetCustomer(customer, module);
                                    if (page.DetailService)
                                    {
                                        CustomerServiceRepeaterHandler handler = new CustomerServiceRepeaterHandler(
                                            customer, module);
                                        rptServicesChild.DataSource = module.CustomerServices;
                                        rptServicesChild.ItemDataBound += handler.ItemDataBound;
                                        rptServicesChild.DataBind();
                                    }
                                    else
                                    {
                                        rptServicesChild.Visible = false;
                                    }
                                }
                            }

                            if (customer.Type == CustomerType.Baby)
                            {
                                CustomerInfoRowInput customerBaby = e.Item.FindControl("customerBaby") as CustomerInfoRowInput;
                                Repeater rptServicesBaby = e.Item.FindControl("rptServicesBaby") as Repeater;
                                if (customerBaby != null && rptServicesBaby != null)
                                {
                                    customerBaby.GetCustomer(customer, module);
                                    if (page.DetailService)
                                    {
                                        CustomerServiceRepeaterHandler handler = new CustomerServiceRepeaterHandler(
                                            customer, module);
                                        rptServicesBaby.DataSource = module.CustomerServices;
                                        rptServicesBaby.ItemDataBound += handler.ItemDataBound;
                                        rptServicesBaby.DataBind();
                                    }
                                    else
                                    {
                                        rptServicesBaby.Visible = false;
                                    }
                                }
                            }
                        }

                    }

                    #endregion

                    #region -- Check box và visible --

                    checkBoxSingle.Checked = false;
                    checkBoxAddChild.Checked = false;
                    checkBoxAddBaby.Checked = false;
                    checkboxAddExtraBed.Checked = false;
                    checkBoxAddAdult.Checked = false;


                    if (item.Adult == 1)
                    {
                        trCustomer2.Attributes.CssStyle["display"] = "none";
                        checkBoxSingle.Checked = true;
                    }
                    trChild.Attributes.CssStyle["display"] = "none";
                    if (item.HasAddExtraBed)
                    {
                        checkboxAddExtraBed.Checked = true;
                    }
                    if (item.HasAddAdult)
                    {
                        trExtraBed.Attributes.CssStyle["display"] = "";
                        checkBoxAddAdult.Checked = true;
                    }
                    if (item.HasChild)
                    {
                        trChild.Attributes.CssStyle["display"] = "";
                        checkBoxAddChild.Checked = true;
                    }
                    else
                    {
                        Repeater rptServicesChild = e.Item.FindControl("rptServicesChild") as Repeater;
                        if (rptServicesChild != null)
                        {
                            rptServicesChild.DataSource = module.CustomerServices;
                            rptServicesChild.DataBind();
                        }
                    }

                    trBaby.Attributes.CssStyle["display"] = "none";
                    if (item.HasBaby)
                    {
                        trBaby.Attributes.CssStyle["display"] = "";
                        checkBoxAddBaby.Checked = true;
                    }
                    else
                    {
                        Repeater rptServicesBaby = e.Item.FindControl("rptServicesBaby") as Repeater;
                        if (rptServicesBaby != null)
                        {
                            rptServicesBaby.DataSource = module.CustomerServices;
                            rptServicesBaby.DataBind();
                        }
                    }

                    #endregion

                    // Load room available                        

                    #region -- available --

                    //DropDownList ddlRooms = e.Item.FindControl("ddlRooms") as DropDownList;
                    //if (ddlRooms != null)
                    //{
                    //    // Danh sách phòng được chọn bao gồm: toàn bộ các phòng chưa được chọn và phòng trong book hiện tại
                    //    IList datasouce = module.RoomGetAvailable(item.Book.Cruise, item, (item.Book.EndDate - item.Book.StartDate).Days,
                    //                                              item.Book);
                    //    Room room = new Room("", false, null, null);
                    //    datasouce.Insert(0, room);
                    //    ddlRooms.DataSource = datasouce;
                    //    ddlRooms.DataTextField = "Name";
                    //    ddlRooms.DataValueField = "Id";
                    //    ddlRooms.DataBind();

                    //    if (item.Room != null)
                    //    {
                    //        ListItem listItem = ddlRooms.Items.FindByValue(item.Room.Id.ToString());
                    //        if (listItem != null)
                    //        {
                    //            listItem.Selected = true;
                    //        }
                    //    }
                    //}

                    var txtRoomNumber = e.Item.FindControl("txtRoomNumber") as TextBox;
                    txtRoomNumber.Text = item.RoomNumber;

                    #endregion


                    DropDownList ddlRoomTypes = e.Item.FindControl("ddlRoomTypes") as DropDownList;
                    if (ddlRoomTypes != null)
                    {
                        //                        if (item.VirtualAdult == 2)
                        //                        {
                        ddlRoomTypes.DataSource = roomTypes; //module.RoomTypexGetAll();
                        ddlRoomTypes.DataValueField = "Value";
                        ddlRoomTypes.DataTextField = "Text";
                        ddlRoomTypes.DataBind();

                        ListItem listitem =
                            ddlRoomTypes.Items.FindByValue(item.RoomType.Id.ToString());

                        if (listitem != null)
                        {
                            listitem.Selected = true;
                        }
                        else
                        {
                            ddlRoomTypes.Items.Add(new ListItem(item.RoomType.Name, item.RoomType.Id.ToString()));
                            ddlRoomTypes.SelectedValue = item.RoomType.Id.ToString();
                        }
                        if (item.Room != null && item.Room.RoomType.Name != "Twin") ddlRoomTypes.Enabled = false;
                        //                        }
                        //else
                        //{
                        //    ddlRoomTypes.Visible = false;
                        //    e.Item.FindControl("labelRoomTypes").Visible = false;
                        //}
                    }

                    #endregion
                }
            }
        }

        protected void buttonSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                //Biến để đếm tổng số customer

                // Tạo đối tượng booking mới
                Booking booking = new Booking();
                CultureInfo cultureInfo = new CultureInfo("vi-VN");

                #region -- Các thông tin cơ bản --
                if (PageEngine.User.Identity is User)
                {
                    booking.CreatedBy = (User)Page.User.Identity;
                    booking.ModifiedBy = (User)Page.User.Identity;
                    booking.Partner = (User)Page.User.Identity;
                    //booking.Sale = (User)Page.User.Identity;
                }

                booking.CreatedDate = DateTime.Now;
                booking.ModifiedDate = DateTime.Now;
                DateTime startDate = DateTime.ParseExact(Session["StartDate"].ToString(), "dd/MM/yyyy",
                                                        cultureInfo.DateTimeFormat);
                booking.StartDate = startDate;
                booking.Status = StatusType.Cancelled;
                #endregion

                #region - Lấy trip và trip option đã book từ biến session -

                SailsTrip trip = Module.TripGetById(Convert.ToInt32(Session["TripId"]));
                booking.Trip = trip;

                TripOption tripOption = TripOption.Option1;
                if (!string.IsNullOrEmpty(Session["TripOption"].ToString()))
                {
                    tripOption = (TripOption)Convert.ToInt32(Session["TripOption"]);
                }

                booking.TripOption = tripOption;
                #endregion

                // Save booking lại
                if (panelCustomer.Visible)
                {
                    booking.Note = fckCustomers.Value;
                }

                if (liAnonymous.Visible)
                {
                    booking.Name = txtYourName.Text;
                    booking.Email = txtEmail.Text;
                }

                booking.PickupAddress = txtPickupAddress.Text;
                booking.SpecialRequest = txtSpecialRequest.Text;
                Module.Save(booking, null);

                #region -- Lấy thông tin về extra services từ session --
                // Mặc dù extra service đã được chọn từ form trước, tuy nhiên tại form này mới lưu lại thông tin booking
                // Do đó đến form này mới lấy lại thông tin extra service
                if (Session["ExtraService"] != null)
                {
                    string[] services = Session["ExtraService"].ToString().Split(',');
                    foreach (string serviceId in services)
                    {
                        ExtraOption service = Module.ExtraOptionGetById(Convert.ToInt32(serviceId));
                        BookingExtra bookingService = new BookingExtra();
                        bookingService.Booking = booking;
                        bookingService.ExtraOption = service;
                        Module.Save(bookingService);
                    }
                }
                #endregion

                #region -- Lưu thông tin khách hàng --
                foreach (RepeaterItem item in rptRoomList.Items)
                {
                    // Đối với mỗi đối tượng trong room list
                    Label label_RoomId = (Label)item.FindControl("label_RoomId");

                    #region -- Thông tin về phòng --
                    HiddenField hiddenRoomClassId = (HiddenField)item.FindControl("hiddenRoomClassId");
                    HiddenField hiddenRoomTypeId = (HiddenField)item.FindControl("hiddenRoomTypeId");
                    HiddenField hiddenRoomId = (HiddenField)item.FindControl("hiddenRoomId");

                    // Lấy về room id theo label Room id đã bound trước đó
                    int roomId = 0;
                    // Lấy ID của phòng nếu đã có phòng chọn (prefer room)
                    if (!string.IsNullOrEmpty(label_RoomId.Text))
                    {
                        roomId = Convert.ToInt32(label_RoomId.Text);
                    }
                    if (!string.IsNullOrEmpty(hiddenRoomId.Value))
                    {
                        roomId = Convert.ToInt32(hiddenRoomId.Value);
                    }
                    Room room = null;
                    if (roomId > 0)
                    {
                        room = Module.RoomGetById(roomId);
                    }
                    #endregion

                    #region -- Lấy thông tin khách hàng --
                    CheckBox checkboxAddExtraBed = (CheckBox)item.FindControl("checkboxAddExtraBed");
                    CheckBox checkBoxAddChild = (CheckBox)item.FindControl("checkBoxAddChild");
                    CheckBox checkBoxAddBaby = (CheckBox)item.FindControl("checkBoxAddBaby");
                    CheckBox checkBoxSingle = (CheckBox)item.FindControl("checkBoxSingle");

                    //TODO: CHECK THIS
                    //BookingType bookingType = (BookingType) Enum.Parse(typeof(BookingType),ddlRoomType.SelectedValue);
                    const BookingType bookingType = BookingType.Double;
                    CustomerInfoInput customerInfo1 = (CustomerInfoInput)item.FindControl("customer1");
                    CustomerInfoInput customerInfo2 = (CustomerInfoInput)item.FindControl("customer2");

                    BookingRoom bookingRoom;
                    if (room != null)
                    {
                        bookingRoom = new BookingRoom(booking, room, room.RoomClass, room.RoomType);
                    }
                    else
                    {
                        RoomClass roomClass = Module.RoomClassGetById(Convert.ToInt32(hiddenRoomClassId.Value));
                        RoomTypex roomType = Module.RoomTypexGetById(Convert.ToInt32(hiddenRoomTypeId.Value));
                        bookingRoom = new BookingRoom(booking, null, roomClass, roomType);
                    }
                    bookingRoom.BookingType = bookingType;
                    bookingRoom.HasAddExtraBed = checkboxAddExtraBed.Checked;
                    bookingRoom.HasBaby = checkBoxAddBaby.Checked;
                    bookingRoom.HasChild = checkBoxAddChild.Checked;
                    bookingRoom.IsSingle = checkBoxSingle.Checked;
                    Module.Save(bookingRoom);
                    #endregion

                    #region -- Khách hàng --

                    booking.Customers.Clear();
                    Customer customer1;
                    Customer customer2;

                    Control trCustomer2 = item.FindControl("trCustomer2");

                    customer1 = customerInfo1.NewCustomer(Module);
                    customer1.BookingRoom = bookingRoom;
                    customer1.Booking = booking;
                    customer1.Type = CustomerType.Adult;
                    Module.Save(customer1);

                    //if (bookingRoom.RoomType.Id != SailsModule.TWIN || trCustomer2.Visible)
                    if (!checkBoxSingle.Checked)
                    {
                        customer2 = customerInfo2.NewCustomer(Module);
                        customer2.BookingRoom = bookingRoom;
                        customer2.Booking = booking;
                        customer2.Type = CustomerType.Adult;
                        Module.Save(customer2);
                    }

                    if (checkBoxAddChild.Checked)
                    {
                        CustomerInfoInput customerChild = (CustomerInfoInput)item.FindControl("customerChild");
                        Customer child = customerChild.NewCustomer(Module);
                        child.BookingRoom = bookingRoom;
                        child.Booking = booking;
                        child.Type = CustomerType.Children;
                        Module.Save(child);
                    }

                    if (checkBoxAddBaby.Checked)
                    {
                        CustomerInfoInput customerBaby = (CustomerInfoInput)item.FindControl("customerBaby");
                        Customer baby = customerBaby.NewCustomer(Module);
                        baby.BookingRoom = bookingRoom;
                        baby.Booking = booking;
                        baby.Type = CustomerType.Baby;
                        Module.Save(baby);
                    }
                    if (checkboxAddExtraBed.Checked)
                    {
                        CustomerExtraInfoRowInput customerExtraBed = (CustomerExtraInfoRowInput)item.FindControl("customerExtraBed");
                        Customer extra = customerExtraBed.NewCustomer(Module);
                        extra.BookingRoom = bookingRoom;
                        extra.Booking = booking;
                        if (customerExtraBed.IsChild)
                            extra.Type = CustomerType.Children;
                        else if (customerExtraBed.IsBaby)
                            extra.Type = CustomerType.Baby;
                        else extra.Type = CustomerType.Adult;

                        Module.Save(extra);
                    }

                    #endregion
                }
                #endregion

                Session.Add("Finish", booking.Id);

                // Chuyển sang trang kết thúc (trang confirm lại lần cuối booking)
                PageEngine.PageRedirect(string.Format("{0}/{1}{2}", UrlHelper.GetUrlFromSection(Module.Section),
                                                              SailsModule.ACTION_BOOKING_FINISH_PARAM,
                                                              UrlHelper.EXTENSION));
            }
            catch (Exception ex)
            {
                _logger.Error("Error when buttonSubmit_Click inCustomersInfo control", ex);
                throw;
            }
        }
        #endregion
    }
}