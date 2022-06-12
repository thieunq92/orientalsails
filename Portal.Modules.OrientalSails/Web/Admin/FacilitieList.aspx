<%@ Page Title="" Language="C#" MasterPageFile="MO-Main.Master" AutoEventWireup="true" CodeBehind="FacilitieList.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.FacilitieList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadMain" runat="server">
    <title>Faciliti Management</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptManagerMain" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdminContentMain" runat="server">
    <h3 class="page-header">Faciliti Management</h3>
    <div class="search-panel">
        <div class="form-group">
            <div class="row">
                <div class="col-xs-12">
                    <asp:Button runat="server" ID="btnAddNew" CssClass="btn btn-primary" OnClientClick="return addFacilitie();"
                        Text="Add facilitie"></asp:Button>
                </div>
            </div>
        </div>
        <div class="form-group row">
            <asp:Repeater runat="server" ID="rptFacilitie">
                <ItemTemplate>
                    <div class="col-xs-6 facili-item">
                        <a href="javascript:;" onclick='editfacilitie("<%#Eval("Id") %>")'><%#Eval("Name") %></a>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

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
    <style>
        .facili-item {
            border: 1px solid #ddd;
            padding: 10px;
        }
        .facili-item:hover {
            background-color: #f5f5f5;
        }
    </style>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptsMain" runat="server">
    <script>
        function addFacilitie() {
            var src = "/Modules/Sails/Admin/FacilitieAdd.aspx?NodeId=1&SectionId=15";
            $("#osModal iframe").attr('src', src);
            $("#osModal").modal();
            return false;
        }
        function editfacilitie(id) {
            var src = "/Modules/Sails/Admin/FacilitieAdd.aspx?NodeId=1&SectionId=15&id=" + id;
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
