<%@ Page Language="C#" MasterPageFile="MO.Master" AutoEventWireup="true"
    CodeBehind="CruisesList.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.CruisesList"
    Title="Cruise Management" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="page-header">
        <h3>Cruise management</h3>
    </div>
    <div class="row">
        <div class="col-xs-12">
            <table class="table table-bordered table-hover table-common">
                <tr class="active">
                    <th>Cruise code</th>
                    <th>Name</th>
                    <th>Facilitie</th>
                    <th>Document</th>
                    <th>Images</th>
                    <th>Reviews</th>
                    <th></th>
                    <th></th>
                </tr>
                <asp:Repeater ID="rptCruises" runat="server" OnItemDataBound="rptCruises_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td><%# DataBinder.Eval(Container.DataItem, "Code") %>
                                <asp:HiddenField runat="server" ID="cid" Value='<%#Eval("Id") %>'/>
                            </td>
                            <td>
                                <asp:HyperLink ID="hplCruise" runat="server"></asp:HyperLink></td>
                            <td><%--<asp:DropDownList runat="server" CssClass="form-control" ID="ddlGroup"/>--%>
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
                            <td>
                                <asp:HyperLink ID="hplRoomClasses" runat="server">Room class</asp:HyperLink></td>
                            <td>
                                <asp:HyperLink ID="hplRooms" runat="server">Rooms</asp:HyperLink></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr>
                    <td></td>
                    <td></td>
                    <td><%--<asp:Button runat="server" CssClass="btn btn-primary" Text="Save" OnClick="OnClick"/>--%></td>
                    <td></td>
                    <td></td>
                </tr>
            </table>
        </div>
    </div>
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
            var src = "/Modules/Sails/Admin/FacilitieMapList.aspx?NodeId=1&SectionId=15&type=CRUISE&id=" + id;
            $("#osModal iframe").attr('src', src);
            $("#osModal").modal();
            return false;
        }
        function editDocument(id) {
            var src = "/Modules/Sails/Admin/DocumentMapList.aspx?NodeId=1&SectionId=15&type=CRUISE&id=" + id;
            $("#osModal iframe").attr('src', src);
            $("#osModal").modal();
            return false;
        }
        function editImages(id) {
            var src = "/Modules/Sails/Admin/ImageMapList.aspx?NodeId=1&SectionId=15&type=CRUISE&id=" + id;
            $("#osModal iframe").attr('src', src);
            $("#osModal").modal();
            return false;
        }
        function editReviews(id) {
            var src = "/Modules/Sails/Admin/ReviewMapList.aspx?NodeId=1&SectionId=15&type=CRUISE&id=" + id;
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
