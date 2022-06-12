<%@ Page Title="" Language="C#" MasterPageFile="MO-Main.Master" AutoEventWireup="true" CodeBehind="AgencyTripConfigPriceList.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.AgencyTripConfigPriceList" %>

<%@ Register TagPrefix="svc" Namespace="CMS.ServerControls" Assembly="CMS.ServerControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadMain" runat="server">
    <title>Cấu hình giá theo cấp đại lý</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptManagerMain" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdminContentMain" runat="server">
    <h3 class="page-header">Cấu hình giá theo cấp đại lý</h3>
    <div class="quotation-table">
        <div class="row">
            <div class="col-xs-12">
                <a href="AgencyTripConfigPriceAdd.aspx?NodeId=1&SectionId=15" class="btn btn-primary">Tạo giá mới</a>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-12">
                <table class="table table-bordered table-hover table-common" style="text-align: center">
                    <tr class="active">
                        <th>Cấp đại lý</th>
                        <th>From</th>
                        <th>To</th>
                       <%-- <th>Created by</th>--%>
                        <%--<th>Export</th>--%>
                    </tr>
                    <asp:Repeater runat="server" ID="rptTripPrice">
                        <ItemTemplate>
                            <tr>
                                <td><a href="AgencyTripConfigPriceAdd.aspx?NodeId=1&SectionId=15&tid=<%# Eval("Id") %>"><%# Eval("AgentLevel.Name") %></a></td>
                                <td><%# DataBinder.Eval(Container.DataItem,"FromDate","{0:dd/MM/yyyy}") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem,"ToDate","{0:dd/MM/yyyy}") %>
                                </td>
                               <%-- <td><%#Eval("CreatedBy.UserName") %></td>--%>
                                <%--                                <td><asp:LinkButton runat="server" Text="Export" CommandArgument='<%#Eval("Id") %>' CommandName="export"></asp:LinkButton></td>--%>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </div>
        <div class="pager">
            <svc:Pager ID="pagerProduct" runat="server" PagerLinkMode="HyperLinkQueryString"
                HideWhenOnePage="false" ControlToPage="rptTripPrice"></svc:Pager>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptsMain" runat="server">
</asp:Content>
