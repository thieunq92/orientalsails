<%@ Page Title="" Language="C#" MasterPageFile="NewPopup.Master" AutoEventWireup="true" CodeBehind="ImageMapList.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.ImageMapList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="container">
        <div class="row">
            <div class="form-group">
                <div class="col-xs-12">
                    <div class="col-xs-5">
                        <asp:FileUpload ID="imgUpload" multiple="multiple" runat="server" />
                    </div>
                    <div class="col-xs-2">
                        <asp:Button ID="btnSaveFile" CssClass="btn btn-primary" Text="Save" runat="server" OnClick="btnSaveFile_Click" />

                    </div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-12">
                <table class="table table-bordered table-hover table-common">
                    <tr class="active">
                        <th>Name</th>
                        <th>Image</th>
                        <th></th>
                    </tr>
                    <asp:Repeater ID="rptImages" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%#Eval("Name") %></td>
                                <td>
                                    <img src='<%#Eval("ImageUrl") %>' style="width: 60px" class="img-fluid" alt="quixote">
                                </td>
                                <td>
                                    <a href="javascript:;" onclick='editImage(<%#Eval("Id") %>)'><i class="fa fa-edit fa-lg"></i></a>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </div>
    </div>
    
    <div id="osModal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="gridSystemModalLabel" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" role="document" style="width: 850px">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close" onclick="closePoup();"><span aria-hidden="true">&times;</span></button>
                    <%-- <h3 class="modal-title"></h3>--%>
                </div>
                <div class="modal-body">
                    <iframe frameborder="0" width="800" scrolling="no" onload="resizeIframe(this)"></iframe>
                </div>
            </div>
        </div>
    </div>
    <script>
        function editImage(id) {
            var query = "";
            if (id > 0) {
                query = "&imgId=" + id;
            }
            $("#osModal iframe").attr('src', '/Modules/Sails/Admin/ImageGalleryEdit.aspx?NodeId=1&SectionId=15&type=' + '<%=Request["type"]%>' + '&ObjId=' + '<%=Request["id"]%>' + query);
            $("#osModal").modal();

        }
        function closePoup() {
            $('#osModal').modal('toggle');
            window.location.href = window.location.href;
        }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
</asp:Content>
