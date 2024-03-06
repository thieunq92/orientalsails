<%@ Page Language="C#" MasterPageFile="NewPopup.Master" AutoEventWireup="true" CodeBehind="CheckVoucher.aspx.cs"
    Inherits="Portal.Modules.OrientalSails.Web.Admin.CheckVoucher" Title="Untitled Page" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register Assembly="System.Web.Extensions" Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Assembly="CMS.ServerControls" Namespace="CMS.ServerControls" TagPrefix="svc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="page-header">
        <h1>Check voucher validity</h1>
    </div>

    <div class="row" style="display: none">
        <div class="col-md-6">
            <asp:TextBox runat="server" ID="txtVoucherCode" class="form-control"></asp:TextBox>
        </div>
        <div class="col-md-2">
            <asp:Button class="btn btn-primary" runat="server" ID="btnCheck" OnClick="btnCheck_ItemDataBound" Text="Check" />
        </div>
    </div>

    <asp:Repeater runat="server" ID="rptVoucher" OnItemDataBound="rptVoucher_OnItemDataBound">
        <itemtemplate>
            <asp:PlaceHolder runat="server" ID="plhValid">
                <div class="row">
                    <div class="col-md-2">
                        <label for="vouchercode">Voucher Code</label>
                    </div>
                    <div class="col-md-10">
                        <asp:Literal runat="server" ID="litVoucherCode"></asp:Literal>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label for="programname">Program name</label>
                    </div>
                    <div class="col-md-10">
                        <asp:Literal runat="server" ID="litProgramName"></asp:Literal>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label for="status">Status</label>
                    </div>
                    <div class="col-md-10">
                        <asp:Literal runat="server" ID="litStatus"></asp:Literal>&nbsp;&nbsp;
                                        <asp:Literal runat="server" ID="litUsed"></asp:Literal>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label for="vouchersissuefor">Vouchers issue for</label>
                    </div>
                    <div class="col-md-10">
                        <asp:Literal runat="server" ID="litAgency"></asp:Literal>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label for="quantity">Quantity</label>
                    </div>
                    <div class="col-md-10">
                        <asp:Literal runat="server" ID="litQuantity"></asp:Literal>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label for="applyfor">Apply for</label>
                    </div>
                    <div class="col-md-10">
                        <asp:Literal runat="server" ID="litApplyFor"></asp:Literal>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label for="cruise">Cruise</label>
                    </div>
                    <div class="col-md-10">
                        <asp:Literal runat="server" ID="litCruise"></asp:Literal>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label for="trip">Trip</label>
                    </div>
                    <div class="col-md-10">
                        <asp:Literal runat="server" ID="litTrip"></asp:Literal>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label for="value">Value</label>
                    </div>
                    <div class="col-md-10">
                        <asp:Literal runat="server" ID="litValue"></asp:Literal>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label for="validuntil">Valid until</label>
                    </div>
                    <div class="col-md-10">
                        <asp:Literal ID="litValidUntil" runat="server"></asp:Literal>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label for="issuedate">Issue date</label>
                    </div>
                    <div class="col-md-10">
                        <asp:Literal runat="server" ID="litIssueDate"></asp:Literal>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label for="contractfile">Contract file</label>
                    </div>
                    <div class="col-md-10">
                        <asp:HyperLink runat="server" ID="hplContract">Contract</asp:HyperLink>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label for="note">Note</label>
                    </div>
                    <div class="col-md-10">
                        <asp:Literal runat="server" ID="litNote"></asp:Literal>
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="plhInvalid" Visible="False">
                <div class="row">
                    <div class="col-md-12">
                        <strong>This voucher code is not valid, please check again</strong>
                    </div>
                </div>
            </asp:PlaceHolder>
        </itemtemplate>
    </asp:Repeater>
</asp:Content>
