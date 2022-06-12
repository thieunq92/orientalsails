<%@ Page Title="" Language="C#" MasterPageFile="NewPopup.Master" AutoEventWireup="true" CodeBehind="ReviewMapEdit.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.ReviewMapEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="col-xs-12">
        <div class="form-group">
            <div class="row">
                <div class="col-xs-2">
                    Full Name
                </div>
                <div class="col-xs-10">
                    <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" placeholder="Full Name"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFullName" ErrorMessage="***"></asp:RequiredFieldValidator>
                </div>
            </div>
        </div>
        <div class="form-group" runat="server" id="divCategory">
            <div class="row">
                <div class="col-xs-2">
                    Review
                </div>
                <div class="col-xs-10">
                    <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" CssClass="form-control" placeholder="Review"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtBody" ErrorMessage="***"></asp:RequiredFieldValidator>
                </div>
            </div>
        </div>

        <div class="form-group">
            <div class="row">
                <div class="col-xs-12">
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
