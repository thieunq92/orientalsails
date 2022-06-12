using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GemBox.Spreadsheet;
using NHibernate.Mapping;
using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Web.Controls;
using Portal.Modules.OrientalSails.Web.UI;

namespace Portal.Modules.OrientalSails.Web.Admin
{
    public partial class QQuotationIssue : SailsAdminBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var agencyIssueId = Request["agencyIssueId"];
            if (!string.IsNullOrWhiteSpace(agencyIssueId))
            {
                var data = new List<QuotationSelect>();
                var list = Module.GetAgencyIssue(Convert.ToInt32(agencyIssueId));
                foreach (AgencyIssue agencyIssue in list)
                {
                    var quotationSelect = new QuotationSelect();
                    quotationSelect.AgencyLevelCode = agencyIssue.AgentLevelCode;
                    quotationSelect.QQuotation = agencyIssue.QQuotation;
                    data.Add(quotationSelect);
                }
                rptQuotationSelect.DataSource = data;
                rptQuotationSelect.DataBind();
                btnAddQuotation.Visible = false;
            }
        }

        protected void btnIssue_Click(object sender, EventArgs e)
        {
            var agency = Module.GetById<Agency>(Convert.ToInt32(Request["AgencyId"]));
            var data = new List<QuotationSelect>();
            var dic = new Dictionary<QCruiseGroup, List<QuotationSelect>>();
            var flagQuotationNull = false;
            foreach (RepeaterItem item in rptQuotationSelect.Items)
            {
                var quotationSelect = new QuotationSelect();
                var ucQuotationSelect = item.FindControl("ucQuotationSelect") as QuotationIssueSelect;
                if (ucQuotationSelect != null)
                {
                    ucQuotationSelect.SetQuotationSelect(quotationSelect, Module);
                }
                data.Add(quotationSelect);
                if (quotationSelect.QQuotation == null || quotationSelect.QQuotation.Id <= 0)
                    flagQuotationNull = true;
                else
                {
                    if (dic.ContainsKey(quotationSelect.QQuotation.GroupCruise))
                    {
                        dic[quotationSelect.QQuotation.GroupCruise].Add(quotationSelect);
                    }
                    else
                    {
                        dic.Add(quotationSelect.QQuotation.GroupCruise, new List<QuotationSelect> { quotationSelect });
                    }
                }
            }
            if (flagQuotationNull)
            {
                ShowError("Select quotation !");
                rptQuotationSelect.DataSource = data;
                rptQuotationSelect.DataBind();
            }
            else
            {
                var agencyContract = new AgencyContract();
                agencyContract.Agency = agency;
                agencyContract.CreateDate = DateTime.Now;
                string fileName = string.Format("quotation_{0}_{1:dd-MM-yyyy}.xlsx", agency.Name, DateTime.Now);
                agencyContract.FileName = fileName;
                agencyContract.ContractTemplateName = fileName;
                agencyContract.IsAgencyIssue = true;
                if (string.IsNullOrWhiteSpace(Request["agencyIssueId"])) Module.SaveOrUpdate(agencyContract);


                ExcelFile excelFile = ExcelFile.Load(Server.MapPath("/Modules/Sails/Admin/ExportTemplates/quotation.xlsx"));
                ExcelWorksheet sheet = excelFile.Worksheets[0];
                int rowQ = 3;
                foreach (KeyValuePair<QCruiseGroup, List<QuotationSelect>> keyValuePair in dic)
                {
                    var agencyIssue = new AgencyIssue();
                    agencyIssue.AgencyContract = agencyContract;
                    agencyIssue.GroupCruise = keyValuePair.Key;
                    sheet.Cells[rowQ, 1].Value = keyValuePair.Key.Name;
                    rowQ++;
                    foreach (QuotationSelect quotationSelect in keyValuePair.Value)
                    {
                        var quotation = quotationSelect.QQuotation;
                        agencyIssue.AgentLevelCode = quotationSelect.AgencyLevelCode;
                        agencyIssue.QQuotation = quotation;
                        if (string.IsNullOrWhiteSpace(Request["agencyIssueId"])) Module.SaveOrUpdate(agencyIssue);

                        sheet.Cells[rowQ, 0].Value = String.Format("FROM: {0}", quotation.Validfrom.ToString("dd/MM/yyyy"));
                        sheet.Cells[rowQ, 1].Value = String.Format("TO: {0}", quotation.Validto.ToString("dd/MM/yyyy"));
                        rowQ++;
                        WriteSheetByTrip(quotationSelect.AgencyLevelCode, "2 NGÀY / 1 ĐÊM", 2, quotation, ref rowQ, ref sheet);
                        WriteSheetByTrip(quotationSelect.AgencyLevelCode, "3 NGÀY / 2 ĐÊM", 3, quotation, ref rowQ, ref sheet);
                    }
                }
                excelFile.Save(Response, fileName + ".xlsx");
            }
        }
        private void WriteSheetByTrip(string agentLevelCode, string tripName, int tripDay, QQuotation quotation, ref int rowQ, ref ExcelWorksheet sheet)
        {
            sheet.Cells[rowQ, 1].Value = tripName;
            rowQ += 2;
            //sheet.Cells[rowQ, 0].Value = quotation.GroupCruise.Name;
            //rowQ++;
            // display price room
            // display room type header
            var roomPrices = Module.GetGroupRoomPrice(quotation.GroupCruise.Id, agentLevelCode, tripDay, quotation);
            sheet.Cells[rowQ, 0].Value = "LOẠI PHÒNG";
            SetStyleHeader(sheet.Cells[rowQ, 0]);
            sheet.Cells[rowQ, 1].Value = "Phòng đôi";
            SetStyleHeader(sheet.Cells[rowQ, 1]);
            sheet.Cells[rowQ, 2].Value = "Phòng đơn";
            SetStyleHeader(sheet.Cells[rowQ, 2]);
            sheet.Cells[rowQ, 3].Value = "Giường/Đệm phụ";
            SetStyleHeader(sheet.Cells[rowQ, 3]);
            sheet.Cells[rowQ, 4].Value = "Trẻ em";
            SetStyleHeader(sheet.Cells[rowQ, 4]);

            // display rooom class
            rowQ++;
            foreach (QGroupRomPrice roomPrice in roomPrices)
            {
                sheet.Cells[rowQ, 0].Value = roomPrice.RoomType;
                SetBorder(sheet.Cells[rowQ, 0]);
                sheet.Cells[rowQ, 1].Value = string.Format("USD: {0:#,0.#} {2}VND: {1:#,0.#}", roomPrice.PriceDoubleUsd, roomPrice.PriceDoubleVnd, Environment.NewLine);
                SetBorder(sheet.Cells[rowQ, 1]);
                sheet.Cells[rowQ, 2].Value = string.Format("USD: {0:#,0.#} {2}VND: {1:#,0.#}", roomPrice.PriceTwinUsd, roomPrice.PriceTwinVnd, Environment.NewLine);
                SetBorder(sheet.Cells[rowQ, 2]);
                sheet.Cells[rowQ, 3].Value = string.Format("USD: {0:#,0.#} {2}VND: {1:#,0.#}", roomPrice.PriceExtraUsd, roomPrice.PriceExtraVnd, Environment.NewLine);
                SetBorder(sheet.Cells[rowQ, 3]);
                sheet.Cells[rowQ, 4].Value = string.Format("USD: {0:#,0.#} {2}VND: {1:#,0.#}", roomPrice.PriceChildUsd, roomPrice.PriceChildVnd, Environment.NewLine);
                SetBorder(sheet.Cells[rowQ, 4]);
                rowQ++;
            }
            rowQ++;
            // display price charter
            sheet.Cells[rowQ, 1].Value = "THUÊ TRỌN TÀU";
            rowQ++;

            var cruises = Module.CruiseGetAllByGroup(quotation.GroupCruise.Id);


            foreach (Cruise cruise in cruises)
            {
                // write cruise name
                //sheet.Cells[rowQ, 0].Value = string.Format("{0} {1} cabins", cruise.Name, cruise.Rooms.Count);
                var charterPrices = Module.GetCruiseCharterPrice(quotation.GroupCruise.Id, cruise, agentLevelCode, tripDay, quotation);
                if (charterPrices.Count > 0)
                {
                    rowQ++;
                    var mergedRange = sheet.Cells.GetSubrange(string.Format("A{0}:A{1}", rowQ, rowQ + 1));
                    mergedRange.Merged = true;
                    mergedRange.Style.VerticalAlignment = VerticalAlignmentStyle.Center;
                    mergedRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    mergedRange.Value = string.Format("{0} {1} cabins", cruise.Name, cruise.Rooms.Count);
                    SetBorderCellRange(mergedRange);
                    var idexRange = 1;
                    foreach (QCharterPrice charterPrice in charterPrices)
                    {
                        sheet.Cells[rowQ - 1, idexRange].Value = string.Format("{0}-{1} khách", charterPrice.Validfrom, charterPrice.Validto);
                        SetStyleHeader(sheet.Cells[rowQ - 1, idexRange]);
                        sheet.Cells[rowQ, idexRange].Value = string.Format("USD: {0:#,0.#} {2}VND: {1:#,0.#}", charterPrice.Priceusd, charterPrice.Priceusd, Environment.NewLine);
                        SetBorder(sheet.Cells[rowQ, idexRange]);
                    }
                    rowQ++;
                }
            }
            rowQ += 2;
        }

        private void SetStyleHeader(ExcelCell sheetCell)
        {
            sheetCell.Style.FillPattern.SetPattern(FillPatternStyle.Solid, SpreadsheetColor.FromName(ColorName.LightGreen), Color.Empty);
            sheetCell.Style.Font.Color = Color.Black;
            sheetCell.Style.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
        }

        private void SetBorder(ExcelCell cell)
        {
            cell.Style.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
        }
        private void SetBorderCellRange(CellRange cell)
        {
            cell.Style.Borders.SetBorders(MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
        }
        protected void btnAddQuotation_OnClick(object sender, EventArgs e)
        {
            var data = new List<QuotationSelect>();
            foreach (RepeaterItem item in rptQuotationSelect.Items)
            {
                var quotationSelect = new QuotationSelect();
                var ucQuotationSelect = item.FindControl("ucQuotationSelect") as QuotationIssueSelect;
                if (ucQuotationSelect != null)
                {
                    ucQuotationSelect.SetQuotationSelect(quotationSelect, Module);
                }
                data.Add(quotationSelect);
            }
            data.Add(new QuotationSelect());
            rptQuotationSelect.DataSource = data;
            rptQuotationSelect.DataBind();
        }

        protected void rptQuotationSelect_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var qquotationSelect = e.Item.DataItem as QuotationSelect;
            if (qquotationSelect != null)
            {
                var ucQuotationSelect = e.Item.FindControl("ucQuotationSelect") as QuotationIssueSelect;
                if (ucQuotationSelect != null)
                {
                    ucQuotationSelect.DisplayQuotationSelect(qquotationSelect, Module);
                }
            }
        }
    }

    public class QuotationSelect
    {
        public QQuotation QQuotation { get; set; }
        public String AgencyLevelCode { get; set; }
    }
}