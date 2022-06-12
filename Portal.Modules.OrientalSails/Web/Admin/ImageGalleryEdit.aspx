<%@ Page Title="" Language="C#" MasterPageFile="NewPopup.Master" AutoEventWireup="true" CodeBehind="ImageGalleryEdit.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.ImageGalleryEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="col-xs-3">
            <asp:Image runat="server" Width="60px" ID="imgGallery"/>
    </div>
    <div class="col-xs-9">
        <div class="form-group">
            <div class="row">
                <div class="col-xs-2">
                    Name
                </div>
                <div class="col-xs-5">
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Name"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName" ErrorMessage="***"></asp:RequiredFieldValidator>
                </div>
            </div>
        </div>
        <div class="form-group" runat="server" id="divCategory">
            <div class="row">
                <div class="col-xs-2">
                    Image
                </div>
                <div class="col-xs-5">
                    <asp:FileUpload runat="server" ID="imgUpload"/>
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="row">
                <div class="col-xs-7">
                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" ValidationGroup="service"
                        CssClass="btn btn-primary" />
                    <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click"
                        CssClass="btn btn-primary" OnClientClick="return confirm('Are you sure ?')" />
                    <asp:Button ID="btnClose" runat="server" Text="Đóng" OnClientClick="closePoup();return false;" CssClass="btn btn-default" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script>
        function closePoup() {
            window.parent.closePoup();
        }
    </script>
</asp:Content>
