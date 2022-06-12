<%@ Page Title="" Language="C#" MasterPageFile="MO.Master" AutoEventWireup="true" CodeBehind="CampaignConfigPriceAdd.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.CampaignConfigPriceAdd" %>

<%@ Register Src="Controls/CruisePriceConfig.ascx" TagPrefix="uc1" TagName="CruisePriceConfig" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <title>Cấu hình giá khuyến mại</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Scripts" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdminContent" runat="server">
    <asp:UpdatePanel ID="upPanel" runat="server">
        <ContentTemplate>
            <fieldset class="price-cruise">
                <legend>
                    <div class="page-header">
                        <h3>Cấu hình giá khuyến mại</h3>
                    </div>
                </legend>

                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-1">
                            Tên khuyến mại
                        </div>
                        <div class="col-xs-4">
                            <asp:TextBox runat="server" ID="txtName" TextMode="MultiLine" CssClass="form-control" autocomplete="off"></asp:TextBox>
                        </div>

                        <div class="col-xs-offset-1 col-xs-1">
                           Mã khuyến mại
                        </div>
                        <div class="col-xs-2">
                            <asp:TextBox ID="txtVoucherCode" runat="server"  CssClass="form-control" autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-xs-offset-1 col-xs-1">
                            Số lượng mã
                        </div>
                        <div class="col-xs-1">
                            <asp:TextBox ID="txtVoucherTotal" runat="server" CssClass="form-control" autocomplete="off"></asp:TextBox>
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
                                <asp:Button runat="server" ID="btnSave" OnClick="btnSave_OnClick" Text="Lưu giá khuyến mại" CssClass="btn btn-success" />
                                &nbsp;<asp:Button runat="server" ID="btnBack" OnClientClick="backList();return false;" Text="Quay lại" CssClass="btn btn-primary" />

                            </div>
                        </div>
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
    <style>
        .page-header {
            color: #087CCE;
        }

        .price-cruise {
            margin: 50px;
        }
    </style>
    <script>
        function backList() {
            window.location.href = '/Modules/Sails/Admin/GoldenDayListCampaign.aspx?NodeId=1&SectionId=15';
        }
    </script>
</asp:Content>
