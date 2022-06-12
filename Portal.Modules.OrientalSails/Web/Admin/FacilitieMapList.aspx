<%@ Page Title="" Language="C#" MasterPageFile="NewPopup.Master" AutoEventWireup="true" CodeBehind="FacilitieMapList.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.FacilitieMapList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="AdminContent" runat="server">
    <script>
        function RefreshParentPage() {
            window.parent.closePoup(0);
        }
        function closePoup() {
            window.parent.closePoup(0);
        }
    </script>
    <div class="form-group">
        <div class="row">
            <div class="col-xs-12">
                <asp:Button runat="server" ID="btnSave" CssClass="btn btn-primary"
                    Text="Save facilitie" OnClick="btnSave_OnClick"></asp:Button>
            </div>
        </div>
    </div>
    <div class="form-group row">
        <asp:Repeater runat="server" ID="rptFacilitie" OnItemDataBound="rptFacilitie_OnItemDataBound">
            <ItemTemplate>
                <div class="col-xs-6 facili-item">
                    <div class="col-xs-10">
                        <%#Eval("Name") %>
                        <asp:HiddenField runat="server" ID="facilitiId" Value='<%#Eval("Id") %>' />
                        <asp:HiddenField runat="server" ID="mapId" />
                    </div>
                    <div class="col-xs-2">
                        <asp:CheckBox runat="server" ID="chkMap" />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
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
<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
</asp:Content>
