<%@ Page Language="C#" MasterPageFile="MO.Master" AutoEventWireup="true" CodeBehind="RoomList.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.RoomList" Title="Room Management" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="page-header">
        <h3>Room management</h3>
    </div>
    <div class="form-group">
        <div class="row">
            <div class="col-xs-1">
                Start date
            </div>
            <div class="col-xs-2">
                <asp:TextBox ID="textBoxStartDate" runat="server" CssClass="form-control" data-control="datetimepicker" placeholder="Start date (dd/mm/yyyy)"></asp:TextBox>
            </div>
            <div class="col-xs-1">
                <asp:Button ID="buttonSearch" runat="server" OnClick="buttonSearch_Click" CssClass="btn btn-primary" />
            </div>
        </div>
    </div>
    <table class="table table-bordered table-hover table-common">
        <asp:Repeater ID="rptRoom" runat="server" OnItemDataBound="rptRoom_ItemDataBound" OnItemCommand="rptRoom_ItemCommand">
            <HeaderTemplate>
                <tr class="active">
                    <th>No
                    </th>
                    <th>Name
                    </th>
                    <th>Room Type
                    </th>
                    <th>Room Class
                    </th>
                    <th>Cruise
                    </th>
                    <th>Floor
                    </th>
                    <th>Facilitie</th>
                    <th>Document</th>
                    <th>Images</th>
                    <th>Reviews</th>
                    <th></th>
                    <th>Action
                    </th>
                </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="item">
                    <td><%#Container.ItemIndex + 1%></td>
                    <td>
                        <asp:HyperLink ID="hyperLink_Name" runat="server"></asp:HyperLink>
                    </td>
                    <td>
                        <asp:Label ID="label_RoomType" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="label_RoomClass" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="labelCruise" runat="server"></asp:Label>
                    </td>
                    <td><%#Eval("Floor") %></td>
                    <td>
                        <a href="javascript:;" onclick='editfacilitie("<%#Eval("Id") %>")'>Facilitie</a>
                    </td>
                    <td>
                        <a href="javascript:;" onclick='editDocument("<%#Eval("Id") %>")'>Document</a>
                    </td>
                    <td>
                        <a href="javascript:;" onclick='editImages("<%#Eval("Id") %>")'>Images</a>
                    </td>
                    <td>
                        <a href="javascript:;" onclick='editReviews("<%#Eval("Id") %>")'>Reviews</a>
                    </td>
                    <td id="tdAvailable" runat="server"></td>
                    <td>
                        <asp:HyperLink ID="hyperLinkEdit" runat="server">
                                         <i class="fa fa-edit fa-lg" aria-hidden="true" data-toggle="tooltip" data-placement="top" title="" data-original-title="Edit"></i>
                        </asp:HyperLink>
                        <asp:LinkButton runat="server" CommandName="Delete"
                            CommandArgument='<%# DataBinder.Eval(Container.DataItem,"Id") %>' OnClientClick="return confirm('Are you sure?')">
                                            <i class="fa fa-window-close fa-lg text-danger" aria-hidden="true" title="" data-toggle="tooltip" data-placement="top" data-original-title="Delete"></i>
                        </asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
    <div class="modal fade" id="osModal" tabindex="-1" role="dialog" aria-labelledby="gridSystemModalLabel" data-backdrop="static" data-keyboard="true">
        <div class="modal-dialog" role="document" style="width: 85vw; height: 80vh">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <iframe frameborder="0" width="100%" style="height: 80vh" scrolling="no"></iframe>
                </div>
            </div>
        </div>
    </div>
    <script>
        function editfacilitie(id) {
            var src = "/Modules/Sails/Admin/FacilitieMapList.aspx?NodeId=1&SectionId=15&type=CABIN&id=" + id;
            $("#osModal iframe").attr('src', src);
            $("#osModal").modal();
            return false;
        }
        function editDocument(id) {
            var src = "/Modules/Sails/Admin/DocumentMapList.aspx?NodeId=1&SectionId=15&type=CABIN&id=" + id;
            $("#osModal iframe").attr('src', src);
            $("#osModal").modal();
            return false;
        }
        function editImages(id) {
            var src = "/Modules/Sails/Admin/ImageMapList.aspx?NodeId=1&SectionId=15&type=CABIN&id=" + id;
            $("#osModal iframe").attr('src', src);
            $("#osModal").modal();
            return false;
        }
        function editReviews(id) {
            var src = "/Modules/Sails/Admin/ReviewMapList.aspx?NodeId=1&SectionId=15&type=CABIN&id=" + id;
            $("#osModal iframe").attr('src', src);
            $("#osModal").modal();
            return false;
        }
        function closePoup(refesh) {
            $("#osModal").modal('hide');
            if (refesh === 1) {
                window.location.href = window.location.href;
            }
        }
    </script>
</asp:Content>
