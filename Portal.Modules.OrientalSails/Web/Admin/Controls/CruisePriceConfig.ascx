<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CruisePriceConfig.ascx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.Controls.CruisePriceConfigCtrl" %>
<div class="page-header">
    <h4>
        <asp:Literal runat="server" ID="litCruise"></asp:Literal></h4>
    <asp:HiddenField runat="server" ID="hidCruiseId" />
</div>
<div class="form-group">
    <div class="row">
        <table class="table table-bordered table-hover">
            <tr class="active">
                <th></th>
                <asp:Repeater runat="server" ID="rptRoomPriceHeader">
                    <ItemTemplate>
                        <th><%#Eval("RoomTypeName") %></th>
                    </ItemTemplate>
                </asp:Repeater>
            </tr>
            <asp:Repeater ID="rptRoomClass" runat="server" OnItemDataBound="rptRoomClass_OnItemDataBound">
                <ItemTemplate>
                    <tr class="item">
                        <td>
                            <%#Eval("Name") %>
                        </td>
                        <asp:Repeater runat="server" ID="rptRoomPrice" >
                            <ItemTemplate>
                                <td>
                                    <div class="input-group">
                                        <span class="input-group-addon">VND</span>
                                        <asp:TextBox runat="server" ID="txtPrice" CssClass="form-control" data-inputmask="'alias': 'numeric', 'groupSeparator': ',', 'autoGroup': true, 'digits': 2, 'digitsOptional': true, 'placeholder': '0', 'rightAlign':false" Text='<%#Eval("Price") %>'></asp:TextBox>
                                    </div>
                                    <asp:HiddenField runat="server" ID="hidRoomTypeId" Value='<%#Eval("RoomTypeId") %>' />
                                    <asp:HiddenField runat="server" ID="hidRoomClassId" Value='<%#Eval("RoomClassId") %>' />
                                    <asp:HiddenField runat="server" ID="hidRomPriceId" Value='<%#Eval("Id") %>' />
                                </td>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </div>
    <div class="row">
        <asp:UpdatePanel ID="upPanel" runat="server">
            <ContentTemplate>
                <div class="col-xs-12">
                    <div class="col-xs-1">
                        <h4><strong>Charter</strong></h4>
                    </div>
                    <div class="col-xs-2">
                        <asp:Button runat="server" Style="float: left" ID="btnAddRange" CssClass="btn btn-primary" Text="Thêm số khách" OnClick="btnAddRange_OnClick" />
                    </div>
                </div>
                <table class="table table-bordered table-hover" style="text-align: center; border-top: none">
                    <tr>
                        <td>Số khách từ</td>
                        <td>Số khách đến</td>
                        <td>Giá</td>
                        <td>Xóa</td>
                    </tr>
                    <asp:Repeater ID="rptCharterRanger" OnItemDataBound="rptCharterRanger_OnItemDataBound" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:HiddenField runat="server" ID="hidCharterPriceId" Value='<%#Eval("Id") %>' />
                                    <asp:TextBox runat="server" ID="txtCusFrom" Text='<%#Eval("CusFrom") %>' CssClass="form-control" placeholder="Từ"></asp:TextBox></td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtCusTo" Text='<%#Eval("CusTo") %>' CssClass="form-control" placeholder="Đến"></asp:TextBox></td>
                                <td>
                                    <div class="input-group">
                                        <span class="input-group-addon">VND</span>
                                        <asp:TextBox runat="server" ID="txtPrice" CssClass="form-control" data-inputmask="'alias': 'numeric', 'groupSeparator': ',', 'autoGroup': true, 'digits': 2, 'digitsOptional': true, 'placeholder': '0', 'rightAlign':false" Text="0"></asp:TextBox>
                                    </div>
                                </td>
                                <td>
                                    <asp:CheckBox runat="server" ID="checkIsDelete" />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
