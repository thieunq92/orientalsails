<%@ Page Title="Allotment Config" Language="C#" MasterPageFile="MO.Master" AutoEventWireup="true" CodeBehind="KeepRoomConfig.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.KeepRoomConfig" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="page-header">
        <h3>Group cruise management</h3>
    </div>
    <div class="row">
        <div class="col-xs-12">
            <table class="table table-bordered table-hover table-common">
                <tr class="active">
                    <th>Group</th>
                    <th>Number keep room</th>
                    <th></th>
                </tr>
                <asp:Repeater ID="rptGroup" runat="server" OnItemDataBound="rptGroup_OnItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:HiddenField runat="server" ID="cid" Value='<%#Eval("Id") %>' />
                                <%#Eval("Name") %>
                            </td>
                            <td>
                                <asp:DropDownList runat="server" CssClass="form-control" ID="ddlNumberKeepRoom" /></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr>
                    <td></td>
                    <td>
                        <asp:Button runat="server" ID="btnSave" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_OnClick" /></td>

                </tr>
            </table>
        </div>
    </div>
</asp:Content>
