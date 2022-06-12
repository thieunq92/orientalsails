<%@ Page Title="" Language="C#" MasterPageFile="MO.Master" AutoEventWireup="true" CodeBehind="TripConfigPriceAdd.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.TripConfigPriceAdd" %>

<%@ Register Src="Controls/CruisePriceConfig.ascx" TagPrefix="uc1" TagName="CruisePriceConfig" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <title>Cập nhật giá trip</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdminContent" runat="server">
    <asp:UpdatePanel ID="upPanel" runat="server">
        <ContentTemplate>
            <fieldset class="price-cruise">
            <legend>
                <div class="page-header">
                    <h3>Cấu hình giá trip</h3>
                </div>
            </legend>
           
            <div class="form-group">
                <div class="row">
                    <div class="col-xs-1">
                        Trip
                    </div>
                    <div class="col-xs-3">
                        <asp:DropDownList ID="ddlTrips" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlTrips_OnSelectedIndexChanged" class="form-control">
                        </asp:DropDownList>
                    </div>
                    <div class="col-xs-offset-1 col-xs-1">
                        Từ ngày
                    </div>
                    <div class="col-xs-2">
                        <asp:TextBox ID="txtFromDate" runat="server" data-control="datetimepicker" class="form-control" autocomplete="off" placeholder="Từ ngày"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFromDate" ErrorMessage="*** Chọn ngày ***"></asp:RequiredFieldValidator>
                    </div>
                    <div class="col-xs-offset-1 col-xs-1">
                        Đến ngày
                    </div>
                    <div class="col-xs-2">
                        <asp:TextBox ID="txtToDate" runat="server" data-control="datetimepicker" class="form-control" autocomplete="off" placeholder="Đến ngày"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtToDate" ErrorMessage="*** Chọn ngày ***"></asp:RequiredFieldValidator>
                    </div>
                </div>
            </div>
            <asp:Repeater runat="server" ID="rptCruise" OnItemDataBound="rptCruise_OnItemDataBound">
                <ItemTemplate>
                    <uc1:CruisePriceConfig runat="server" ID="CruisePriceConfig" />
                </ItemTemplate>
            </asp:Repeater>
            <div class="form-group">
                <div class="row">
                    <div class="col-xs-12">
                        <div class="input-group">
                            <%--                        <span class="input-group-addon"><strong>Charter</strong></span>--%>
                            <asp:Button runat="server" ID="btnSave" OnClick="btnSave_OnClick" Text="Lưu giá trip" CssClass="btn btn-success" />
                            &nbsp;<asp:Button runat="server" ID="btnBack" OnClientClick="backList();return false;" Text="Quay lại" CssClass="btn btn-primary" />
                        </div>
                    </div>
                </div>
            </div>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
    <style>
        .page-header { color: #087CCE; }
        .price-cruise{margin: 50px}
    </style>
    <script>
        function backList() {
            window.location.href = '/Modules/Sails/Admin/TripConfigPriceList.aspx?NodeId=1&SectionId=15';
        }
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Scripts" runat="server">
</asp:Content>
