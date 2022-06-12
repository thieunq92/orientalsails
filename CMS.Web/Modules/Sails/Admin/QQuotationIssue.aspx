<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="NewPopup.Master" CodeBehind="QQuotationIssue.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.QQuotationIssue" %>

<%@ Register TagPrefix="ucQuotationSelect" TagName="agent" Src="../Controls/QuotationIssueSelect.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="AdminContent" runat="server">
    <script>
        function RefreshParentPage() {
            window.parent.closePoup(1);
        }
        function closePoup() {
            window.parent.closePoup(0);
        }
    </script>
    <asp:UpdatePanel ID="upPanael" runat="server">
        <ContentTemplate>
            <div class="form-group">
                <div class="row">
                    <div class="col-xs-2 --no-padding-right">
                        <asp:Button runat="server" ID="btnAddQuotation" CssClass="btn btn-primary" Text="Add" OnClick="btnAddQuotation_OnClick"></asp:Button>
                    </div>
                </div>
            </div>
            <asp:Repeater runat="server" ID="rptQuotationSelect" OnItemDataBound="rptQuotationSelect_OnItemDataBound">
                <ItemTemplate>
                    <ucQuotationSelect:agent ID="ucQuotationSelect" runat="server" />
                </ItemTemplate>
            </asp:Repeater>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="form-group">
        <div class="row">
            <div class="col-xs-2 --no-padding-right">
                <button type="button" class="btn btn-default" data-dismiss="modal" onclick="RefreshParentPage();">Close</button>
                <asp:Button runat="server" ID="btnIssue" CssClass="btn btn-primary" Text="Issue" OnClick="btnIssue_Click"></asp:Button>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script>
        function selectQuotation(doc) {
            var clientId = doc.id.replace('nameid','');
            var width = 800;
            var height = 600;
            window.open('/Modules/Sails/Admin/QQuotationSelectorPage.aspx?NodeId=1&SectionId=15&clientid=' + clientId, 'Quotationselect', 'width=' + width + ',height=' + height + ',top=' + ((screen.height / 2) - (height / 2)) + ',left=' + ((screen.width / 2) - (width / 2)));
        };
    </script>
</asp:Content>
