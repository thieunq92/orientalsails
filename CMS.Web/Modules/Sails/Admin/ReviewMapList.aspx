<%@ Page Title="" Language="C#" MasterPageFile="NewPopup.Master" AutoEventWireup="true" CodeBehind="ReviewMapList.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.ReviewMapList" %>

<asp:Content ID="Content3" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="search-panel">
        <div class="form-group">
            <div class="row">
                <div class="col-xs-12">
                    <asp:Button runat="server" ID="btnAddNew" CssClass="btn btn-primary" OnClientClick="return editReview();"
                        Text="Add review"></asp:Button>
                </div>
            </div>
        </div>
        <div class="form-group row">
            <table class="table table-bordered table-hover table-common">
                <tr class="active">
                    <th>Name</th>
                    <th>Review</th>
                    <th></th>
                </tr>
                <asp:Repeater ID="rptReviews" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td><%# DataBinder.Eval(Container.DataItem, "FullName") %>
                            </td>
                            <td><%# DataBinder.Eval(Container.DataItem, "Body") %></td>
                            <td>
                                <a href="javascript:;" onclick='editReview(<%#Eval("Id") %>)'><i class="fa fa-edit fa-lg"></i></a>
                            </td>

                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
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

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Scripts" runat="server">
    <script>
        function editReview(id) {
            var query = "";
            if (id > 0) {
                query = "&rid=" + id;
            }
            $("#osModal iframe").attr('src', '/Modules/Sails/Admin/ReviewMapEdit.aspx?NodeId=1&SectionId=15&type=' + '<%=Request["type"]%>' + '&ObjId=' + '<%=Request["id"]%>' + query);
            $("#osModal").modal();
            return false;
        }
        function closePoup() {
            $('#osModal').modal('toggle');
            window.location.href = window.location.href;
        }
    </script>
</asp:Content>
