using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Aspose.Words.Tables;
using CMS.Web.Util;
using GemBox.Spreadsheet;
using iTextSharp.text.pdf;
using log4net;
using NHibernate.Criterion;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.ReportEngine;
using Portal.Modules.OrientalSails.Web.UI;
using Portal.Modules.OrientalSails.Web.Util;
using TextBox = System.Web.UI.WebControls.TextBox;
using System.Linq;
using Portal.Modules.OrientalSails.BusinessLogic;
using System.Xml;
using OfficeOpenXml;

namespace Portal.Modules.OrientalSails.Web.Admin.Archive
{
    public partial class BookingReport : SailsAdminBase
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(BookingReport));
        private int _adult;
        private int _baby;
        private int _child;
        private Cruise _cruise;
        private IList _cruises;
        private double _customerCost;
        private IList _dailyCost;
        protected DateTime _date;
        private int _doubleCabin;
        private IList _guides;
        private double _runningCost;
        private int _numberOfGroups;
        private int _currentGroup;

        /// <summary>
        /// Các dịch vụ/chi phí theo ngày
        /// </summary>
        private IList _services;

        private IList _suppliers;

        /// <summary>
        /// Tổng số khách
        /// </summary>
        private double _total;

        /// <summary>
        /// Tổng chi phí nhập thủ công
        /// </summary>
        private double _totalCost;

        private int _transferAdult;
        private int _transferChild;
        private int _twin;

        protected IList Suppliers
        {
            get
            {
                if (_suppliers == null)
                {
                    _suppliers = Module.SupplierGetAll();
                }
                return _suppliers;
            }
        }

        protected IList Guides
        {
            get
            {
                if (_guides == null)
                {
                    _guides = Module.GuidesGetAll();
                }
                return _guides;
            }
        }

        protected IList DailyCost
        {
            get
            {
                if (_dailyCost == null)
                {
                    _dailyCost = Module.CostTypeGetDailyInput();
                }
                return _dailyCost;
            }
        }

        protected Cruise ActiveCruise
        {
            get
            {
                if (_cruise == null && Request.QueryString["cruiseid"] != null)
                {
                    _cruise = Module.CruiseGetById(Convert.ToInt32(Request.QueryString["cruiseid"]));
                }
                return _cruise;
            }
        }

        protected IList AllCruises
        {
            get
            {
                if (_cruises == null)
                {
                    _cruises = Module.CruiseGetAll();
                }
                return _cruises;
            }
        }

        private DateTime Date
        {
            get
            {
                var d = new DateTime();
                if (string.IsNullOrEmpty(Request.QueryString["Date"]))
                {
                    return DateTime.Today;
                }
                return DateTime.TryParseExact(Request.QueryString["Date"], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d) ? d : new DateTime();
            }
        }

        private bool _canViewTotal = true;


        private BookingReportBLL bookingReportBLL;
        public BookingReportBLL BookingReportBLL
        {
            get
            {
                if (bookingReportBLL == null)
                    bookingReportBLL = new BookingReportBLL();
                return bookingReportBLL;
            }
        }

        public List<Customer> customerBirthdayList = new List<Customer>();

        public List<Booking> inspectionBookingList = new List<Booking>();

        public DateTime? StartDate
        {
            get
            {
                DateTime? startDate = DateTime.Now;
                try
                {
                    var oaStartDate = Double.Parse(Request.QueryString["Date"]);
                    startDate = DateTime.FromOADate(oaStartDate);
                }
                catch
                {
                }
                return startDate;
            }
        }

        public bool IsLimousineTab
        {
            get
            {
                var isLimousineTab = false;
                try
                {
                    if (Request.QueryString["t"].ToLower() == "lms")
                        isLimousineTab = true;
                }
                catch { }
                return isLimousineTab;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Redirect();
            if (!IsPostBack)
            {
                #region -- Chi phí theo ngày --

                // Lấy danh sách các chi phí nhập thủ công theo ngày
                _services = new ArrayList();
                foreach (CostType type in DailyCost)
                {
                    if (type.IsSupplier)
                    {
                        _services.Add(type);
                    }
                }

                if (Request.QueryString["Date"] != null)
                {
                    txtDate.Text = Request.QueryString["Date"];
                }
                else
                {
                    txtDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                }

                GetDataSource();

                // Lấy danh sách tàu và redirect nếu chỉ có 1 tàu
                IList cruises;
                if (ActiveCruise == null)
                {
                    if (AllCruises.Count == 1)
                    {
                        Cruise cruise = (Cruise)AllCruises[0];
                        string date = string.Empty;
                        if (Request.QueryString["date"] != null)
                        {
                            date = "&date=" + Request.QueryString["date"];
                        }
                        PageRedirect(string.Format("BookingReport.aspx?NodeId={0}&SectionId={1}&cruiseid={2}{3}",
                                                   Node.Id, Section.Id, cruise.Id, date));
                        return;
                    }
                }
                else
                {
                    cruises = new ArrayList();
                    cruises.Add(ActiveCruise);
                    DateTime adate = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var expense = Module.ExpenseGetByDate(ActiveCruise, adate);
                    if (expense.NumberOfGroup > 0)
                    {
                        _numberOfGroups = expense.NumberOfGroup;
                    }
                }

                #endregion

                // Lấy ngày cần hiển thị


                if (ActiveCruise != null)
                {
                    DateTime adate = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    string script =
                        string.Format(
                            "window.location='CustomerComment.aspx?NodeId={0}&SectionId={1}&cruiseid={2}&date={3}'",
                            Node.Id, Section.Id, ActiveCruise.Id, adate.ToOADate());
                    btnComment.Attributes.Add("onclick", script);
                }
                else
                {
                    btnComment.Visible = false;
                }

                #region -- Danh sách các tàu --

                if (AllCruises.Count == 1)
                {
                    rptCruises.Visible = false;
                }
                else
                {
                    rptCruises.DataSource = AllCruises;
                    rptCruises.DataBind();
                }

                #endregion



                _canViewTotal = Module.PermissionCheck(Permission.VIEW_TOTAL_BY_DATE, UserIdentity);

                rptBookingList.DataBind();

                #region -- Lấy về và hiển thị "shadow" của các booking đã được rời đi trước đó --

                // lấy về bóng của toàn bộ booking trong ngày
                DateTime today = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy",
                                                     CultureInfo.InvariantCulture);
                var shadows = Module.GetBookingShadow(today);
                var list = new ArrayList();
                foreach (Booking booking in shadows)
                {
                    var histories = Module.BookingGetHistory(booking);
                    for (int ii = histories.Count - 1; ii >= 0; ii--)
                    {
                        if (((BookingHistory)(histories[ii])).StartDate == today || ((BookingHistory)(histories[ii])).Status == StatusType.Cancelled)
                        {
                            var bh = (BookingHistory)(histories[ii]);
                            // hai trường hợp: số phòng lớn hơn 6 thì lấy 45 ngày, còn lại lấy 7 ngày
                            if (booking.BookingRooms.Count >= 6)
                            {
                                //if (bh.Date > today.AddDays(-45) && bh.Date < today.AddDays(45))
                                //{
                                //    list.Add(booking);
                                //}
                                if (bh.Date > today.AddDays(-45))
                                {
                                    list.Add(booking);
                                }
                            }
                            else
                            {
                                //if (bh.Date > today.AddDays(-7) && bh.Date < today.AddDays(7))
                                //{
                                //    list.Add(booking);
                                //}

                                if (bh.Date > today.AddDays(-7))
                                {
                                    list.Add(booking);
                                }
                            }
                            break;
                        }
                    }
                }

                rptShadows.DataSource = list;
                rptShadows.DataBind();

                #endregion


                if (ActiveCruise != null)
                {
                    BarRevenue bar =
                        Module.BarRevenueGetByDate(ActiveCruise, DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy",
                                                                                     CultureInfo.InvariantCulture));
                }
                else
                {
                }

                if (ActiveCruise != null)
                {
                    //Link xếp phòng
                    DateTime adate = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    string url = string.Format("RoomSelector.aspx?NodeId={0}&SectionId={1}&date={2}&cruiseid={3}", Node.Id,
                                               Section.Id, adate.ToOADate(), ActiveCruise.Id);
                    btnOrganizer.Attributes.Add("onclick",
                                                CMS.ServerControls.Popup.OpenPopupScript(url, "Room", 700, 1000) + "; return false;");
                }
                else
                {
                    btnOrganizer.Visible = false;
                }

                foreach (RepeaterItem item in rptBookingList.Items)
                {
                    var tdBirthday = (HtmlTableCell)item.FindControl("tdBirthday");
                    var tdInspection = (HtmlTableCell)item.FindControl("tdInspection");
                    if (customerBirthdayList.Count == 0)
                        tdBirthday.Attributes.Add("style", "display:none");
                    if (inspectionBookingList.Count == 0)
                        tdInspection.Attributes.Add("style", "display:none");
                }

                foreach (RepeaterItem item in rptShadows.Items)
                {
                    var tdBirthdayRptShadows = (HtmlTableCell)item.FindControl("tdBirthday");
                    var tdInspectionRptShadows = (HtmlTableCell)item.FindControl("tdInspection");
                    if (customerBirthdayList.Count == 0)
                        tdBirthdayRptShadows.Attributes.Add("style", "display:none");
                    if (inspectionBookingList.Count == 0)
                        tdInspectionRptShadows.Attributes.Add("style", "display:none");

                }
            }

            if (ActiveCruise == null)
            {
                btnExport.Visible = false;
            }

            SetLinkLimousineTab();

            var isLimousineTab = IsLimousineTab;
            if (isLimousineTab)
            {
                DoWhenLimousineTab();
            }
        }

        public void Redirect()
        {
            var dateQuery = "";
            if (String.IsNullOrEmpty(Request.QueryString["Date"]))
            {
                dateQuery = "&Date=" + Date.ToString("dd/MM/yyyy");
            }
            if (Date >= new DateTime(2018, 12, 7))
            {
                Response.Redirect("../BookingReport.aspx" + Request.Url.Query + dateQuery);
            }
        }

        public void DoWhenLimousineTab()
        {
            var startDate = StartDate;
            rptBookingList.DataSource = BookingReportBLL.BookingGetAllBy(startDate, (int)StatusType.Approved, true);
            rptBookingList.DataBind();
            rptCruiseExpense.DataSource = null;
            rptCruiseExpense.DataBind();
            btnExportLimousine.Visible = true;
            ColorTab();
        }

        public void ColorTab()
        {
            var hplLimousineTabLi = (HtmlGenericControl)hplLimousineTab.Parent;
            hplLimousineTabLi.Attributes.Add("class", "selected");
        }

        public void SetLinkLimousineTab()
        {
            var startDate = StartDate;
            var startDateQuery = "";
            if (startDate != null)
            {
                startDateQuery += "&Date=" + startDate.Value;
            }
            hplLimousineTab.NavigateUrl = "BookingReport.aspx?NodeId=1&SectionId=15&t=lms" + startDateQuery;
        }
        #region -- Report --

        protected void btnSaveExpenses_Click(object sender, EventArgs e)
        {
            IList list;
            int count = GetData(out list, false);
            if (count == 0)
            {
                ShowErrors(Resources.errorNoBookingSave);
                return;
            }
            DateTime date = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            foreach (RepeaterItem rptItem in rptCruiseExpense.Items)
            {
                var hiddenId = (HiddenField)rptItem.FindControl("hiddenId");
                Cruise cruise = Module.CruiseGetById(Convert.ToInt32(hiddenId.Value));
                Expense expense = Module.ExpenseGetByDate(cruise, date);

                if (expense.Id < 0)
                {
                    Module.SaveOrUpdate(expense);
                }

                var rptServices = (Repeater)rptItem.FindControl("rptServices");
                IList<ExpenseService> expenseList = rptServicesToIList(rptServices);
                foreach (ExpenseService service in expenseList)
                {
                    service.Expense = expense;
                    if (service.IsRemoved)
                    {
                        if (service.Id > 0)
                        {
                            expense.Services.Remove(service);
                        }
                    }
                    else
                    {
                        // Phải có tên hoặc giá thì mới lưu
                        //if (!string.IsNullOrEmpty(service.Name) || service.Cost > 0 ||
                        //    service.Type.Id == SailsModule.GUIDE_COST)
                        //{
                        Module.SaveOrUpdate(service);
                        //}
                    }
                }

                int numberOfGroup = 1;
                foreach (ExpenseService service in expenseList)
                {
                    numberOfGroup = Math.Max(service.Group, numberOfGroup);
                }
                expense.NumberOfGroup = numberOfGroup;

                Module.SaveOrUpdate(expense);
                _numberOfGroups = numberOfGroup;

            }

            if (ActiveCruise != null)
            {
                DayNote note = Module.DayNoteGetByDay(ActiveCruise, date);
                if (!string.IsNullOrEmpty(note.Note) || note.Id > 0)
                {
                    Module.SaveOrUpdate(note);
                }
            }

            foreach (RepeaterItem rptItem in rptBookingList.Items)
            {
                var ddlGroup = (DropDownList)rptItem.FindControl("ddlGroup");
                var hiddenId = (HiddenField)rptItem.FindControl("hiddenId");
                var bk = Module.BookingGetById(Convert.ToInt32(hiddenId.Value));
                int group = Convert.ToInt32(Request.Params[ddlGroup.UniqueID]);
                bk.Group = group;
                Module.SaveOrUpdate(bk);
            }

            rptBookingList.DataSource = list;
            rptBookingList.DataBind();
            LoadService(_date);

            foreach (RepeaterItem item in rptBookingList.Items)
            {
                var tdBirthday = (HtmlTableCell)item.FindControl("tdBirthday");
                var tdInspection = (HtmlTableCell)item.FindControl("tdInspection");
                if (customerBirthdayList.Count == 0)
                    tdBirthday.Attributes.Add("style", "display:none");
                if (inspectionBookingList.Count == 0)
                    tdInspection.Attributes.Add("style", "display:none");
            }

            foreach (RepeaterItem item in rptShadows.Items)
            {
                var tdBirthdayRptShadows = (HtmlTableCell)item.FindControl("tdBirthday");
                var tdInspectionRptShadows = (HtmlTableCell)item.FindControl("tdInspection");
                if (customerBirthdayList.Count == 0)
                    tdBirthdayRptShadows.Attributes.Add("style", "display:none");
                if (inspectionBookingList.Count == 0)
                    tdInspectionRptShadows.Attributes.Add("style", "display:none");

            }

            if (ActiveCruise != null)
            {
                BarRevenue bar = Module.BarRevenueGetByDate(ActiveCruise, date);
                Module.SaveOrUpdate(bar);
            }
        }


        protected void btnExportLimousine_Click(object sender, EventArgs e)
        {
            string tplPath = Server.MapPath("/Modules/Sails/Admin/ExportTemplates/LenhDieuTourLimousine.xls");
            var excelFile = new ExcelFile();
            excelFile.LoadXls(tplPath);
            var sheet = excelFile.Worksheets[0];
            var startDate = StartDate;
            var listLimousineBooking = BookingReportBLL.BookingGetAllBy(startDate, (int)StatusType.Approved, true);

            int current = 14;
            foreach (var limousineBooking in listLimousineBooking)
            {
                string name = limousineBooking.CustomerNameFull.Replace("<br/>", "\n");
                if (name.Length > 0)
                {
                    name = name.Remove(name.Length - 1);
                }
                sheet.Cells[current, 1].Value = name; // Cột name
                sheet.Cells[current, 2].Value = limousineBooking.Adult; // Cột adult
                sheet.Cells[current, 3].Value = limousineBooking.Child; // Cột child
                sheet.Cells[current, 4].Value = limousineBooking.Baby; // Cột baby
                sheet.Cells[current, 5].Value = limousineBooking.Trip.TripCode; // Cột trip code
                sheet.Cells[current, 6].Value = limousineBooking.PickupAddress; // Cột pickup address
                sheet.Cells[current, 7].Value = limousineBooking.SpecialRequest; // Cột special request
                sheet.Cells[current, 8].Value = limousineBooking.Note;
                sheet.Cells[current, 9].Value = "OS" + limousineBooking.Id;
            }

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";

            Response.AppendHeader("content-disposition", "attachment; filename=" + string.Format("Lenh_dieu_tour.xls"));

            MemoryStream m = new MemoryStream();

            excelFile.SaveXls(m);

            Response.OutputStream.Write(m.GetBuffer(), 0, m.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();

            m.Close();
            Response.End();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {

            #region -- Lưu thông tin expense --

            IList list;
            int count = GetData(out list, false);
            if (count == 0)
            {
                ShowErrors(Resources.errorNoBookingSave);
                return;
            }
            DateTime date = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            IList<ExpenseService> expenseList = null;
            foreach (RepeaterItem rptItem in rptCruiseExpense.Items)
            {
                HiddenField hiddenId = (HiddenField)rptItem.FindControl("hiddenId");
                Cruise cruise = Module.CruiseGetById(Convert.ToInt32(hiddenId.Value));
                Expense expense = Module.ExpenseGetByDate(cruise, date);

                if (expense.Id < 0)
                {
                    Module.SaveOrUpdate(expense);
                }

                Repeater rptServices = (Repeater)rptItem.FindControl("rptServices");
                expenseList = rptServicesToIList(rptServices);
                foreach (ExpenseService service in expenseList)
                {
                    service.Expense = expense;
                    if (service.IsRemoved)
                    {
                        if (service.Id > 0)
                        {
                            expense.Services.Remove(service);
                        }
                    }
                    else
                    {
                        // Phải có tên hoặc giá thì mới lưu
                        if (!string.IsNullOrEmpty(service.Name) || service.Cost > 0 ||
                            service.Type.Id == SailsModule.GUIDE_COST)
                        {
                            Module.SaveOrUpdate(service);
                        }
                    }
                }

                int numberOfGroups = 1;
                foreach (ExpenseService service in expenseList)
                {
                    numberOfGroups = Math.Max(service.Group, numberOfGroups);
                }
                expense.NumberOfGroup = numberOfGroups;

                Module.SaveOrUpdate(expense);
            }

            if (ActiveCruise != null)
            {
                DayNote note = Module.DayNoteGetByDay(ActiveCruise, date);

                if (!string.IsNullOrEmpty(note.Note) || note.Id > 0)
                {
                    Module.SaveOrUpdate(note);
                }
            }

            LoadService(date);

            if (ActiveCruise != null)
            {
                BarRevenue bar = Module.BarRevenueGetByDate(ActiveCruise, date);
                Module.SaveOrUpdate(bar);
            }

            #endregion

            string tplPath = Server.MapPath("/Modules/Sails/Admin/ExportTemplates/Lenhdieutour.xls");
            //TourCommand.Export(list, count, expenseList, _date, BookingFormat, Response, tplPath);

            var excelFile = new ExcelFile();
            excelFile.LoadXls(tplPath);

            var startDate = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            if (startDate == null)
                throw new Exception("startDate = null");


            int? cruiseId = null;
            if (!String.IsNullOrEmpty(Request.QueryString["cruiseid"]))
            {
                cruiseId = Convert.ToInt32(Request.QueryString["cruiseid"]);
            }
            Cruise cruiseObject = null;
            if (cruiseId != null)
            {
                cruiseObject = Module.GetObject<Cruise>(cruiseId.Value);
            }


            GemBox.Spreadsheet.ExcelWorksheet sheetAllGroup = excelFile.Worksheets.AddCopy("LenhDieuTourAll", excelFile.Worksheets[0]);
            sheetAllGroup.Cells["D1"].Value = startDate.ToLongDateString();
            sheetAllGroup.Cells["H1"].Value = cruiseObject.GetModifiedCruiseName();
            var sailsexpenseAllGroup = Module.ExpenseGetByDate(cruiseObject, startDate) as Expense;
            for (int i = 0; i < sailsexpenseAllGroup.Services.Count; i++)
            {
                var expenseService = sailsexpenseAllGroup.Services[i] as ExpenseService;
                if (expenseService == null)
                    break;
                switch (expenseService.Type.Id)
                {
                    case SailsModule.TRANSPORT:
                        sheetAllGroup.Cells["C4"].Value = expenseService.Name;
                        sheetAllGroup.Cells["F4"].Value = expenseService.Phone;
                        break;
                    case SailsModule.GUIDE_COST:
                        if (expenseService.Supplier != null)
                        {
                            sheetAllGroup.Cells["C3"].Value = expenseService.Supplier.Name;
                            sheetAllGroup.Cells["F3"].Value = expenseService.Supplier.Phone;
                        }
                        else
                        {
                            sheetAllGroup.Cells["C3"].Value = expenseService.Name;
                            sheetAllGroup.Cells["F3"].Value = expenseService.Phone;
                        }
                        break;
                    case SailsModule.OPERATOR:
                        sheetAllGroup.Cells["C5"].Value = expenseService.Name;
                        sheetAllGroup.Cells["F5"].Value = expenseService.Phone;
                        break;
                }
            }

            int adultAllGroup = 0;
            int childAllGroup = 0;
            int babyAllGroup = 0;

            int pAdultAllGroup = 0;
            int pChildAllGroup = 0;
            int pBabyAllGroup = 0;

            for (int i = 0; i < list.Count; i++)
            {
                var booking = list[i] as Booking;
                if (booking == null)
                    throw new Exception("booking = null");

                if (booking.StartDate == startDate)
                {
                    adultAllGroup += booking.Adult;
                    childAllGroup += booking.Child;
                    babyAllGroup += booking.Baby;
                }
                else
                {
                    pAdultAllGroup += booking.Adult;
                    pChildAllGroup += booking.Child;
                    pBabyAllGroup += booking.Baby;
                }
            }

            int paxAllGroup = adultAllGroup + childAllGroup + pAdultAllGroup + pChildAllGroup;

            sheetAllGroup.Cells["C6"].Value = paxAllGroup;
            sheetAllGroup.Cells["C10"].Value = adultAllGroup;
            sheetAllGroup.Cells["D10"].Value = childAllGroup;
            sheetAllGroup.Cells["E10"].Value = babyAllGroup;

            sheetAllGroup.Cells["C16"].Value = pAdultAllGroup;
            sheetAllGroup.Cells["D16"].Value = pChildAllGroup;
            sheetAllGroup.Cells["E16"].Value = pBabyAllGroup;


            // Sao chép dòng đầu theo số lượng booking
            // Dòng đầu tiên là dòng 9
            const int firstrowAllGroup = 8;

            // Đếm số book trong ngày
            int currAllGroup = 0;
            foreach (Booking booking in list)
            {
                if (booking.StartDate == _date)
                {
                    currAllGroup += 1;
                }
            }
            if (currAllGroup == 0)
            {
                sheetAllGroup.Rows[14].InsertCopy(count - currAllGroup - 1, sheetAllGroup.Rows[firstrowAllGroup]);
            }
            if (currAllGroup > 0)
            {
                sheetAllGroup.Rows[firstrowAllGroup].InsertCopy(currAllGroup - 1, sheetAllGroup.Rows[firstrowAllGroup]);
            }
            int firstProwAllGroup = 14 + currAllGroup;

            // Ghi vào file excel theo từng dòng
            int crowAllGroup = firstrowAllGroup;
            int cProwAllGroup = firstProwAllGroup - 1;
            foreach (Booking booking in list)
            {
                int current;
                int index;
                if (booking.StartDate != _date)
                {
                    // Dòng previous hiện tại
                    current = cProwAllGroup;
                    // Index = cột previous hiện tại - previous đầu tiên
                    index = cProwAllGroup - firstProwAllGroup + 2;
                    cProwAllGroup++;
                }
                else
                {
                    current = crowAllGroup;
                    index = crowAllGroup - firstrowAllGroup + 1;
                    crowAllGroup++;
                }
                sheetAllGroup.Cells[current, 0].Value = index; // Cột index
                string name = booking.CustomerNameFull.Replace("<br/>", "\n");
                if (name.Length > 0)
                {
                    name = name.Remove(name.Length - 1);
                }
                sheetAllGroup.Cells[current, 1].Value = name; // Cột name
                sheetAllGroup.Cells[current, 2].Value = booking.Adult; // Cột adult
                sheetAllGroup.Cells[current, 3].Value = booking.Child; // Cột child
                sheetAllGroup.Cells[current, 4].Value = booking.Baby; // Cột baby
                sheetAllGroup.Cells[current, 5].Value = booking.Trip.TripCode; // Cột trip code
                sheetAllGroup.Cells[current, 6].Value = booking.PickupAddress; // Cột pickup address
                sheetAllGroup.Cells[current, 7].Value = booking.SpecialRequest; // Cột special request
                sheetAllGroup.Cells[current, 8].Value = booking.Note;
                sheetAllGroup.Cells[current, 9].Value = "OS" + booking.Id;
            }


            int numberOfGroup = -1;
            foreach (Booking booking in list)
            {
                if (booking.Group > numberOfGroup)
                {
                    numberOfGroup = booking.Group;
                }
            }

            foreach (ExpenseService service in expenseList)
            {
                if (service.Group > numberOfGroup)
                {
                    numberOfGroup = service.Group;
                }
            }

            if (numberOfGroup <= 0)
            {
                numberOfGroup = 1; // Tối thiểu là 1 group
            }

            for (int ii = 0; ii <= numberOfGroup; ii++)
            {
                // Mở sheet 0
                GemBox.Spreadsheet.ExcelWorksheet originSheet = excelFile.Worksheets[0];

                int numberOfBooking = 0;
                foreach (Booking booking in list)
                {
                    if (booking.Group == ii)
                    {
                        numberOfBooking++;
                    }
                }

                if (numberOfBooking == 0 && ii == 0) continue;

                GemBox.Spreadsheet.ExcelWorksheet sheet = excelFile.Worksheets.AddCopy("LenhDieuTour" + ii, originSheet);
                sheet.Cells["D1"].Value = startDate.ToLongDateString();
                sheet.Cells["H1"].Value = cruiseObject.GetModifiedCruiseName();

                var sailsexpense = Module.ExpenseGetByDate(cruiseObject, startDate) as Expense;
                for (int i = 0; i < sailsexpense.Services.Count; i++)
                {
                    var expenseService = sailsexpense.Services[i] as ExpenseService;
                    if (expenseService.Type.Id == SailsModule.OPERATOR)
                    {
                        sheet.Cells["C5"].Value = expenseService.Name;
                        sheet.Cells["F5"].Value = expenseService.Phone;
                    }
                    if (expenseService.Group != ii) continue;
                    if (expenseService == null)
                        break;
                    switch (expenseService.Type.Id)
                    {
                        case SailsModule.TRANSPORT:
                            sheet.Cells["C4"].Value = expenseService.Name;
                            sheet.Cells["F4"].Value = expenseService.Phone;
                            break;
                        case SailsModule.GUIDE_COST:
                            if (expenseService.Supplier != null)
                            {
                                sheet.Cells["C3"].Value = expenseService.Supplier.Name;
                                sheet.Cells["F3"].Value = expenseService.Supplier.Phone;
                            }
                            else
                            {
                                sheet.Cells["C3"].Value = expenseService.Name;
                                sheet.Cells["F3s"].Value = expenseService.Phone;
                            }
                            break;
                        case SailsModule.OPERATOR:
                            sheet.Cells["C5"].Value = expenseService.Name;
                            sheet.Cells["F5"].Value = expenseService.Phone;
                            break;
                    }
                }

                int adult = 0;
                int child = 0;
                int baby = 0;

                int pAdult = 0;
                int pChild = 0;
                int pBaby = 0;

                for (int i = 0; i < list.Count; i++)
                {
                    var booking = list[i] as Booking;
                    if (booking == null)
                        throw new Exception("booking = null");

                    if (booking.StartDate == startDate)
                    {
                        adult += booking.Adult;
                        child += booking.Child;
                        baby += booking.Baby;
                    }
                    else
                    {
                        pAdult += booking.Adult;
                        pChild += booking.Child;
                        pBaby += booking.Baby;
                    }
                }

                int pax = adult + child + pAdult + pChild;

                sheet.Cells["C6"].Value = pax;
                sheet.Cells["C10"].Value = adult;
                sheet.Cells["D10"].Value = child;
                sheet.Cells["E10"].Value = baby;

                sheet.Cells["C16"].Value = pAdult;
                sheet.Cells["D16"].Value = pChild;
                sheet.Cells["E16"].Value = pBaby;


                // Sao chép dòng đầu theo số lượng booking
                // Dòng đầu tiên là dòng 9
                const int firstrow = 8;

                // Đếm số book trong ngày
                int curr = 0;
                foreach (Booking booking in list)
                {
                    if (booking.StartDate == _date)
                    {
                        curr += 1;
                    }
                }
                if (curr == 0)
                {
                    sheet.Rows[14].InsertCopy(count - curr - 1, sheet.Rows[firstrow]);
                }
                if (curr > 0)
                {
                    sheet.Rows[firstrow].InsertCopy(curr - 1, sheet.Rows[firstrow]);
                }
                int firstProw = 14 + curr;

                #region -- Thông tin từng booking --


                int crow = firstrow;
                int cProw = firstProw - 1;
                foreach (Booking booking in list)
                {
                    if (booking.Group != ii) continue;
                    int current;
                    int index;
                    if (booking.StartDate != _date)
                    {
                        // Dòng previous hiện tại
                        current = cProw;
                        // Index = cột previous hiện tại - previous đầu tiên
                        index = cProw - firstProw + 2;
                        cProw++;
                    }
                    else
                    {
                        current = crow;
                        index = crow - firstrow + 1;
                        crow++;
                    }
                    sheet.Cells[current, 0].Value = index; // Cột index
                    string name = booking.CustomerNameFull.Replace("<br/>", "\n");
                    if (name.Length > 0)
                    {
                        name = name.Remove(name.Length - 1);
                    }
                    sheet.Cells[current, 1].Value = name; // Cột name
                    sheet.Cells[current, 2].Value = booking.Adult; // Cột adult
                    sheet.Cells[current, 3].Value = booking.Child; // Cột child
                    sheet.Cells[current, 4].Value = booking.Baby; // Cột baby
                    sheet.Cells[current, 5].Value = booking.Trip.TripCode; // Cột trip code
                    sheet.Cells[current, 6].Value = booking.PickupAddress; // Cột pickup address
                    sheet.Cells[current, 7].Value = booking.SpecialRequest; // Cột special request
                    sheet.Cells[current, 8].Value = booking.Note;
                    sheet.Cells[current, 9].Value = "OS" + booking.Id;
                }
            }
            excelFile.Worksheets[0].Delete();
            // Ghi vào file excel theo từng dòng
                #endregion

            #region -- Trả dữ liệu về cho người dùng --

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            if (cruiseObject != null)
                Response.AppendHeader("content-disposition", "attachment; filename=" + string.Format("Lenh_dieu_tour_{0}_{1}.xls", startDate.ToString("dd/MM/yyyy"), cruiseObject.GetModifiedCruiseName().Replace(" ", "_")));
            else
                Response.AppendHeader("content-disposition", "attachment; filename=" + string.Format("Lenh_dieu_tour_{0}_{1}.xls", startDate.ToString("dd/MM/yyyy"), "AllCruises"));

            MemoryStream m = new MemoryStream();

            excelFile.SaveXls(m);

            Response.OutputStream.Write(m.GetBuffer(), 0, m.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();

            m.Close();
            Response.End();

            #endregion
        }


        protected void btnExportRoom_Click(object sender, EventArgs e)
        {
            IList list;
            int count = GetData(out list, false);

            int totalRows = 0;
            foreach (Booking booking in list)
            {
                totalRows += booking.BookingRooms.Count;
            }


            // Bắt đầu thao tác export
            ExcelFile excelFile = new ExcelFile();
            excelFile.LoadXls(Server.MapPath("/Modules/Sails/Admin/ExportTemplates/Rooming_list.xls"));
            // Mở sheet 0
            GemBox.Spreadsheet.ExcelWorksheet sheet = excelFile.Worksheets[0];

            #region -- Xuất dữ liệu --

            const int firstrow = 3;
            sheet.Rows[firstrow].InsertCopy(totalRows - 1, sheet.Rows[firstrow]);
            int curr = firstrow;
            foreach (Booking booking in list)
            {
                foreach (BookingRoom room in booking.BookingRooms)
                {
                    sheet.Cells[curr, 0].Value = curr - firstrow + 1;
                    string name = string.Empty;
                    foreach (Customer customer in room.Customers)
                    {
                        if (!string.IsNullOrEmpty(customer.Fullname))
                        {
                            name += customer.Fullname + "\n";
                        }
                    }
                    if (name.Length > 0)
                    {
                        name = name.Remove(name.Length - 1);
                    }
                    sheet.Cells[curr, 1].Value = name;
                    sheet.Cells[curr, 2].Value = room.Adult + room.Child;
                    sheet.Cells[curr, 3].Value = room.RoomType.Name;
                    if (room.Room != null)
                    {
                        sheet.Cells[curr, 4].Value = room.Room.Name;
                    }
                    curr++;
                }
            }

            #endregion

            #region -- Trả dữ liệu về cho người dùng --

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AppendHeader("content-disposition",
                                  "attachment; filename=" + string.Format("Roomlist{0:dd_MMM}", _date));

            MemoryStream m = new MemoryStream();

            excelFile.SaveXls(m);

            Response.OutputStream.Write(m.GetBuffer(), 0, m.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();

            m.Close();
            Response.End();

            #endregion
        }

        protected void btnExportWelcome_Click(object sender, EventArgs e)
        {
            IList bklist;
            int count = GetData(out bklist, false);
            IList list = new ArrayList();
            foreach (Booking booking in bklist)
            {
                foreach (BookingRoom bkroom in booking.BookingRooms)
                {
                    list.Add(bkroom);
                }
            }
            count = list.Count;


            // Bắt đầu thao tác export
            ExcelFile excelFile = new ExcelFile();
            excelFile.LoadXls(Server.MapPath("/Modules/Sails/Admin/ExportTemplates/welcome_board.xls"));
            // Mở sheet 0
            GemBox.Spreadsheet.ExcelWorksheet sheet = excelFile.Worksheets[0];

            Expense expense = Module.ExpenseGetByDate(ActiveCruise, _date);

            CellRange range = sheet.Cells.GetSubrangeRelative(0, 0, 12, 6);

            #region -- Xuất dữ liệu --

            const int firstrow = 0;
            for (int ii = 1; ii < count; ii++)
            {
                range.CopyTo(ii * 6, 0);
                for (int jj = 0; jj < 6; jj++)
                {
                    sheet.Rows[ii * 6 + jj].Height = sheet.Rows[jj].Height;
                }
            }
            int curr = firstrow;
            foreach (BookingRoom bkroom in list)
            {
                string name = bkroom.CustomerName;
                name = name.Replace("<br/>", "\n");
                if (name.Length > 0)
                {
                    name = name.Remove(name.Length - 1);
                }
                sheet.Cells[curr + 2, 0].Value = name;
                foreach (ExpenseService service in expense.Services)
                {
                    switch (service.Type.Id)
                    {
                        case SailsModule.TRANSPORT:
                            sheet.Cells[curr + 4, 1].Value = service.Name + " - " + service.Phone;
                            break;
                        case SailsModule.GUIDE_COST:
                            if (service.Supplier != null)
                            {
                                sheet.Cells[curr + 5, 1].Value = service.Supplier.Name + " - " + service.Supplier.Phone;
                            }
                            else
                            {
                                sheet.Cells[curr + 5, 1].Value = service.Name + " - " + service.Phone;
                            }
                            break;
                    }
                }
                sheet.Cells[curr + 4, 4].Value = bkroom.Book.PickupAddress;
                sheet.Cells[curr + 4, 9].Value = UserIdentity.FullName;
                curr += 6;
            }

            #endregion

            #region -- Trả dữ liệu về cho người dùng --

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AppendHeader("content-disposition",
                                  "attachment; filename=" + string.Format("WelcomeBoard{0:dd_MMM}", _date));

            MemoryStream m = new MemoryStream();

            excelFile.SaveXls(m);

            Response.OutputStream.Write(m.GetBuffer(), 0, m.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();

            m.Close();
            Response.End();

            #endregion
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {

            IList dataSource;
            int count = GetData(out dataSource, false);

            // Lấy danh sách khách hàng
            IList customers = new ArrayList();
            DateTime startDate = DateTime.MinValue;


            foreach (Booking book in dataSource)
            {
                if (startDate < book.StartDate)
                {
                    startDate = book.StartDate;
                }
                foreach (BookingRoom room in book.BookingRooms)
                {
                    foreach (Customer customer in room.RealCustomers)
                    {
                        customers.Add(customer);
                    }
                }
            }

            if (startDate == DateTime.MinValue)
            {
                ShowErrors("Không có thông tin");
                return;
            }
            MemoryStream mem = new MemoryStream();
            using (var excelPackage = new ExcelPackage(new FileInfo(Server.MapPath("/Modules/Sails/Admin/ExportTemplates/ClientDetails.xlsx"))))
            {
                var sheet = excelPackage.Workbook.Worksheets["Sheet1"];
                sheet.Cells["G2"].Value = "Start date:" + " " + startDate.ToString("dd-MMM");
                sheet.Cells["H2"].Value = ActiveCruise.Name;
                int currentRow = 6;
                int index = 1;
                sheet.InsertRow(currentRow, customers.Count, currentRow - 1);
                currentRow--;
                for (int i = 0; i < customers.Count; i++)
                {
                    var customer = customers[i] as Customer;
                    if (customer != null)
                    {
                        sheet.Cells[currentRow, 1].Value = index;
                        if (!String.IsNullOrEmpty(customer.Fullname))
                            sheet.Cells[currentRow, 2].Value = customer.Fullname.ToUpper();
                        if (customer.Birthday.HasValue)
                            sheet.Cells[currentRow, 4].Value = customer.Birthday.Value.ToString("dd/MM/yyyy");
                        else
                            sheet.Cells[currentRow, 4].Value = "";
                        if (customer.IsMale.HasValue)
                        {
                            if (customer.IsMale.Value == true)
                                sheet.Cells[currentRow, 3].Value = "M | Male";
                            else
                                sheet.Cells[currentRow, 3].Value = "F | Female";
                        }
                        if (customer.Nationality != null)
                            if (customer.Nationality.Name.ToLower() != "khong ro")
                                sheet.Cells[currentRow, 6].Value = customer.Nationality.Name;
                            else
                                sheet.Cells[currentRow, 6].Value = customer.Passport;
                        sheet.Cells[currentRow, 5].Value = customer.Passport;
                        sheet.Cells[currentRow, 7].Value = customer.Booking.Cruise.GetModifiedCruiseName() + " " + customer.Booking.Trip.NumberOfDay + "D";
                        currentRow++;
                        index++;
                    }
                    else
                    {
                        throw new Exception("customer = null");
                    }
                }
                sheet.DeleteRow(currentRow);
                excelPackage.SaveAs(mem);
            }
            int? cruiseId = null;
            if (!String.IsNullOrEmpty(Request.QueryString["cruiseid"]))
            {
                cruiseId = Convert.ToInt32(Request.QueryString["cruiseid"]);
            }
            Cruise cruise = null;
            if (cruiseId != null)
            {
                cruise = Module.GetObject<Cruise>(cruiseId.Value);
            }
            Response.Clear();
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            if (cruise != null)
                Response.AppendHeader("content-disposition", "attachment; filename=" + string.Format("Client_Details_{0}_{1}.xlsx", startDate.ToString("dd/MM/yyyy"), cruise.GetModifiedCruiseName().Replace(" ", "_")));
            else
                Response.AppendHeader("content-disposition", "attachment; filename=" + string.Format("Client_Details_{0}_{1}.xlsx", startDate.ToString("dd/MM/yyyy"), "AllCruises"));
            mem.Position = 0;
            byte[] buffer = mem.ToArray();
            Response.BinaryWrite(buffer);
            Response.End();
        }

        protected void btnProvisional_Click(object sender, EventArgs e)
        {
            DateTime startDate = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            int cruiseId = -1;
            try
            {
                cruiseId = Convert.ToInt32(Request.QueryString["cruiseid"]);
            }
            catch (Exception) { }
            var bookingStatus = (int)StatusType.Approved;
            var bookings = BookingReportBLL.BookingReportBLL_BookingSearchBy(startDate, cruiseId, bookingStatus);
            var bookings2Days = new List<Booking>();
            var bookings3Days = new List<Booking>();
            foreach (var booking in bookings)
            {
                if (booking.Trip.NumberOfDay == 2)
                    bookings2Days.Add(booking);

                if (booking.Trip.NumberOfDay == 3)
                    bookings3Days.Add(booking);
            }

            var VietNamCustomerOfBookings2Days = new List<Customer>();
            var ForeignCustomerOfBookings2Days = new List<Customer>();
            var VietNamCustomerOfBookings3Days = new List<Customer>();
            var ForeignCustomerOfBookings3Days = new List<Customer>();

            ProvisalRegisterSortCustomer(bookings2Days, ref VietNamCustomerOfBookings2Days, ref ForeignCustomerOfBookings2Days);
            ProvisalRegisterSortCustomer(bookings3Days, ref VietNamCustomerOfBookings3Days, ref ForeignCustomerOfBookings3Days);

            var excelFile = new ExcelFile();
            excelFile.LoadXls(Server.MapPath("/Modules/Sails/Admin/ExportTemplates/DangKyTamTruTemplate.xls"));
            var sheetVietNam2Days = excelFile.Worksheets[0];
            var sheetVietNam3Days = excelFile.Worksheets[1];
            var sheetNuocNgoai2Days = excelFile.Worksheets[2];
            var sheetNuocNgoai3Days = excelFile.Worksheets[3];

            var stt = 1;
            ExportFillProvisalRegister(VietNamCustomerOfBookings2Days, sheetVietNam2Days, ref stt);
            ExportFillProvisalRegister(VietNamCustomerOfBookings3Days, sheetVietNam3Days, ref stt);
            ExportFillProvisalRegister(ForeignCustomerOfBookings2Days, sheetNuocNgoai2Days, ref stt);
            ExportFillProvisalRegister(ForeignCustomerOfBookings3Days, sheetNuocNgoai3Days, ref stt);

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            var cruise = bookingReportBLL.CruiseGetById(cruiseId);
            if (cruise != null)
                Response.AppendHeader("content-disposition", "attachment; filename=" + string.Format("Form_tam_tru_{0}_{1}.xls", startDate.ToString("dd/MM/yyyy"), cruise.GetModifiedCruiseName().Replace(" ", "_")));
            if (cruise == null)
            {
                return;
            }
            MemoryStream m = new MemoryStream();
            excelFile.SaveXls(m);

            Response.OutputStream.Write(m.GetBuffer(), 0, m.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();

            m.Close();
            Response.End();
        }

        public void ProvisalRegisterSortCustomer(IList<Booking> bookings, ref List<Customer> vietNamCustomers, ref List<Customer> foreignCustomer)
        {
            foreach (var booking in bookings)
            {
                foreach (var bookingRoom in booking.BookingRooms)
                {
                    foreach (var customer in bookingRoom.Customers)
                    {
                        if (customer.Nationality == null)
                        {
                            foreignCustomer.Add(customer);
                            continue;
                        }

                        if (customer.Nationality.Name == "VIET NAM")
                            vietNamCustomers.Add(customer);
                        else
                            foreignCustomer.Add(customer);
                    }
                }
            }
        }

        public void ExportFillProvisalRegister(IList<Customer> customers, GemBox.Spreadsheet.ExcelWorksheet sheet, ref int stt)
        {
            var activeRow = 1;
            foreach (var customer in customers)
            {
                sheet.Cells[activeRow, 0].Value = stt.ToString();
                stt++;

                sheet.Cells[activeRow, 1].Value = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(customer.Fullname.ToLower());

                var birthday = "";
                try
                {
                    if(customer.Nationality.Name == "VIET NAM")
                    {
                        birthday = customer.Birthday.Value.ToString("MM/dd/yyyy");
                    }
                    else
                    {
                        birthday = customer.Birthday.Value.ToString("dd/MM/yyyy");
                    }  
                }
                catch (Exception) { }
                sheet.Cells[activeRow, 2].Value = birthday;

                sheet.Cells[activeRow, 3].Value = "D";

                var isMale = false;
                try
                {
                    isMale = customer.IsMale.Value;
                }
                catch (Exception) { }

                if (isMale)
                    sheet.Cells[activeRow, 4].Value = "M";
                else
                    sheet.Cells[activeRow, 4].Value = "F";

                var maquoctich = "";
                try
                {
                    maquoctich = customer.Nationality.AbbreviationCode;
                }
                catch (Exception) { }
                sheet.Cells[activeRow, 5].Value = maquoctich;
                sheet.Cells[activeRow, 6].Value = customer.Passport;
                sheet.Cells[activeRow, 7].Value = ((BookingRoom)customer.BookingRooms[0]).Room != null ? ((BookingRoom)customer.BookingRooms[0]).Room.Name : "";
                sheet.Cells[activeRow, 8].Value = ((BookingRoom)customer.BookingRooms[0]).Book.StartDate.ToString("dd/MM/yyyy");
                sheet.Cells[activeRow, 9].Value = ((BookingRoom)customer.BookingRooms[0]).Book.EndDate.ToString("dd/MM/yyyy");
                sheet.Cells[activeRow, 10].Value = ((BookingRoom)customer.BookingRooms[0]).Book.EndDate.ToString("dd/MM/yyyy");
                sheet.Cells[activeRow, 10].Value = customer.NguyenQuan;
                activeRow++;
            }
        }
        protected void rptBookingList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!_canViewTotal)
            {
                ValueBinder.HideControl(e.Item, "plhTotal");
            }

            if (e.Item.DataItem is Booking)
            {
                var booking = e.Item.DataItem as Booking;
                {
                    if (booking.StartDate < _date)
                    {
                        var trItem = (HtmlTableRow)e.Item.FindControl("trItem");
                        trItem.Attributes.Add("style", "background-color :" + SailsModule.WARNING);
                    }

                    if (booking.Status == StatusType.Pending)
                    {
                        var trItem = (HtmlTableRow)e.Item.FindControl("trItem");
                        trItem.Attributes.Add("style", "background-color : #6699FF");
                        ValueBinder.ShowControl(e.Item, "imgPending");
                        if (booking.Booker != null)
                            ValueBinder.BindLiteral(e.Item, "litBooker", booking.Booker);
                        if (booking.Agency.Sale != null)
                        {
                            ValueBinder.BindLiteral(e.Item, "litSaleInCharge", booking.Agency.Sale.FullName);
                            ValueBinder.BindLiteral(e.Item, "litSalePhone", booking.Agency.Sale.Website);
                        }
                        ValueBinder.BindLiteral(e.Item, "litCreatedBy", booking.CreatedBy.FullName);
                        ValueBinder.BindLiteral(e.Item, "litCreatorPhone", booking.CreatedBy.Website);
                        ValueBinder.BindLiteral(e.Item, "litCreatorEmail", booking.CreatedBy.Email);
                        if (booking.Deadline.HasValue)
                            ValueBinder.BindLiteral(e.Item, "litPendingUntil", booking.Deadline.Value.ToString("HH:mm dd/MM/yyyy"));
                    }

                    var label_NameOfPax = (Label)e.Item.FindControl("label_NameOfPax");
                    var label_NoOfAdult = (Label)e.Item.FindControl("label_NoOfAdult");
                    var label_NoOfChild = (Label)e.Item.FindControl("label_NoOfChild");
                    var label_NoOfBaby = (Label)e.Item.FindControl("label_NoOfBaby");
                    var label_NoOfDoubleCabin = (Label)e.Item.FindControl("label_NoOfDoubleCabin");
                    var label_NoOfTwinCabin = (Label)e.Item.FindControl("label_NoOfTwinCabin");
                    var label_NoOfTransferAdult = (Label)e.Item.FindControl("label_NoOfTransferAdult");
                    var label_NoOfTransferChild = (Label)e.Item.FindControl("label_NoOfTransferChild");
                    var label_TotalPrice = (Label)e.Item.FindControl("label_TotalPrice");
                    var hyperLink_Partner = (HyperLink)e.Item.FindControl("hyperLink_Partner");

                    var litIndex = (Literal)e.Item.FindControl("litIndex");
                    litIndex.Text =
                        (e.Item.ItemIndex + 1).ToString();

                    var litBirthday = (Literal)e.Item.FindControl("litBirthday");
                    for (var i = 0; i < booking.BookingRooms.Count; i++)
                    {
                        var bookingRoom = booking.BookingRooms[i] as BookingRoom;
                        if (bookingRoom == null) continue;
                        var realCustomerList = bookingRoom.RealCustomers;
                        for (var j = 0; j < realCustomerList.Count; j++)
                        {
                            var customer = realCustomerList[j] as Customer;
                            if (customer == null) continue;
                            var customerBirthday = customer.Birthday;
                            if (!customerBirthday.HasValue) continue;
                            if (customerBirthday.Value.Day != booking.StartDate.Day) continue;
                            if (customerBirthday.Value.Month != booking.StartDate.Month) continue;
                            customerBirthdayList.Add(customer);
                            litBirthday.Text = @"<img width='20px' src='https://cdn0.iconfinder.com/data/icons/daily-boxes/150/gift-box-128.png' title='Birthday'/>";
                        }
                    }

                    var litInspection = (Literal)e.Item.FindControl("litInspection");
                    if (booking.Inspection == true)
                    {
                        inspectionBookingList.Add(booking);
                        litInspection.Text = @"<img width='20px' src='http://oplii.com/wp-content/uploads/2014/06/module_inspection_icon_255x2551.png' title='Inspection'/>";
                    }

                    #region -- NAME --

                    label_NameOfPax.Text = "<span id = 'customername' href='#' bookingid = '" + booking.Id + "'>" + booking.CustomerName + "</span>";

                    #endregion

                    #region -- Partner --

                    if (booking.Agency != null)
                    {
                        hyperLink_Partner.Text = booking.Agency.Name;
                        hyperLink_Partner.NavigateUrl =
                            string.Format("../AgencyView.aspx?NodeId={0}&SectionId={1}&agencyid={2}", Node.Id, Section.Id,
                                          booking.Agency.Id);
                    }
                    else
                    {
                        hyperLink_Partner.Text = SailsModule.NOAGENCY;
                    }

                    #endregion

                    #region -- Number of pax --

                    label_NoOfAdult.Text = booking.Adult.ToString();
                    _adult += booking.Adult;

                    label_NoOfChild.Text = booking.Child.ToString();
                    label_NoOfBaby.Text = booking.Baby.ToString();
                    _child += booking.Child;
                    _baby += booking.Baby;

                    #endregion

                    #region - No of cabins -

                    label_NoOfDoubleCabin.Text = booking.DoubleCabin.ToString();
                    label_NoOfTwinCabin.Text = string.Format("{1}({0} adults)", booking.TwinCabin,
                                                             booking.TwinCabin / 2 + booking.TwinCabin % 2);

                    _doubleCabin += booking.DoubleCabin;
                    _twin += booking.TwinCabin;

                    #endregion

                    #region -- Transfer --

                    bool transfer = false;
                    foreach (ExtraOption service in booking.ExtraServices)
                    {
                        if (service.Id == SailsModule.TRANSFER)
                        {
                            transfer = true;
                            break;
                        }
                    }
                    if (transfer)
                    {
                        label_NoOfTransferAdult.Text = label_NoOfAdult.Text;
                        label_NoOfTransferChild.Text = label_NoOfChild.Text;
                        _transferChild += booking.Child;
                        _transferAdult += booking.Adult;
                    }
                    else
                    {
                        label_NoOfTransferAdult.Text = "0";
                        label_NoOfTransferChild.Text = "0";
                    }

                    #endregion

                    #region -- Total --

                    label_TotalPrice.Text = booking.Total.ToString();
                    _total += booking.Total;

                    #endregion

                    #region -- Itinerary --

                    Label labelItinerary = e.Item.FindControl("labelItinerary") as Label;
                    if (labelItinerary != null)
                    {
                        labelItinerary.Text = booking.Trip.TripCode;
                    }

                    Label labelPuAddress = e.Item.FindControl("labelPuAddress") as Label;
                    if (labelPuAddress != null)
                    {
                        labelPuAddress.Text = booking.PickupAddress;
                    }

                    Label labelSpecialRequest = e.Item.FindControl("labelSpecialRequest") as Label;
                    if (labelSpecialRequest != null)
                    {
                        labelSpecialRequest.Text = booking.SpecialRequest;
                    }

                    HyperLink hplCode = e.Item.FindControl("hplCode") as HyperLink;
                    if (hplCode != null)
                    {
                        if (true)
                        //if (string.IsNullOrEmpty(booking.AgencyCode))
                        {
                            if (booking.Id > 0)
                            {
                                hplCode.Text = string.Format(BookingFormat, booking.Id);
                            }
                            else
                            {
                                hplCode.Text = string.Format(BookingFormat, booking.Id);
                            }
                        }
                        else
                        {
                            hplCode.Text = booking.AgencyCode;
                        }
                        hplCode.NavigateUrl = string.Format("../BookingView.aspx?NodeId={0}&SectionId={1}&bi={2}",
                                                            Node.Id, Section.Id, booking.Id);
                    }

                    #endregion

                    var anchorFeedback = e.Item.FindControl("anchorFeedback") as HtmlAnchor;
                    if (anchorFeedback != null)
                    {
                        string url = string.Format("../SurveyInput.aspx?NodeId={0}&SectionId={1}&bi={2}", Node.Id,
                                                   Section.Id, booking.Id);
                        anchorFeedback.Attributes.Add("onclick",
                                                      CMS.ServerControls.Popup.OpenPopupScript(url, "Survey input", 600, 800));
                    }

                    using (var hplStartDate = e.Item.FindControl("hplStartDate") as HyperLink)
                    {
                        if (hplStartDate != null)
                        {
                            hplStartDate.Text = booking.StartDate.ToString("dd/MM/yyyy");
                            hplStartDate.NavigateUrl = string.Format("BookingReport.aspx?NodeId={0}&SectionId={1}&date={2}",
                                                                     Node.Id, Section.Id, booking.StartDate);
                        }
                    }

                    DropDownList ddlGroup = (DropDownList)e.Item.FindControl("ddlGroup");
                    ddlGroup.Items.Add(0.ToString());
                    if (_numberOfGroups < int.MaxValue)
                    {

                        for (int ii = 1; ii <= _numberOfGroups; ii++)
                        {
                            ddlGroup.Items.Add(ii.ToString());
                        }
                        ddlGroup.SelectedValue = booking.Group.ToString();
                    }
                    else if (Request.QueryString["cruiseid"] == null)
                    {
                        ddlGroup.Visible = false;
                        Literal litGroup = (Literal)e.Item.FindControl("litGroup");
                        litGroup.Text = booking.Group.ToString();
                    }
                }
            }

            if (e.Item.ItemType == ListItemType.Footer)
            {
                Label label_NoOfAdult = (Label)e.Item.FindControl("label_NoOfAdult");
                label_NoOfAdult.Text = _adult.ToString();
                Label label_NoOfChild = (Label)e.Item.FindControl("label_NoOfChild");
                label_NoOfChild.Text = _child.ToString();
                Label label_NoOfBaby = (Label)e.Item.FindControl("label_NoOfBaby");
                label_NoOfBaby.Text = _baby.ToString();
                Label label_NoOfDoubleCabin = (Label)e.Item.FindControl("label_NoOfDoubleCabin");
                label_NoOfDoubleCabin.Text = _doubleCabin.ToString();
                Label label_NoOfTwinCabin = (Label)e.Item.FindControl("label_NoOfTwinCabin");
                label_NoOfTwinCabin.Text = string.Format("{1}({0} adults)", _twin, _twin / 2 + _twin % 2);
                Label label_NoOfTransferAdult = (Label)e.Item.FindControl("label_NoOfTransferAdult");
                label_NoOfTransferAdult.Text = _transferAdult.ToString();
                Label label_NoOfTransferChild = (Label)e.Item.FindControl("label_NoOfTransferChild");
                label_NoOfTransferChild.Text = _transferChild.ToString();
                Label label_TotalPrice = (Label)e.Item.FindControl("label_TotalPrice");
                label_TotalPrice.Text = _total.ToString("#,####");
            }
        }

        protected void btnDisplay_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBookingCode.Text))
            {
                string url = string.Format("BookingReport.aspx?NodeId={0}&SectionId={1}&code={2}", Node.Id, Section.Id,
                                           txtBookingCode.Text);
                PageRedirect(url);
            }
            else
            {
                DateTime date = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string url = string.Format("BookingReport.aspx?NodeId={0}&SectionId={1}&Date={2}", Node.Id, Section.Id,
                                           date.ToString("dd/MM/yyyy"));
                PageRedirect(url);
            }
            //GetDataSource();
            //rptBookingList.DataBind();
        }

        #region -- Services --
        protected void rptServices_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem is ExpenseService)
            {
                ExpenseService service = (ExpenseService)e.Item.DataItem;

                if (service.Group != _currentGroup)
                {
                    _currentGroup = service.Group;
                    HtmlTableRow seperator = (HtmlTableRow)e.Item.FindControl("seperator");
                    seperator.Visible = true;
                }

                HiddenField hiddenId = (HiddenField)e.Item.FindControl("hiddenId");
                HiddenField hiddenType = (HiddenField)e.Item.FindControl("hiddenType");
                TextBox txtName = (TextBox)e.Item.FindControl("txtName");
                TextBox txtPhone = (TextBox)e.Item.FindControl("txtPhone");
                DropDownList ddlSuppliers = (DropDownList)e.Item.FindControl("ddlSuppliers");
                DropDownList ddlGuides = (DropDownList)e.Item.FindControl("ddlGuides");
                var ddlGroups = (DropDownList)e.Item.FindControl("ddlGroups");
                TextBox txtCost = (TextBox)e.Item.FindControl("txtCost");
                Literal litType = (Literal)e.Item.FindControl("litType");

                if (_numberOfGroups < int.MaxValue)
                {
                    for (int ii = 1; ii <= _numberOfGroups; ii++)
                    {
                        ddlGroups.Items.Add(ii.ToString());
                    }
                }

                hiddenId.Value = service.Id.ToString();
                hiddenType.Value = (service.Type.Id).ToString();

                if (service.Group >= 0)
                {
                    ddlGroups.SelectedValue = service.Group.ToString();
                }
                txtName.Text = service.Name;
                txtPhone.Text = service.Phone;
                txtCost.Text = service.Cost.ToString("#,0.#");
                txtCost.Attributes.Add("onchange", "this.value = addCommas(this.value);");
                ddlGuides.Visible = false;

                if (service.Type.Id == SailsModule.GUIDE_COST)
                {
                    litType.Text = "Guide";
                    ddlGuides.DataSource = Guides;
                    ddlGuides.DataTextField = "Name";
                    ddlGuides.DataValueField = "Id";
                    ddlGuides.DataBind();
                    txtName.Visible = false;
                    ddlGuides.Visible = true;
                    ddlSuppliers.Visible = false;
                    if (string.IsNullOrEmpty(txtPhone.Text))
                    {
                        txtPhone.Text = "AUTO FROM DATABASE";
                    }
                    txtPhone.Enabled = false;
                }
                else
                {
                    litType.Text = service.Type.Name;
                    if (service.Type.IsSupplier)
                    {
                        ddlSuppliers.DataSource = Suppliers;
                    }
                    else
                    {
                        ddlSuppliers.Visible = false;
                        e.Item.FindControl("btnRemove").Visible = false;
                        e.Item.FindControl("txtCost").Visible = false;
                    }
                }

                if (ddlSuppliers.Visible)
                {
                    ddlSuppliers.DataTextField = "Name";
                    ddlSuppliers.DataValueField = "Id";
                    ddlSuppliers.DataBind();
                }

                #region -- Xử lý đặc biệt --

                if (service.Type.Id == SailsModule.GUIDE_COST)
                {
                    if (service.Supplier != null)
                    {
                        ddlGuides.SelectedValue = service.Supplier.Id.ToString();
                        txtPhone.Text = service.Supplier.Phone;
                    }
                }
                else
                {
                    if (service.Type.IsSupplier)
                    {
                        if (service.Supplier != null)
                        {
                            ddlSuppliers.SelectedValue = service.Supplier.Id.ToString();
                        }
                    }
                }

                #endregion

                if (service.IsRemoved)
                {
                    e.Item.Visible = false;
                }
                else if (service.Type.IsSupplier)
                {
                    _totalCost += service.Cost;
                }
            }
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            Control control = ((Control)sender).Parent;
            control.Visible = false;
        }

        protected void btnAddService_Click(object sender, EventArgs e)
        {
            Button btnAddService = (Button)sender;
            Repeater rptServices = btnAddService.Parent.Parent.Parent.FindControl("rptServices") as Repeater;
            IList<ExpenseService> list = rptServicesToIList(rptServices);
            ExpenseService service = new ExpenseService();
            CostType costType = Module.CostTypeGetById(Convert.ToInt32(btnAddService.CommandArgument));
            service.Type = costType;
            service.IsRemoved = !costType.IsSupplier;
            var singleServiceTypeGreatestGroup = list.Where(x => x.Type == service.Type).Max(x => x.Group);
            service.Group = singleServiceTypeGreatestGroup + 1;
            list.Add(service);

            var allServicesTypeGreatestGroup = list.GroupBy(x => x.Type)
             .Select(x => new
             {
                 Type = x.Key,
                 Count = x.Count()
             }).Max(x => x.Count);
            _numberOfGroups = allServicesTypeGreatestGroup;
            rptServices.DataSource = list;
            rptServices.DataBind();
        }

        protected void rtpAddServices_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem is CostType)
            {
                Button btnAddService = (Button)e.Item.FindControl("btnAddService");
                btnAddService.Text = "Add " + ((CostType)e.Item.DataItem).Name;
            }
        }
        #endregion

        #endregion

        #region -- PRIVATE METHODS --

        protected void GetDataSource()
        {
            IList list;
            int count = GetData(out list, true);
            rptBookingList.DataSource = list;

            Locked locked = null;
            if (ActiveCruise != null)
            {
                locked = Module.LockedCheckByDate(ActiveCruise, Date);
            }
            else
            {
                btnLock.Visible = false;
                return;
            }

            if (locked != null)
            {
                if (locked.Id > 0)
                {
                    btnLock.Text = @"Unlock";
                    txtLockDescription.Text = locked.Description;
                    if (Module.LockedCheckCharter(locked))
                    {
                        btnLock.Visible = false;
                    }
                }
                else
                {
                    btnLock.Text = @"Lock";
                }
            }
            else
            {
                btnLock.Visible = false;
            }
        }

        protected int GetData(out IList list, bool loadService)
        {
            // Lấy về điều kiện cruise và date (bắt buộc date)
            Cruise cruise = null;

            // Check xem có code không
            if (Request.QueryString["code"] != null)
            {
                ICriterion crit = Expression.Eq("Deleted", false);
                crit = SailsModule.AddBookingCodeCriterion(crit, Request.QueryString["code"]);

                var temp = Module.GetObject<Booking>(crit, 2, 0);
                if (temp.Count > 1)
                {
                    ShowErrors("Please input booking code correctly");
                    list = new ArrayList();
                    return 0;
                }
                else if (temp.Count == 0)
                {
                    ShowErrors("No booking with the code you provided");
                    list = new ArrayList();
                    return 0;
                }
                else
                {
                    cruise = temp[0].Cruise;
                    _date = temp[0].StartDate;
                }
            }

            if (cruise == null) // nếu chưa nhận được tham số từ code
            {
                if (Request.QueryString["cruiseid"] != null)
                {
                    cruise = Module.CruiseGetById(Convert.ToInt32(Request.QueryString["cruiseid"]));
                }

                if (string.IsNullOrEmpty(txtDate.Text))
                {
                    _date = DateTime.Today;
                }
                else
                {
                    _date = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
            }

            // Điều kiện bắt buộc: chưa xóa và có status là Approved, hoặc chưa hết hạn holding
            ICriterion criterion = Module.LockCrit();

            // Không bao gồm booking đã transfer
            criterion = Expression.And(criterion, Expression.Not(Expression.Eq("IsTransferred", true)));

            // Điều kiện về tàu
            if (cruise != null)
            {
                //Cruise cruise = Module.CruiseGetById(Convert.ToInt32(Request.QueryString["cruiseid"]));
                criterion = Expression.And(criterion, Expression.Eq("Cruise", cruise));
            }

            criterion = Module.AddDateExpression(criterion, _date);

            int count;
            list = Module.BookingGetByCriterion(criterion, null, out count, 0, 0);
            List<Booking> bookings = new List<Booking>();

            foreach (Booking booking in list)
            {
                bookings.Add(booking);
            }

            list = bookings.OrderBy(o => o.Trip.Id).ToList<Booking>();

            // Nếu cần load service và cruise hiện tại khác null
            if (loadService)
            {
                LoadService(_date);
            }
            return count;
        }

        private void LoadService(DateTime date)
        {
            _numberOfGroups = 0;
            IList cruises;
            if (ActiveCruise == null)
            {
                cruises = AllCruises;
                _numberOfGroups = int.MaxValue;
            }
            else
            {
                cruises = new ArrayList();
                cruises.Add(ActiveCruise);
            }

            _date = date;
            // Nếu chỉ có một tàu hiển thị chi phí
            if (ActiveCruise != null)
            {
                Expense expense = Module.ExpenseGetByDate(ActiveCruise, date);
                if (expense.Id < 0)
                {
                    Module.SaveOrUpdate(expense);
                }

                if (expense.LockIncome && expense.LockBy != null)
                {
                    litLockIncome.Text = string.Format("Locked by {0} at {1} ", expense.LockBy.FullName, expense.LockDate);
                    btnLockIncome.Visible = false;
                    btnUnlockIncome.Visible = true;
                }

                ExpenseCalculator calculator = new ExpenseCalculator(Module, PartnershipManager);

                _customerCost = 0;
                _runningCost = 0;
                Dictionary<CostType, double> cost = calculator.ExpenseCalculate(null, expense);
                foreach (KeyValuePair<CostType, double> pair in cost)
                {
                    if (pair.Key.IsSupplier && !pair.Key.IsDailyInput && !pair.Key.IsDaily && !pair.Key.IsMonthly &&
                        !pair.Key.IsYearly)
                    {
                        _customerCost += pair.Value;
                    }
                    else if (pair.Key.IsSupplier && !pair.Key.IsDailyInput && pair.Key.IsDaily)
                    {
                        _runningCost += pair.Value;
                    }
                }
            }

            if (DailyCost.Count > 0)
            {
                rptCruiseExpense.DataSource = cruises;
                rptCruiseExpense.DataBind();
            }
            else
            {
                plhDailyExpenses.Visible = false;
                rptCruiseExpense.Visible = false;
            }
        }

        /// <summary>
        /// Load thông tin các chi phí nhập thủ công theo ngày
        /// </summary>
        /// <param name="rptServices"></param>
        /// <param name="cruise"></param>
        /// <param name="date"></param>
        private void LoadService(Repeater rptServices, Cruise cruise, DateTime date)
        {
            #region -- Load service --

            Expense expense = Module.ExpenseGetByDate(cruise, date);

            Dictionary<CostType, bool> serviceMap = new Dictionary<CostType, bool>();

            foreach (CostType type in DailyCost)
            {
                serviceMap.Add(type, false);
            }

            IList services = new ArrayList();

            // Lấy dịch vụ trong cơ sở dữ liệu vào
            foreach (ExpenseService service in expense.Services)
            {
                try
                {
                    if (service.Type.IsDailyInput && !service.Type.IsMonthly && !service.Type.IsYearly)
                    {
                        serviceMap[service.Type] = true;
                        services.Add(service);
                        _numberOfGroups = Math.Max(_numberOfGroups, service.Group);
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }

            foreach (CostType type in DailyCost)
            {
                if (!serviceMap[type])
                {
                    ExpenseService service = new ExpenseService();
                    service.Type = type;
                    service.IsOwnService = !type.IsSupplier;
                    services.Add(service);
                }
            }

            var tempServices = new List<ExpenseService>();
            ExpenseService firstOperator = null;
            foreach (ExpenseService service in services)
            {
                if (service.Type.Name == "Operator")
                {
                    if (firstOperator != null)
                    {
                        var tempExpense = service.Expense;
                        tempExpense.Services.Remove(service);
                        Module.SaveOrUpdate(tempExpense);
                        continue;
                    }
                    else
                    {
                        firstOperator = service;
                    }
                }
                tempServices.Add(service);
            }


            rptServices.DataSource = tempServices;
            rptServices.DataBind();

            #endregion

            #region -- Load note --

            if (ActiveCruise != null)
            {
                DayNote note = Module.DayNoteGetByDay(ActiveCruise, date);
            }

            #endregion
        }

        protected IList<ExpenseService> rptServicesToIList(Repeater rptServices)
        {
            IList<ExpenseService> list = new List<ExpenseService>();
            foreach (RepeaterItem item in rptServices.Items)
            {
                HiddenField hiddenId = (HiddenField)item.FindControl("hiddenId");
                HiddenField hiddenType = (HiddenField)item.FindControl("hiddenType");
                TextBox txtName = (TextBox)item.FindControl("txtName");
                TextBox txtPhone = (TextBox)item.FindControl("txtPhone");
                DropDownList ddlSuppliers = (DropDownList)item.FindControl("ddlSuppliers");
                DropDownList ddlGuides = (DropDownList)item.FindControl("ddlGuides");
                DropDownList ddlGroups = (DropDownList)item.FindControl("ddlGroups");
                TextBox txtCost = (TextBox)item.FindControl("txtCost");

                int serviceId = Convert.ToInt32(hiddenId.Value);

                ExpenseService service;
                if (serviceId <= 0)
                {
                    service = new ExpenseService();
                }
                else
                {
                    service = Module.ExpenseServiceGetById(serviceId);
                }
                service.Type = Module.CostTypeGetById(Convert.ToInt32(hiddenType.Value));
                service.Name = txtName.Text;
                service.Phone = txtPhone.Text;
                if (service.Type.Id == SailsModule.GUIDE_COST)
                {
                    service.Supplier = Module.AgencyGetById(Convert.ToInt32(ddlGuides.SelectedValue));
                }
                else if (service.Type.IsSupplier)
                {
                    if (ddlSuppliers.SelectedIndex >= 0)
                    {
                        service.Supplier = Module.AgencyGetById(Convert.ToInt32(ddlSuppliers.SelectedValue));
                    }
                    else
                    {
                        service.Supplier = null;
                    }
                }
                service.IsOwnService = !service.Type.IsSupplier;
                service.Cost = Convert.ToDouble(txtCost.Text);
                service.IsRemoved = !item.Visible;
                if (ddlGroups.SelectedIndex >= 0)
                {
                    service.Group = Convert.ToInt32(ddlGroups.SelectedValue);
                }
                list.Add(service);
            }
            return list;
        }

        #endregion

        protected void btnIncomeDate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDate.Text))
            {
                _date = DateTime.Today;
            }
            else
            {
                _date = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            IList data = Module.BookingGetByStartDate(_date, null, false);

            ReportEngine.IncomeByDate(data, this, Response,
                                      Server.MapPath("/Modules/Sails/Admin/ExportTemplates/IncomeDate.xls"));
            //IncomeByDate.Emotion(data, this, Response,
            //                     Server.MapPath("/Modules/Sails/Admin/ExportTemplates/IncomeDate.xls"));
        }

        protected void rptCruises_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem is Cruise)
            {
                var cruise = (Cruise)e.Item.DataItem;
                HyperLink hplCruises = e.Item.FindControl("hplCruises") as HyperLink;
                hplCruises.CssClass = "btn btn-default";
                if (hplCruises != null)
                {
                    if (cruise.Id.ToString() == Request.QueryString["cruiseid"])
                    {
                        hplCruises.CssClass = "btn btn-default active";
                    }

                    DateTime date = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    int cabins;
                    int people = Module.CountBooked(cruise, Date, out cabins);

                    hplCruises.Text = string.Format("{0} ({1} pax/{2} cabins)", cruise.Name, people, cabins);
                    hplCruises.NavigateUrl = string.Format(
                        "BookingReport.aspx?NodeId={0}&SectionId={1}&Date={2}&cruiseid={3}", Node.Id, Section.Id,
                        date.ToString("dd/MM/yyyy"), cruise.Id);
                }
            }
            else
            {
                HyperLink hplCruises = e.Item.FindControl("hplCruises") as HyperLink;
                if (hplCruises != null)
                {
                    if (Request.QueryString["cruiseid"] == null)
                    {
                        hplCruises.CssClass = "btn btn-default active";
                        if (IsLimousineTab)
                        {
                            hplCruises.Attributes.Remove("class");
                        }
                    }
                    DateTime date = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    hplCruises.NavigateUrl = string.Format(
                        "BookingReport.aspx?NodeId={0}&SectionId={1}&Date={2}", Node.Id, Section.Id, date.ToString("dd/MM/yyyy"));
                }
            }
        }

        protected void rptCruiseExpense_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem is Cruise)
            {
                Cruise cruise = (Cruise)e.Item.DataItem;
                PlaceHolder plhCruiseExpense = e.Item.FindControl("plhCruiseExpense") as PlaceHolder;
                if (cruise.Name == "Transfer" || cruise.Name == "OS Scorpio" || cruise.Name == "Daily Charter")
                {
                    plhCruiseExpense.Visible = false;
                }
                Repeater rptServices = e.Item.FindControl("rptServices") as Repeater;
                LoadService(rptServices, cruise, _date);

                Repeater rptAddServices = e.Item.FindControl("rptAddServices") as Repeater;
                if (rptAddServices == null) return;
                if (Request.QueryString["cruiseid"] != null)
                {
                    rptAddServices.DataSource = _services;
                    rptAddServices.DataBind();
                }
                var btnAddServiceBlock = (Button)e.Item.FindControl("btnAddServiceBlock");
                btnAddServiceBlock.CommandArgument = cruise.Id.ToString();
                _currentGroup = 0;
            }
            else
            {
                Literal litSTotal = e.Item.FindControl("litTotal") as Literal;
                if (litSTotal != null)
                {
                    litSTotal.Text = _totalCost.ToString("#,###.##");
                }
            }
        }

        protected void btnLock_Click(object sender, EventArgs e)
        {
            // Check xem nếu có lock chưa
            Locked locked = Module.LockedCheckByDate(ActiveCruise, Date);
            locked.Description = txtLockDescription.Text;

            if (locked.Id > 0)
            {
                Module.Delete(locked);
            }
            else
            {
                Module.SaveOrUpdate(locked);
            }
            GetDataSource();
        }

        #region -- LOCK INCOME --
        /// <summary>
        /// Khi người dùng khóa doanh thu
        /// 01. Check quyền
        /// 02. Khóa từng tàu, hoặc khóa tàu hiện tại
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnLockIncome_Click(object sender, EventArgs e)
        {
            // Check quyền
            if (!Module.PermissionCheck(Permission.ACTION_LOCKINCOME, UserIdentity))
            {
                ShowErrors("You don't have permission to lock income");
                return;
            }

            DateTime date;
            if (Request.QueryString["date"] != null)
            {
                date = DateTime.FromOADate(Convert.ToDouble(Request.QueryString["date"]));
            }
            else
            {
                date = DateTime.Today;
            }

            // Khóa doanh thu
            // Nếu là toàn bộ tàu thì khóa từng tàu (nếu chưa khóa)
            if (Request.QueryString["cruiseid"] != null)
            {
                var cruise = Module.CruiseGetById(Convert.ToInt32(Request.QueryString["cruiseid"]));
                var expense = Module.ExpenseGetByDate(cruise, date);
                if (!expense.LockIncome) // Chỉ khóa nếu chưa khóa
                {
                    expense.LockIncome = true;
                    expense.LockBy = UserIdentity;
                    expense.LockDate = DateTime.Now;
                    Module.SaveOrUpdate(expense);
                }
            }
            else
            {
                var list = Module.CruiseGetAll();
                foreach (Cruise cruise in list)
                {
                    var expense = Module.ExpenseGetByDate(cruise, date);
                    if (!expense.LockIncome) // Chỉ khóa nếu chưa khóa
                    {
                        expense.LockIncome = true;
                        expense.LockBy = UserIdentity;
                        expense.LockDate = DateTime.Now;
                        Module.SaveOrUpdate(expense);
                    }
                }
            }

            PageRedirect(Request.RawUrl);
        }
        #endregion
        protected void btnViewFeedback_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            string url = string.Format("../FeedbackReport.aspx?NodeId={0}&SectionId={1}&from={2}&to={2}", Node.Id, Section.Id,
                                       date.ToOADate());
            PageRedirect(url);
        }

        protected void btnExport_3_Click(object sender, EventArgs e)
        {
            #region -- Lưu thông tin expense --

            IList list;
            int count = GetData(out list, false);
            if (count == 0)
            {
                ShowErrors(Resources.errorNoBookingSave);
                return;
            }
            DateTime date = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            IList<ExpenseService> expenseList = new List<ExpenseService>();
            IList<ExpenseService> expenseListExport = new List<ExpenseService>();
            foreach (RepeaterItem rptItem in rptCruiseExpense.Items)
            {
                HiddenField hiddenId = (HiddenField)rptItem.FindControl("hiddenId");
                Cruise cruise = Module.CruiseGetById(Convert.ToInt32(hiddenId.Value));
                Expense expense = Module.ExpenseGetByDate(cruise, date);
                Module.SaveOrUpdate(expense);
                Repeater rptServices = (Repeater)rptItem.FindControl("rptServices");
                expenseList = rptServicesToIList(rptServices);

                if (String.IsNullOrEmpty(Request.QueryString["cruiseid"]))
                {
                    for (int i = 0; i < rptServicesToIList(rptServices).Count; i++)
                    {
                        expenseListExport.Add(rptServicesToIList(rptServices)[i]);
                    }
                }
                else
                {
                    expenseListExport = rptServicesToIList(rptServices);
                }
                foreach (ExpenseService service in expenseList)
                {
                    service.Expense = expense;
                    if (service.IsRemoved)
                    {
                        if (service.Id > 0)
                        {
                            expense.Services.Remove(service);
                        }
                    }
                    else
                    {
                        // Phải có tên hoặc giá thì mới lưu
                        if (!string.IsNullOrEmpty(service.Name) || service.Cost > 0 ||
                            service.Type.Id == SailsModule.GUIDE_COST)
                        {
                            Module.SaveOrUpdate(service);
                        }
                    }
                }

                Module.SaveOrUpdate(expense);
            }

            if (ActiveCruise != null)
            {
                DayNote note = Module.DayNoteGetByDay(ActiveCruise, date);

                if (!string.IsNullOrEmpty(note.Note) || note.Id > 0)
                {
                    Module.SaveOrUpdate(note);
                }
            }

            LoadService(date);

            if (ActiveCruise != null)
            {
                BarRevenue bar = Module.BarRevenueGetByDate(ActiveCruise, date);
                Module.SaveOrUpdate(bar);
            }

            #endregion

            string tplPath = Server.MapPath("/Modules/Sails/Admin/ExportTemplates/Lenhdieutour_dayboat.xls");
            if (String.IsNullOrEmpty(Request.QueryString["cruiseid"]))
                TourCommand.Export2(list, count, expenseListExport, _date, BookingFormat, Response, tplPath, null);
            else
            {
                var cruise = Module.GetObject<Cruise>(Convert.ToInt32(Request.QueryString["cruiseid"]));
                TourCommand.Export2(list, count, expenseListExport, _date, BookingFormat, Response, tplPath, cruise);
            }

        }

        protected void btnUnlockIncome_Click(object sender, EventArgs e)
        {
            // Check quyền
            if (!Module.PermissionCheck(Permission.ACTION_LOCKINCOME, UserIdentity))
            {
                ShowErrors("You don't have permission to lock income");
                return;
            }

            DateTime date;
            if (Request.QueryString["date"] != null)
            {
                date = DateTime.FromOADate(Convert.ToDouble(Request.QueryString["date"]));
            }
            else
            {
                date = DateTime.Today;
            }

            // Khóa doanh thu
            // Nếu là toàn bộ tàu thì mở khóa từng tàu (nếu đã khóa)
            if (Request.QueryString["cruiseid"] != null)
            {
                var cruise = Module.CruiseGetById(Convert.ToInt32(Request.QueryString["cruiseid"]));
                var expense = Module.ExpenseGetByDate(cruise, date);
                if (expense.LockIncome) // Chỉ mở khóa nếu đã khóa
                {
                    expense.LockIncome = false;
                    expense.LockBy = null;
                    expense.LockDate = null;
                    Module.SaveOrUpdate(expense);
                }
            }
            else
            {
                var list = Module.CruiseGetAll();
                foreach (Cruise cruise in list)
                {
                    var expense = Module.ExpenseGetByDate(cruise, date);
                    if (!expense.LockIncome) // Chỉ mở khóa nếu đã khóa
                    {
                        expense.LockIncome = false;
                        expense.LockBy = null;
                        expense.LockDate = null;
                        Module.SaveOrUpdate(expense);
                    }
                }
            }

            PageRedirect(Request.RawUrl);
        }

        protected void btnAddServiceBlock_Click(object sender, EventArgs e)
        {
            Button btnAddServiceBlock = (Button)sender;
            Repeater rptServices = btnAddServiceBlock.Parent.FindControl("rptServices") as Repeater;
            IList<ExpenseService> list = rptServicesToIList(rptServices);

            Cruise cruise = Module.CruiseGetById(Convert.ToInt32(btnAddServiceBlock.CommandArgument));

            //CostType costType = Module.CostTypeGetById(Convert.ToInt32(btnAddService.CommandArgument));

            int maxGroup = 1;
            foreach (ExpenseService temp in list)
            {
                maxGroup = Math.Max(temp.Group, maxGroup);
            }

            foreach (CostType costType in DailyCost)
            {
                ExpenseService service = new ExpenseService();
                service.Type = costType;
                service.IsRemoved = !costType.IsSupplier;
                service.Group = maxGroup + 1;

                int index = 0;
                foreach (ExpenseService temp in list)
                {
                    index += 1;
                }

                list.Insert(index, service);
            }
            _numberOfGroups = maxGroup + 1; // khi thêm nhóm thì tăng số lượng nhóm lên 1
            rptServices.DataSource = list;
            rptServices.DataBind();
        }
        public void ShowWarning(string warning)
        {
            Session["WarningMessage"] = "<strong>Warning!</strong> " + warning + "<br/>" + Session["WarningMessage"];
        }

        public void ShowErrors(string error)
        {
            Session["ErrorMessage"] = "<strong>Error!</strong> " + error + "<br/>" + Session["ErrorMessage"];
        }

        public void ShowSuccess(string success)
        {
            Session["SuccessMessage"] = "<strong>Success!</strong> " + success + "<br/>" + Session["SuccessMessage"];
        }

    }
}