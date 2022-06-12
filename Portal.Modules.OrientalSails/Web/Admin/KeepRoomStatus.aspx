<%@ Page Title="Allotment Management" Language="C#" MasterPageFile="MO.Master" AutoEventWireup="true" CodeBehind="KeepRoomStatus.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.KeepRoomStatus" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="page-header">
        <h3>Group cruise status</h3>
    </div>
    <div class="form-group">
        <div class="row">
            <div class="col-xs-1">
                From
            </div>
            <div class="col-xs-2">
                <asp:TextBox runat="server" ID="txtFrom" CssClass="form-control" placeholder="Valid From (dd/mm/yyyy)" />
            </div>
            <div class="col-xs-1">
                To
            </div>
            <div class="col-xs-2">
                <asp:TextBox runat="server" ID="txtTo" CssClass="form-control" placeholder="Valid To (dd/mm/yyyy)" />
            </div>
            <div class="col-xs-1">
                <asp:Button runat="server" ID="btnSearch" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_OnClick" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-8">
            <table class="table table-bordered table-hover table-common">
                <tr class="active">
                    <th rowspan="2">Date</th>
                    <asp:Repeater runat="server" ID="rptGroupHeader1">
                        <ItemTemplate>
                            <th colspan="2"><%#Eval("Name") %></th>
                        </ItemTemplate>
                    </asp:Repeater>

                </tr>
                <tr class="active">
                    <asp:Repeater runat="server" ID="rptGroupHeader2">
                        <ItemTemplate>
                            <th>Keep room</th>
                            <th>Avaiable</th>
                        </ItemTemplate>
                    </asp:Repeater>
                </tr>
                <asp:Repeater ID="rptDateWarning" runat="server" OnItemDataBound="rptDateWarning_OnItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:Literal runat="server" ID="litDate"></asp:Literal>
                            </td>
                            <asp:Repeater runat="server" ID="rptRoomStatus">
                                <ItemTemplate>
                                    <td><%#Eval("NumberOfKeepRoom") %></td>
                                    <td style='<%# ((Int32)Eval("NumberOfKeepRoom")) > ((Int32)Eval("AvaiableRoom") ) ? "background: red;" : "" %>'><%#Eval("AvaiableRoom") %></td>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script>
        $("#<%= txtFrom.ClientID %>").datetimepicker({
            timepicker: false,
            format: 'd/m/Y',
            scrollInput: false,
            scrollMonth: false,
        })

        $("#<%= txtTo.ClientID %>").datetimepicker({
            timepicker: false,
            format: 'd/m/Y',
            scrollInput: false,
            scrollMonth: false,
        })
    </script>
</asp:Content>
