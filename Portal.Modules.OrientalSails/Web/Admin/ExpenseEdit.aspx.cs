using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using NHibernate.Criterion;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.UI;
using Portal.Modules.OrientalSails.Web.Util;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class ExpenseEdit : SailsAdminBase
    {
        #region -- PRIVATE MEMBERS --

        private IList _allCostTypes;
        private IList _autoCostTypes;
        private CruiseExpenseTable _cruiseTable;
        private Dictionary<CostType, double> _currentCostMap;
        private double _currentTotal;
        private DailyCostTable _dailyTable;
        private IList _monthlyCost;
        private IList _yearlyCost;

        /// <summary>
        /// Tổng chi phí của cả thời kỳ báo cáo theo từng dịch vụ
        /// </summary>
        private Dictionary<CostType, double> _grandTotal;

        /// <summary>
        /// Tổng chi phí trung bình tháng
        /// </summary>
        private double _month;
        /// <summary>
        /// Tổng chi phí trung bình năm
        /// </summary>
        private double _year;

        private int _pax;
        private CostingTable _table;
        private CostingTable[,] _tableCache;

        /// <summary>
        /// Bảng lưu chi phí tháng theo mốc thời gian và theo tàu
        /// </summary>
        private readonly Dictionary<Cruise, Dictionary<DateTime, double>> _monthExpenseCruise =
            new Dictionary<Cruise, Dictionary<DateTime, double>>();

        /// <summary>
        /// Bảng lưu chi phí năm theo mốc thời gian và theo tàu
        /// </summary>
        private readonly Dictionary<Cruise, Dictionary<DateTime, double>> _yearExpenseCruise =
            new Dictionary<Cruise, Dictionary<DateTime, double>>();

        /// <summary>
        /// Biến lưu bảng chi phí tháng theo mốc thời gian
        /// </summary>
        private readonly Dictionary<DateTime, double> _monthExpense = new Dictionary<DateTime, double>();

        /// <summary>
        /// Biến lưu bảng chi phí tháng theo mốc thời gian
        /// </summary>
        private readonly Dictionary<DateTime, double> _yearExpense = new Dictionary<DateTime, double>();

        protected IList AutoCalTypes
        {
            get
            {
                if (_autoCostTypes == null)
                {
                    _autoCostTypes = Module.CostTypeGetNotInput();
                }
                return _autoCostTypes;
            }
        }

        protected IList AllCostTypes
        {
            get
            {
                if (_allCostTypes == null)
                {
                    _allCostTypes = Module.CostTypeGetDailyCost();
                }
                return _allCostTypes;
            }
        }

        protected IList MonthlyCost
        {
            get
            {
                if (_monthlyCost == null)
                {
                    _monthlyCost = Module.CostTypeGetMonthly();
                }
                return _monthlyCost;
            }
        }

        protected IList YearlyCost
        {
            get
            {
                if (_yearlyCost == null)
                {
                    _yearlyCost = Module.CostTypeGetYearly();
                }
                return _yearlyCost;
            }
        }

        private Cruise _cruise;
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

        protected void Plus(Dictionary<CostType, double> currentCostMap)
        {
            if (_grandTotal == null)
            {
                _grandTotal = new Dictionary<CostType, double>();
                foreach (CostType type in AllCostTypes)
                {
                    _grandTotal.Add(type, currentCostMap[type]);
                }
            }
            else
            {
                foreach (CostType type in AllCostTypes)
                {
                    _grandTotal[type] += currentCostMap[type];
                }
            }
        }

        #endregion

        #region -- PAGE EVENTS --

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = "Báo cáo chi phí";
            if (!IsPostBack)
            {
                if (Request.QueryString["from"] != null)
                {
                    DateTime from = DateTime.FromOADate(Convert.ToDouble(Request.QueryString["from"]));
                    txtFrom.Text = from.ToString("dd/MM/yyyy");
                }
                if (Request.QueryString["to"] != null)
                {
                    DateTime to = DateTime.FromOADate(Convert.ToDouble(Request.QueryString["to"]));
                    txtTo.Text = to.ToString("dd/MM/yyyy");
                }
                GetDataSource();
                BindCruises();
            }
        }

        protected void BindCruises()
        {
            IList cruises = Module.CruiseGetAll();
            if (cruises.Count == 1)
            {
                if (ActiveCruise == null)
                {
                    Cruise cruise = (Cruise)cruises[0];
                    PageRedirect(string.Format("ExpenseReport.aspx?NodeId={0}&SectionId={1}&cruiseid={2}", Node.Id, Section.Id, cruise.Id));
                }
                else
                {
                    rptCruises.Visible = false;
                }
            }
            else
            {
                rptCruises.DataSource = cruises;
                rptCruises.DataBind();
            }
        }

        #endregion

        #region -- CONTROL EVENTS --

        protected virtual void rptBookingList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Control ctrl = e.Item.FindControl("plhPeriodExpense");
            if (ctrl != null)
            {
                ctrl.Visible = PeriodExpenseAvg;
            }

            if (e.Item.DataItem is Expense)
            {
                #region -- Tính toán chi phí & hiển thị --

                Expense expense = ExpenseCalculate(e);

                Plus(_currentCostMap); // Cộng bảng chi phí

                #region -- Show info --
                Repeater rptServices = (Repeater)e.Item.FindControl("rptServices");
                rptServices.DataSource = AllCostTypes;
                rptServices.DataBind();

                Literal litTotal = e.Item.FindControl("litTotal") as Literal;
                if (litTotal != null)
                {
                    litTotal.Text = _currentTotal.ToString("#,0.#");
                }

                HyperLink hplDate = e.Item.FindControl("hplDate") as HyperLink;
                if (hplDate != null)
                {
                    hplDate.Text = expense.Date.ToString("dd/MM/yyyy");
                }

                #endregion

                return;

                #endregion
            }

            #region -- Header --
            if (e.Item.ItemType == ListItemType.Header)
            {
                Repeater rptServices = (Repeater)e.Item.FindControl("rptServices");
                rptServices.DataSource = AllCostTypes;
                rptServices.DataBind();
            }
            #endregion

            #region -- Footer --
            if (e.Item.ItemType == ListItemType.Footer)
            {
                Repeater rptServices = (Repeater)e.Item.FindControl("rptServices");
                rptServices.DataSource = AllCostTypes;
                rptServices.DataBind();

                double total = 0;
                foreach (CostType type in AllCostTypes)
                {
                    total += _grandTotal[type];
                }

                total += _month;
                total += _year;

                Literal litMonth = e.Item.FindControl("litMonth") as Literal;
                if (litMonth != null)
                {
                    litMonth.Text = _month.ToString("#,0.#");
                }

                Literal litYear = e.Item.FindControl("litYear") as Literal;
                if (litYear != null)
                {
                    litYear.Text = _year.ToString("#,0.#");
                }

                Literal litTotal = e.Item.FindControl("litTotal") as Literal;
                if (litTotal != null)
                {
                    litTotal.Text = total.ToString("#,0.#");
                }
            }
            #endregion
        }

        protected virtual void rptServices_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem is CostType)
            {
                CostType type = (CostType)e.Item.DataItem;
                Literal litCost = e.Item.FindControl("litCost") as Literal;
                if (litCost != null)
                {
                    litCost.Text = _currentCostMap[type].ToString("#,0.#");
                    _currentTotal += _currentCostMap[type];
                }
            }
        }

        protected virtual void rptServiceTotal_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem is CostType)
            {
                CostType type = (CostType)e.Item.DataItem;
                Literal litCost = e.Item.FindControl("litCost") as Literal;
                if (litCost != null)
                {
                    litCost.Text = _grandTotal[type].ToString("#,0.#");
                }
            }
        }

        protected void rptBookingList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
        }

        protected void btnDisplay_Click(object sender, EventArgs e)
        {
            DateTime from = DateTime.ParseExact(txtFrom.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(txtTo.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            PageRedirect(string.Format("ExpenseReport.aspx?NodeId={0}&SectionId={1}&from={2}&to={3}", Node.Id, Section.Id, from.ToOADate(), to.ToOADate()));
        }

        protected void rptCruises_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem is Cruise)
            {
                Cruise cruise = (Cruise)e.Item.DataItem;

                HtmlGenericControl liMenu = e.Item.FindControl("liMenu") as HtmlGenericControl;
                if (liMenu != null)
                {
                    if (cruise.Id.ToString() == Request.QueryString["cruiseid"])
                    {
                        liMenu.Attributes.Add("class", "selected");
                    }
                }

                HyperLink hplCruises = e.Item.FindControl("hplCruises") as HyperLink;
                if (hplCruises != null)
                {
                    DateTime from = DateTime.ParseExact(txtFrom.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime to = DateTime.ParseExact(txtTo.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    hplCruises.Text = cruise.Name;
                    hplCruises.NavigateUrl =
                        string.Format("ExpenseReport.aspx?NodeId={0}&SectionId={1}&from={2}&to={3}&cruiseid={4}", Node.Id, Section.Id, from.ToOADate(), to.ToOADate(), cruise.Id);
                }
            }
            else
            {
                HtmlGenericControl liMenu = e.Item.FindControl("liMenu") as HtmlGenericControl;
                if (liMenu != null)
                {
                    if (Request.QueryString["cruiseid"] == null)
                    {
                        liMenu.Attributes.Add("class", "selected");
                    }
                }
                HyperLink hplCruises = e.Item.FindControl("hplCruises") as HyperLink;
                if (hplCruises != null)
                {
                    DateTime from = DateTime.ParseExact(txtFrom.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime to = DateTime.ParseExact(txtTo.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    hplCruises.NavigateUrl =
                        string.Format("ExpenseReport.aspx?NodeId={0}&SectionId={1}&from={2}&to={3}", Node.Id,
                                      Section.Id, from.ToOADate(), to.ToOADate());
                }
            }
        }

        #endregion

        #region -- METHODS --

        protected void GetDataSource()
        {
            try
            {
                // Ngày bắt đầu và ngày két thúc
                DateTime from;
                DateTime to;
                if (string.IsNullOrEmpty(txtFrom.Text) || string.IsNullOrEmpty(txtTo.Text))
                {
                    from = DateTime.Today.AddDays(-DateTime.Today.Day + 1);
                    to = from.AddMonths(1).AddDays(-1);
                }
                else
                {
                    from = DateTime.ParseExact(txtFrom.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    to = DateTime.ParseExact(txtTo.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                txtFrom.Text = from.ToString("dd/MM/yyyy");
                txtTo.Text = to.ToString("dd/MM/yyyy");
                if (to.Subtract(from).Days > 100)
                {
                    throw new Exception("Kỳ hạn hiển thị quá dài, có thể gây quá tải hệ thống");
                }

                IList list = Module.ExpenseGetByDate(ActiveCruise, from, to);

                // Bổ sung các ngày còn thiếu
                if (from <= to && (list.Count == 0 || list.Count < to.Subtract(from).Days + 1))
                {
                    IDictionary<DateTime, Expense> dic = new Dictionary<DateTime, Expense>();
                    foreach (Expense expense in list)
                    {
                        if (!dic.ContainsKey(expense.Date))
                        {
                            dic.Add(expense.Date, expense);
                        }
                    }
                    DateTime current;
                    current = from; // Bắt đầu từ ngày đầu tiên
                    while (current <= to)
                    {
                        if (!dic.ContainsKey(current))
                        {
                            Expense expense = new Expense();
                            expense.Date = current;
                            expense.Cruise = ActiveCruise;
                            Module.SaveOrUpdate(expense);
                        }
                        current = current.AddDays(1);
                    }

                    //Cuối cùng refresh lại danh sách là xong
                    list = Module.ExpenseGetByDate(ActiveCruise, from, to);
                }
                rptBookingList.DataSource = list;
                rptBookingList.DataBind();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                ShowError(ex.Message);
            }
        }

        protected CostingTable GetCurrentTable(DateTime date, SailsTrip trip, TripOption option)
        {
            #region -- costing table --

            if (_tableCache == null)
            {
                // Lấy về mảng costing table
                int trips = Module.TripMaxId() + 1;
                const int options = 3;
                _tableCache = new CostingTable[trips, options];
            }

            // Nếu bảng giá tại vị trí này là null hoặc hết hạn
            if (_tableCache[trip.Id, (int)option] == null || _tableCache[trip.Id, (int)option].ValidTo < date)
            {
                _tableCache[trip.Id, (int)option] = Module.CostingTableGetValid(date, trip, option);
            }

            _table = _tableCache[trip.Id, (int)option];

            #endregion

            if (_table == null)
            {
                throw new Exception(string.Format("No costing table for {0:dd/MM/yyyy}, {1} {2}", date, trip.Name,
                                                  option));
            }

            return _table;
        }

        // Lấy bảng giá tại thời điểm
        protected DailyCostTable GetCurrentDailyTable(DateTime date)
        {
            if (_dailyTable == null || _dailyTable.ValidTo < date)
            {
                _dailyTable = Module.DailyCostTableGetValid(date);
            }

            if (_dailyTable == null && Module.HasRunningCost)
            {
                throw new Exception(string.Format("Không có bảng giá chuyến cho {0:dd/MM/yyyy}", date));
            }

            return _dailyTable;
        }

        /// <summary>
        /// Lấy báng giá tàu Hải Phong tại thời điểm
        /// </summary>
        /// <param name="date"></param>
        /// <param name="cruise"></param>
        protected CruiseExpenseTable GetCurrentCruiseTable(DateTime date, Cruise cruise)
        {
            #region -- cruise table --

            bool isNeedNewTable = false;
            if (_cruiseTable != null)
            {
                if (_cruiseTable.ValidFrom > date || _cruiseTable.ValidTo < date || _cruiseTable.Cruise != cruise)
                {
                    isNeedNewTable = true;
                }
            }
            else
            {
                isNeedNewTable = true;
            }

            if (isNeedNewTable)
            {
                _cruiseTable = Module.CruiseTableGetValid(date, cruise);
            }

            #endregion

            return _cruiseTable;
        }

        /// <summary>
        /// Tính toán chi phí cho repeater item lưu thông tin expense
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private Expense ExpenseCalculate(RepeaterItemEventArgs e)
        {
            #region -- Thông tin chung của expense --

            Expense expense = (Expense)e.Item.DataItem;
            // Khi tính chi phí thì chỉ tính theo khách đã check-in
            ICriterion criterion = Expression.And(Expression.Eq("StartDate", expense.Date.Date),
                                                  Expression.Eq("Status", StatusType.Approved));
            // Bỏ deleted và cả transfer
            criterion = Expression.And(Expression.Eq("Deleted", false), criterion);
            criterion = Expression.And(Expression.Eq("IsTransferred", false), criterion);

            // Nếu là trang báo cáo chi phí từng tàu thì chỉ lấy theo tàu đó
            if (ActiveCruise != null)
            {
                criterion = Expression.And(criterion, Expression.Eq("Cruise", ActiveCruise));
            }

            IList bookings =
                Module.BookingGetByCriterion(criterion, null, 0, 0);

            int adult = 0;
            int child = 0;
            //int baby = 0;
            foreach (Booking booking in bookings)
            {
                adult += booking.Adult;
                child += booking.Child;
            }
            _pax += adult + child;

            #endregion

            if (ActiveCruise != null)
            {
                GetCurrentCruiseTable(expense.Date, ActiveCruise);
            }

            _currentTotal = 0; // Tổng cho ngày hiện tại

            // Chi phí tháng/năm chỉ được tính vào chi phí ngày nếu cho phép tính trung bình
            if (PeriodExpenseAvg)
            {
                #region -- Chi phí tháng --

                // Nếu là tính chi phí cho một tàu thì chia chi phí bình thường
                if (expense.Cruise != null)
                {
                    // Nếu có chạy hoặc là tháng chưa kết thúc
                    if (bookings.Count > 0 ||
                        expense.Date.Month + expense.Date.Year * 12 >= DateTime.Today.Month + DateTime.Today.Year * 12)
                    {
                        // Tính chi phí tháng
                        Literal litMonth = e.Item.FindControl("litMonth") as Literal;
                        if (litMonth != null)
                        {
                            DateTime dateMonth = new DateTime(expense.Date.Year, expense.Date.Month, 1);
                            if (!_monthExpense.ContainsKey(dateMonth))
                            {
                                int runcount; // Số ngày tàu chạy trong tháng, chỉ phục vụ việc tính chi phí trung bình
                                // Không cần tính lại trong mỗi lần lặp
                                // Nếu là tháng chưa kết thúc
                                if (dateMonth.AddMonths(1) > DateTime.Today)
                                {
                                    runcount = dateMonth.AddMonths(1).Subtract(dateMonth).Days;
                                }
                                else
                                {
                                    runcount = Module.RunningDayCount(expense.Cruise, expense.Date.Year,
                                                                      expense.Date.Month);
                                }

                                Expense monthExpense = Module.ExpenseGetByDate(expense.Cruise, dateMonth);
                                if (monthExpense.Id < 0)
                                {
                                    Module.SaveOrUpdate(monthExpense);
                                }
                                double total = Module.CopyMonthlyCost(monthExpense);
                                _monthExpense.Add(dateMonth, total / runcount);
                            }

                            litMonth.Text = _monthExpense[dateMonth].ToString("#,0.#");
                            _month += _monthExpense[dateMonth];
                            _currentTotal += _monthExpense[dateMonth];
                        }
                    }
                }
                else // Nếu là tính chi phí tổng hợp thì tính cho tất cả các tàu rồi cộng lại
                {
                    IList cruises = Module.CruiseGetAll();
                    double monthAll = 0; // tổng chi phí tháng trung bình
                    foreach (Cruise cruise in cruises)
                    {
                        DateTime dateMonth = new DateTime(expense.Date.Year, expense.Date.Month, 1);
                        // Nếu chưa có bảng chi phí cho tàu hiện tại
                        if (!_monthExpenseCruise.ContainsKey(cruise))
                        {
                            _monthExpenseCruise.Add(cruise, new Dictionary<DateTime, double>());
                            // Tạo một từ điển trắng để lưu dữ liệu
                        }

                        // Nếu chưa có chi phí của tàu hiện tại trong tháng hiện tại
                        if (!_monthExpenseCruise[cruise].ContainsKey(dateMonth))
                        {
                            int runcount;
                            // Nếu là tháng chưa kết thúc
                            if (dateMonth.AddMonths(1) > DateTime.Today)
                            {
                                runcount = dateMonth.AddMonths(1).Subtract(dateMonth).Days;
                            }
                            else
                            {
                                runcount = Module.RunningDayCount(cruise, expense.Date.Year, expense.Date.Month);
                            }

                            Expense monthExpense = Module.ExpenseGetByDate(cruise, dateMonth);
                            if (monthExpense.Id < 0)
                            {
                                Module.SaveOrUpdate(monthExpense);
                            }
                            double total = Module.CopyMonthlyCost(monthExpense);
                            _monthExpenseCruise[cruise].Add(dateMonth, total / runcount);
                        }

                        bool isRun = false;
                        if (dateMonth.AddMonths(1) <= DateTime.Today)
                        {
                            foreach (Booking booking in bookings)
                            {
                                if (booking.Cruise == cruise)
                                {
                                    isRun = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            isRun = true; // Nếu là tháng chưa kết thúc thì mặc định là mọi ngày tàu đều chạy
                        }


                        if (isRun)
                        {
                            monthAll += _monthExpenseCruise[cruise][dateMonth];
                            // cộng thêm chi phí cho tàu này, ngày này khi tàu có chạy
                        }
                    }

                    _month += monthAll;

                    Literal litMonth = e.Item.FindControl("litMonth") as Literal;
                    if (litMonth != null)
                    {
                        litMonth.Text = monthAll.ToString("#,0.#");
                    }
                }

                #endregion

                #region -- Chi phí năm --

                if (expense.Cruise != null)
                {
                    // Nếu có chạy hoặc năm chưa kết thúc
                    if (bookings.Count > 0 || expense.Date.Year >= DateTime.Today.Year)
                    {
                        // Tính chi phí năm
                        Literal litYear = e.Item.FindControl("litYear") as Literal;
                        if (litYear != null)
                        {
                            DateTime dateYear = new DateTime(expense.Date.Year, 1, 1);
                            int runcount;
                            // Nếu là năm chưa kết thúc
                            if (dateYear.AddYears(1) > DateTime.Today)
                            {
                                runcount = dateYear.AddYears(1).Subtract(dateYear).Days;
                            }
                            else
                            {
                                runcount = Module.RunningDayCount(expense.Cruise, expense.Date.Year, 0);
                            }
                            if (!_yearExpense.ContainsKey(dateYear))
                            {
                                Expense yearExpense = Module.ExpenseGetByDate(expense.Cruise, dateYear);
                                if (yearExpense.Id < 0)
                                {
                                    Module.SaveOrUpdate(yearExpense);
                                }
                                double total = Module.CopyYearlyCost(yearExpense);
                                _yearExpense.Add(dateYear, total / runcount);
                            }

                            litYear.Text = _yearExpense[dateYear].ToString("#,0.#");
                            _year += _yearExpense[dateYear];
                            _currentTotal += _yearExpense[dateYear];
                        }
                    }
                }
                else
                {
                    IList cruises = Module.CruiseGetAll();
                    double yearAll = 0; // tổng chi phí tháng trung bình
                    foreach (Cruise cruise in cruises)
                    {
                        DateTime dateYear = new DateTime(expense.Date.Year, 1, 1);

                        // Nếu chưa có bảng chi phí cho tàu hiện tại
                        if (!_yearExpenseCruise.ContainsKey(cruise))
                        {
                            _yearExpenseCruise.Add(cruise, new Dictionary<DateTime, double>());
                            // Tạo một từ điển trắng để lưu dữ liệu
                        }

                        // Nếu chưa có chi phí của tàu hiện tại trong năm hiện tại
                        if (!_yearExpenseCruise[cruise].ContainsKey(dateYear))
                        {
                            int runcount;
                            // Nếu là năm chưa kết thúc
                            if (dateYear.AddYears(1) > DateTime.Today)
                            {
                                runcount = dateYear.AddYears(1).Subtract(dateYear).Days;
                            }
                            else
                            {
                                runcount = Module.RunningDayCount(cruise, expense.Date.Year, 0);
                            }

                            Expense yearExpense = Module.ExpenseGetByDate(cruise, dateYear);
                            if (yearExpense.Id < 0)
                            {
                                Module.SaveOrUpdate(yearExpense);
                            }
                            double total = Module.CopyYearlyCost(yearExpense);

                            _yearExpenseCruise[cruise].Add(dateYear, total / runcount);
                        }

                        bool isRun = false;
                        if (dateYear.AddYears(1) <= DateTime.Today)
                        {
                            foreach (Booking booking in bookings)
                            {
                                if (booking.Cruise == cruise)
                                {
                                    isRun = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            isRun = true; // Nếu là năm chưa kết thúc thì mặc định là mọi ngày tàu đều chạy
                        }


                        if (isRun)
                        {
                            yearAll += _yearExpenseCruise[cruise][dateYear];
                            // cộng thêm chi phí cho tàu này, ngày này khi tàu có chạy
                        }
                    }

                    _year += yearAll;

                    Literal litYear = e.Item.FindControl("litYear") as Literal;
                    if (litYear != null)
                    {
                        litYear.Text = yearAll.ToString("#,0.#");
                    }
                }

                #endregion
            }

            if (_cruiseTable == null && ActiveCruise != null)
            {
                throw new Exception("Hai phong cruise price table is out of valid");
            }

            // Lấy về bảng giá đã tính
            _currentCostMap = expense.Calculate(AllCostTypes, GetCurrentTable, GetCurrentDailyTable, GetCurrentCruiseTable,
                                                expense.Cruise,
                                                bookings, Module, PartnershipManager);
            return expense;
        }

        #endregion
    }
}