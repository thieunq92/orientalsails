using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Portal.Modules.OrientalSails.BusinessLogic;
using Portal.Modules.OrientalSails.DataTransferObject;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.Admin.Utilities;
using Portal.Modules.OrientalSails.Web.UI;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class KeepRoomStatus : SailsAdminBasePage
    {
        private IList<QCruiseGroup> _cruiseGroups = new List<QCruiseGroup>();
        private Dictionary<DateTime, List<QCruiseGroup>> _dicGroupCruiseAvaiable = new Dictionary<DateTime, List<QCruiseGroup>>();
        private DateTime from = DateTime.Now;
        private DateTime to = DateTime.Now.AddMonths(1);
        public IEnumerable<RoomsAvaiableDTO> RoomsAvaiableDTO { get; set; }

        private DashBoardManagerBLL dashBoardManagerBLL;
        public DashBoardManagerBLL DashBoardManagerBLL
        {
            get
            {
                if (dashBoardManagerBLL == null)
                {
                    dashBoardManagerBLL = new DashBoardManagerBLL();
                }
                return dashBoardManagerBLL;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadInfo();
                LoadGroup();
                LoadRoomStatus();
            }
        }

        private void LoadRoomStatus()
        {
            var dateRange = new List<DateTime>();
            var current = from;
            RoomsAvaiableDTO = DashBoardManagerBLL.CruiseGetRoomsAvaiableInDateRange(from, to);

            while (current < to.AddDays(1))
            {
                dateRange.Add(current);
                current = current.AddDays(1);
            }
            var warningDates = new List<DateTime>();
            var roomsAvaiableDtos = RoomsAvaiableDTO as IList<RoomsAvaiableDTO> ?? RoomsAvaiableDTO.ToList();
            foreach (DateTime dateTime in dateRange)
            {
                var groups = new List<QCruiseGroup>();
                var isNotAvaiable = false;
                foreach (QCruiseGroup cruiseGroup in _cruiseGroups)
                {
                    var cruiseIds = cruiseGroup.Cruises.Select((x => x.Id)).ToList();
                    var qCruiseGroup = new QCruiseGroup();
                    qCruiseGroup.NumberOfKeepRoom = cruiseGroup.NumberOfKeepRoom;
                    qCruiseGroup.AvaiableRoom = roomsAvaiableDtos.Where(r => DateTimeUtil.EqualsUpToDay(r.Date, dateTime) && cruiseIds.Contains(r.CruiseId)).Sum(x => x.NoRAvaiable);
                    if (qCruiseGroup.AvaiableRoom < qCruiseGroup.NumberOfKeepRoom)
                    {
                        isNotAvaiable = true;
                    }
                    groups.Add(qCruiseGroup);
                }
                if (isNotAvaiable)
                {
                    _dicGroupCruiseAvaiable.Add(dateTime, groups);
                    warningDates.Add(dateTime);
                }
            }
            //--
            rptDateWarning.DataSource = warningDates;
            rptDateWarning.DataBind();
        }

        private void LoadInfo()
        {

            if (!string.IsNullOrWhiteSpace(Request["from"]))
            {
                from = DateTime.ParseExact(Request["from"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrWhiteSpace(Request["to"]))
            {
                to = DateTime.ParseExact(Request["to"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            txtFrom.Text = from.ToString("dd/MM/yyyy");
            txtTo.Text = to.ToString("dd/MM/yyyy");
        }

        private void LoadGroup()
        {
            _cruiseGroups = Module.GetCruiseGroup();
            rptGroupHeader1.DataSource = _cruiseGroups;
            rptGroupHeader1.DataBind();
            rptGroupHeader2.DataSource = _cruiseGroups;
            rptGroupHeader2.DataBind();
        }

        protected void btnSearch_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("KeepRoomStatus.aspx{0}&from={1}&to={2}", GetBaseQueryString(), txtFrom.Text, txtTo.Text));
        }

        protected void rptDateWarning_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var date = (DateTime)e.Item.DataItem;
            var litDate = (Literal)e.Item.FindControl("litDate");
            if (litDate != null)
            {
                litDate.Text = date.ToString("dd/MM/yyyy");
            }
            var rptRoomStatus = (Repeater)e.Item.FindControl("rptRoomStatus");
            if (rptRoomStatus != null)
            {
                var list = _dicGroupCruiseAvaiable[date];
                rptRoomStatus.DataSource = list;
                rptRoomStatus.DataBind();
            }
        }
    }
}