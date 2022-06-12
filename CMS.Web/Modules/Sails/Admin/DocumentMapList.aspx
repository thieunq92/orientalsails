<%@ Page Title="" Language="C#" MasterPageFile="NewPopup.Master" AutoEventWireup="true" CodeBehind="DocumentMapList.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.DocumentMapList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="row">

        <div class="col-xs-12">
            <div class="form-group">
                <div class="row">
                    <div class="col-xs-12">
                        <input type="button" class="btn btn-primary" onclick="popupDocument(0);return false;" value="Thêm mới document"/>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="row">
                    <table class="document-table table table-bordered table-hover">
                        <tbody>
                            <tr class="active">
                                <th>Document Name</th>
                                <th>View</th>
                                <th>Download</th>
                                <th></th>

                            </tr>
                            <asp:Repeater ID="rptDocument" runat="server" OnItemDataBound="rptDocument_OnItemDataBound">
                                <ItemTemplate>
                                    <tr>
                                        <td><%#Eval("Name") %></td>
                                        <td>
                                            <asp:HyperLink ID="hplView" Target="_blank" runat="server"><i class="fas fa-eye"></i></asp:HyperLink>

                                        </td>
                                        <td>
                                            <asp:HyperLink ID="hplDownload" Target="_blank" runat="server"><i class="fas fa-download"></i></asp:HyperLink></td>
                                        <td>
                                            <a href="javascript:;" onclick='popupDocument(<%#Eval("Id") %>)'><i class="fa fa-edit fa-lg"></i></a>

                                        </td>

                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <div id="osModal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="gridSystemModalLabel" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" role="document" style="width: 850px">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close" onclick="closePoup();"><span aria-hidden="true">&times;</span></button>
                    <h3 class="modal-title">Document</h3>
                </div>
                <div class="modal-body">
                    <iframe frameborder="0" width="800" scrolling="no" onload="resizeIframe(this)"></iframe>
                </div>
            </div>
        </div>
    </div>
    <script>
        function popupDocument(id) {
            var query = "";
            if (id > 0) {
                query = "&docid=" + id;
            }
            $("#osModal iframe").attr('src', '/Modules/Sails/Admin/DocumentEdit.aspx?NodeId=1&SectionId=15&type=' + '<%=Request["type"]%>' + '&ObjId=' + '<%=Request["id"]%>'  +query);
            $("#osModal").modal();

        }
        function closePoup() {
            $('#osModal').modal('toggle');
            window.location.href = window.location.href;
        }
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Scripts" runat="server">
</asp:Content>
