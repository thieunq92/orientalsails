<%@ Page Language="C#" MasterPageFile="MO.Master" AutoEventWireup="true"
    CodeBehind="BookingView.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.BookingView"
    Title="Booking View" %>

<%@ Register Assembly="CMS.ServerControls" Namespace="CMS.ServerControls" TagPrefix="svc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register TagPrefix="uc" TagName="customer" Src="../Controls/CustomerInfoRowInput.ascx" %>
<%@ Register TagPrefix="ucextra" TagName="customer" Src="../Controls/CustomerExtraInfoRowInput.ascx" %>
<%@ Register Assembly="Portal.Modules.OrientalSails" Namespace="Portal.Modules.OrientalSails.Web.Controls"
    TagPrefix="orc" %>
<%@ Import Namespace="Portal.Modules.OrientalSails.Web.Admin.Utilities" %>
<%@ Import Namespace="Portal.Modules.OrientalSails.Web.Admin.Enums" %>
<asp:Content ID="AdminContent" ContentPlaceHolderID="AdminContent" runat="server">
    <div ng-controller="bookingViewController" runat="server" id="bookingViewController">
        <div class="row">
            <div class="col-md-12" runat="server" visible='<%# UserIdentity.UserName != "captain.os1" %>'>
                <button id="btnSave" type="button" class="btn btn-primary" data-uniqueid="<%= btnSave.UniqueID %>" style="margin-right: 0px">Save</button>
                <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary hidden" OnClick="buttonSubmit_Click" />
                <a href="SendEmail.aspx?NodeId=1&SectionId=15&BookingId=<%= Booking.Id %>" class="btn btn-primary" id="sendemail">SendEmail</a>
                <a href="BookingHistories.aspx?NodeId=1&SectionId=15&BookingId=<%= Booking.Id %>" class="btn btn-primary">View History</a>
            </div>
        </div>
        <div class="bookinginfor-panel">
            <div class="form-group">
                <div class="row">
                    <div class="col-xs-1">
                        Booking code        
                    </div>
                    <div class="col-xs-1">
                        <asp:Label ID="lblBookingId" runat="server"></asp:Label>
                    </div>
                    <div class="col-xs-1">
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="chkInspection" runat="server">Inspection
                       
                            </label>
                        </div>
                    </div>
                    <div class="col-xs-1">
                        <asp:UpdatePanel runat="server">
                            <ContentTemplate>
                                <div class="checkbox">
                                    <label>
                                        <input type="checkbox" id="chkCharter" runat="server" onserverchange="chkCharter_OnCheckedChanged" onclick="submit()">Charter
                               
                                    </label>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col-xs-1 --no-padding-left">
                        Trip          
                    </div>
                    <div class="col-xs-2">
                        <asp:PlaceHolder runat="server" ID="plhTripReadonly" Visible="false">
                            <asp:Literal runat="server" ID="litTrip"></asp:Literal>
                            (contact accountant to change) </asp:PlaceHolder>
                        <asp:DropDownList ID="ddlTrips" Enabled="False" runat="server" CssClass="form-control">
                        </asp:DropDownList>
                        <asp:DropDownList ID="ddlOptions" runat="server" CssClass="form-control">
                            <asp:ListItem>Option 1</asp:ListItem>
                            <asp:ListItem>Option 2</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-xs-1 --no-padding-left" runat="server" visible='<%# UserIdentity.UserName != "captain.os1" %>'>
                        <button class="btn btn-success" onclick="return changeTrip()">Change trip</button>
                    </div>
                    <div class="col-xs-1 --text-right">
                        Cruise      
                    </div>
                    <div class="col-xs-2 --no-padding-left">
                        <asp:DropDownList ID="ddlCruises" Enabled="False" runat="server" CssClass="form-control">
                        </asp:DropDownList>
                    </div>
                    <div class="col-xs-1 --no-padding-left" runat="server" visible='<%# UserIdentity.UserName != "captain.os1" %>'>
                        <button class="btn btn-success" onclick="return changeBoat()">Change boat</button>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="row">
                    <div class="col-xs-1">
                        <label for="startdate">Start Date</label>
                    </div>
                    <div class="col-xs-3">
                        <asp:TextBox ID="txtStartDate" CssClass="form-control" Enabled="False" placeholder="Start Date (dd/mm/yyyy)" runat="server"></asp:TextBox>
                    </div>
                    <div class="col-xs-1 --no-padding-left">
                        Status
               
                    </div>
                    <div class="col-xs-2">
                        <asp:DropDownList ID="ddlStatusType" runat="server" CssClass="form-control">
                        </asp:DropDownList>
                    </div>
                    <div class="col-xs-2 nopadding-left">
                        <div id="statusinfor-placeholder">
                            <asp:TextBox ID="txtDeadline" runat="server" placeholder="Deadline Pending" CssClass="form-control"></asp:TextBox>
                            <asp:TextBox ID="txtCutOffDays" runat="server" placeholder="CutOff Days" CssClass="form-control"></asp:TextBox>
                            <asp:TextBox runat="server" ID="txtCancelledReason" TextMode="MultiLine" placeholder="Lý do hủy booking" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-xs-1 nopadding-right --no-padding-left">
                        Number Of Pax
               
                    </div>
                    <div class="col-xs-1 nopadding-left nopadding-right" style="width: 3%">
                        <asp:Literal ID="litPax" runat="server"></asp:Literal>
                        <i class="fa fa-info-circle fa-lg" aria-hidden="true" data-toggle="tooltip" data-placement="right" title="<%= PaxGetDetails() %>"></i>
                    </div>
                    <asp:PlaceHolder ID="plhNumberOfCabin" runat="server">
                        <div class="col-xs-1 nopadding-left nopadding-right">
                            Number Of Cabin           
                        </div>
                        <div class="col-xs-1 nopadding-left nopadding-right" style="width: 3%">
                            <asp:Literal ID="litCabins" runat="server"></asp:Literal>
                            <i class="fa fa-info-circle fa-lg" aria-hidden="true" data-toggle="tooltip" data-placement="right auto" title="<%= CabinGetDetails() %>"></i>
                        </div>
                    </asp:PlaceHolder>
                </div>
            </div>
            <div class="form-group">
                <div class="row">
                    <div class="col-xs-1">
                        <label for="agency">Agency</label>
                    </div>
                    <div class="col-xs-3">
                        <asp:DropDownList ID="ddlAgencies" CssClass="form-control" runat="server">
                        </asp:DropDownList>
                    </div>
                    <div class="col-xs-2 nopadding-left" style="padding-right: 8px">
                        <asp:TextBox ID="txtAgencyCode" CssClass="form-control" placeholder="TA Code" runat="server" data-toggle="tooltip" title="TA code" data-placement="top"></asp:TextBox>
                    </div>
                    <div class="col-xs-2 nopadding-left" style="padding-right: 8px">
                        <asp:TextBox runat="server" ID="txtSeriesCode" CssClass="form-control" placeholder="Series code" data-toggle="tooltip" title="Series code" data-placement="top"></asp:TextBox>
                    </div>
                    <div class="col-xs-2 --no-padding-left">
                        <svc:CascadingDropDown ID="cddlBooker" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                        </svc:CascadingDropDown>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="row">
                    <div class="col-xs-1">
                        <label for="total">Total</label>
                    </div>
                    <div class="col-xs-2" style="padding-right: 8px">
                        <asp:TextBox ID="txtTotal" runat="server" placeholder="Total" CssClass="form-control" data-inputmask="'alias': 'numeric', 'groupSeparator': ',', 'autoGroup': true, 'digits': 2, 'digitsOptional': true, 'placeholder': '0'" ng-model="actuallyCollected"></asp:TextBox>
                    </div>
                    <div class="col-xs-1 nopadding-left">
                        <asp:DropDownList runat="server" ID="ddlCurrencies" CssClass="form-control">
                            <asp:ListItem Value="1">USD</asp:ListItem>
                            <asp:ListItem Value="0">VND</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-xs-2 nopadding-left --width-auto" runat="server" visible='<%# UserIdentity.UserName != "captain.os1" %>'>
                        <asp:Button ID="lbtCalculate" CssClass="btn btn-primary" runat="server" OnClick="lbtCalculate_Click" Text="Calculate"
                            Style="width: auto"></asp:Button>
                        <asp:Button runat="server" ID="btnLockIncome" CssClass="btn btn-primary" Visible="false" Text="Lock this booking"
                            OnClick="btnLockIncome_Click" />
                        <asp:Button runat="server" ID="btnUnlockIncome" Visible="false" CssClass="btn btn-primary"
                            Text="Unlock" OnClick="btnUnlockIncome_Click" />
                        <i class="fa fa-info-circle fa-lg" aria-hidden="true" data-toggle="tooltip" data-placement="right" title="<%= UserGetUserLockIncomeDetails() %>"></i>
                    </div>
                    <div class="col-xs-5 nopadding-left">
                        <div class="checkbox">
                            <label class="checkbox-inline">
                                <input runat="server" id="chkSpecial" type="checkbox" />Upgrade/Special price</label>
                            <label class="checkbox-inline">
                                <input type="checkbox" runat="server" id="chkInvoice">VAT</label>
                            <label class="checkbox-inline">
                                <input id="chkIsPaymentNeeded" runat="server" type="checkbox" />
                                Pay Before Tour
                       
                            </label>
                            <label class="checkbox-inline">
                                <input runat="server" id="chkEarlyBird" type="checkbox" />Early Bird                
                            </label>

                        </div>
                    </div>
                </div>
            </div>
            <div class="form-group" style="display: none">
                <div class="row">
                    <div class="col-xs-1">
                        <label for="commission">Commission</label>
                    </div>
                    <div class="col-xs-2">
                        <asp:TextBox ID="txtCommission" runat="server" placeholder="Commission" CssClass="form-control "></asp:TextBox>
                    </div>
                    <div class="col-xs-1 nopadding-left">
                        <asp:DropDownList runat="server" ID="ddlCommissionCurrencies" CssClass="form-control">
                            <asp:ListItem Value="1">USD</asp:ListItem>
                            <asp:ListItem Value="0">VND</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                </div>
            </div>
            <div class="form-group" style="display: none">
                <div class="row">
                    <div class="col-xs-1 nopadding-right">
                        <label for="cancelpenalty">Cancel Penalty</label>
                    </div>
                    <div class="col-xs-2">
                        <asp:TextBox ID="txtPenalty" runat="server" Text="0" CssClass="form-control" placeholder="Cancel Penalty"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="row">
                    <div class="col-xs-1 --no-padding-right">
                        Voucher Code           
                    </div>
                    <div class="col-xs-4 nopadding-right">
                        <div class="input-group" style="width: 97%">
                            <asp:TextBox ID="txtAllVoucher" placeholder="Voucher code" runat="server" CssClass="form-control"></asp:TextBox>
                            <span class="input-group-btn" runat="server" visible='<%# UserIdentity.UserName != "captain.os1" %>'>
                                <input type="button" class="btn btn-primary" value="Check Code" id="checkvoucher" style="height: 25px" />
                            </span>
                        </div>
                    </div>
                    <div class="col-xs-7 --no-padding-left">
                        <p>
                            <asp:Literal runat="server" ID="litCreated"></asp:Literal>
                        </p>
                    </div>
                </div>
            </div>
            <div class="form-group" id="extra-service">
                <div class="row">
                    <div class="col-xs-1 --no-padding-right">
                        <label for="extraservices">Extra Services</label>
                    </div>
                    <div class="col-xs-1" id="detail-service">
                        <asp:PlaceHolder ID="plhDetailService" runat="server">
                            <asp:Repeater ID="rptExtraServices" runat="server" OnItemDataBound="rptExtraServices_ItemDataBound">
                                <ItemTemplate>
                                    <div class="checkbox">
                                        <asp:HiddenField ID="hiddenId" runat="server" Value='<%#Eval("Id") %>' />
                                        <label>
                                            <input id="chkService" runat="server" type="checkbox" ng-model='<%# Eval("Name")%>' /><%#Eval("Name") %></label>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </asp:PlaceHolder>
                        <asp:DropDownList ID="ddlTransferLocation" runat="server" CssClass="form-control" Visible="<%# IsSeatingCruise %>" DataSource='<%# Enumeration.GetAll<TransferLocationType>() %>' DataTextField="Value" DataValueField="Key" AppendDataBoundItems="true" Style="margin-top: 10px" ng-class="{'ng-hide':Transfers == false}"></asp:DropDownList>
                    </div>
                    <div class="col-xs-10" id="transfer-service-details" ng-class="{'ng-hide':Transfers == false}">
                        <div class="row">
                            <div class="col-xs-1 nopadding-right --width-auto">
                                Bus type                   
                            </div>
                            <div class="col-xs-2" style="margin-top: -8px; width: 10%">
                                <asp:Repeater ID="rptBusType" runat="server">
                                    <ItemTemplate>
                                        <div class="radio">
                                            <label>
                                                <input type="radio" name="radBusType" value='<%# Eval("Id") %>'
                                                    <%# Booking.Transfer_BusType == null ? (Container.ItemIndex == 0 ? "checked='checked'" : "") 
                                            :(Booking.Transfer_BusType.Id == (int)Eval("Id")?"checked = 'checked'" : "")
                                                %>>
                                                <%# Eval("Name") %>
                                            </label>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                            <div class="col-xs-2 nopadding-right" style="width: 6%">
                                Service
                       
                            </div>
                            <div class="col-xs-2 --width-auto" style="margin-top: -8px">
                                <div class="radio">
                                    <label>
                                        <asp:RadioButton runat="server" GroupName="transferService" ID="rbtTransferService_TwoWay" Text="Two ways"
                                            Checked='<%# !String.IsNullOrEmpty(Booking.Transfer_Service) ? (Booking.Transfer_Service == "Two Way" ? true : false) : true%>' />
                                    </label>
                                </div>
                                <div class="radio">
                                    <label>
                                        <asp:RadioButton runat="server" GroupName="transferService" ID="rbtTransferService_OneWay" Text="One way"
                                            Checked='<%# Booking.Transfer_Service == "One Way" ? true : false %>' />
                                    </label>
                                </div>
                            </div>
                            <div class="col-xs-3 --width-auto">
                                <div class="form-group">
                                    <div class="row">
                                        <div class="col-xs-4">
                                            Date to
                                   
                                        </div>
                                        <div class="col-xs-8">
                                            <asp:TextBox ID="txtTransfer_Dateto" CssClass="form-control"
                                                runat="server" placeholder="Date to(dd/mm/yyyy)" data-control="datetimepicker"
                                                ng-model="transfer_DateTo" ng-change="transferDateToChangedHandler()"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="row">
                                        <div class="col-xs-4">
                                            Date back
                                   
                                        </div>
                                        <div class="col-xs-8">
                                            <asp:TextBox ID="txtTransfer_Dateback" CssClass="form-control"
                                                runat="server" placeholder="Date back(dd/mm/yyyy)" data-control="datetimepicker"
                                                ng-model="transfer_DateBack" ng-change="transferDateBackChangedHandler()"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-xs-1 nopadding-right" style="width: 10%">
                                Special Request Transfer
                       
                            </div>
                            <div class="col-xs-5 --no-padding-right" style="width: 32.8%">
                                <asp:TextBox ID="txtTransfer_Note" CssClass="form-control" runat="server" placeholder="Special Request Transfer"
                                    TextMode="MultiLine"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="row">
                    <div class="col-xs-12">
                        <asp:Literal runat="server" ID="litInform" Visible="true" />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-3">
                    <label for="pickupaddress">
                        Pickup Address        
                    </label>
                </div>
                <div class="col-xs-1">
                    <label for="pickupaddress">
                        Pickup time
                    </label>
                </div>
                <div class="col-xs-4 --no-padding-left" runat="server" visible='<%# CanViewSpecialRequestFood %>'>
                    Special Request Food
                </div>
                <div class="col-xs-4 --no-padding-left" runat="server" visible="<%# CanViewSpecialRequestRoom && !IsSeatingCruise %>">
                    Special Request Room
                </div>
                <div class="col-xs-4 --no-padding-left" style="display: none">
                    Customer Info              
                </div>
            </div>
            <div class="row">
                <div class="col-xs-3">
                    <asp:TextBox ID="txtPickup" runat="server" CssClass="form-control" TextMode="MultiLine" placeholder="Pickup Address"></asp:TextBox>
                </div>
                <div class="col-xs-1">
                    <asp:TextBox ID="txtPickupTime" runat="server" CssClass="form-control" autocomplete="off" placeholder="(hh:mm)" data-control="timepicker"></asp:TextBox>
                </div>
                <div class="col-xs-4 --no-padding-left" runat="server" visible="<%# CanViewSpecialRequestFood %>">
                    <asp:TextBox ID="txtSpecialRequest" runat="server" CssClass="form-control" TextMode="MultiLine" placeholder="Special Request Food" Style="display: inline; width: 70%; float: left"></asp:TextBox>
                    <label class="checkbox-inline" style="float: left; margin-left: 10px">
                        <input runat="server" id="chkAnChay" type="checkbox" />Ăn Chay                       
                    </label>
                </div>
                <div class="col-xs-4 --no-padding-left" runat="server" visible="<%# CanViewSpecialRequestRoom && !IsSeatingCruise %>">
                    <asp:TextBox ID="txtSpecialRequestRoom" runat="server" CssClass="form-control" TextMode="MultiLine" placeholder="Special Request Room"></asp:TextBox>
                </div>
                <div class="col-xs-4 --no-padding-left">
                    <asp:TextBox ID="txtCustomerInfo" runat="server" CssClass="form-control" TextMode="MultiLine" placeholder="Customer Info" Visible="false"></asp:TextBox>
                </div>
            </div>
            <br />
            <div class="row" ng-init="loadCommission()">
                <div class="col-xs-12">
                    <h3>Trích ngoài
                <button type="button" class="btn btn-primary" ng-click="addCommission()">Thêm</button></h3>
                    <br />
                    <div class="form-group" ng-show="listCommission.length > 0">
                        <div class="row">
                            <div class="col-xs-2">
                                Số phiếu chi
                            </div>
                            <div class="col-xs-2">
                                Trả cho
                            </div>
                            <div class="col-xs-2">
                                Số tiền
                            </div>
                        </div>
                    </div>
                    <div class="form-group" ng-repeat="item in listCommission">
                        <div class="row">
                            <input type="hidden" ng-model="item.id">
                            <div class="col-xs-2">
                                <input type="text" class="form-control" placeholder="Số phiếu chi" data-inputmask="'alias': 'integer','allowMinus':false,'groupSeparator': ',', 'autoGroup': true,'rightAlign':false" ng-model="item.paymentVoucher">
                            </div>
                            <div class="col-xs-2 nopadding-right">
                                <input type="text" class="form-control" placeholder="Trả cho" ng-model="item.payFor" />
                            </div>
                            <div class="col-xs-2 nopadding-right">
                                <div class="input-group">
                                    <input type="text" class="form-control" placeholder="Số tiền" ng-model="item.amount" data-control="inputmask" ng-change="calculateTotalCommission() " />
                                    <span class="input-group-addon">VND</span>
                                </div>
                            </div>
                            <div class="col-xs-2">
                                <div class="checkbox" style="margin-top: 0px; margin-bottom: 0px">
                                    <label>
                                        <input type="checkbox" ng-model="item.transfer">Chuyển khoản
                                    </label>
                                </div>
                            </div>
                            <div class="col-xs-1 nopadding-right">
                                <button type="button" class="btn btn-primary" ng-click="removeCommission($index)">
                                    Xóa
                                </button>
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="row">
                            <div class="col-xs-offset-4 col-xs-8" ng-show="listCommission.length > 0">
                                Tổng : {{ calculateTotalCommission() }}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row" ng-init="loadServiceOutside()">
                <div class="col-xs-12">
                    <h3>Dịch vụ ngoài 
                <button type="button" class="btn btn-primary" ng-click="addServiceOutside()">Thêm</button></h3>
                    <br />
                    <div class="form-group" ng-show="listServiceOutside.length > 0">
                        <div class="row">
                            <div class="col-xs-2">
                                Trả cho
                            </div>
                            <div class="col-xs-2">
                                Đơn giá
                            </div>
                            <div class="col-xs-2" style="width: 12%">
                                Số lượng
                            </div>
                            <div class="col-xs-2">
                                Thành tiền
                            </div>
                        </div>
                    </div>
                    <div class="form-group" ng-repeat="item in listServiceOutside">
                        <div class="row">
                            <input type="hidden" ng-model="item.id">
                            <div class="col-xs-2 nopadding-right">
                                <input type="text" class="form-control" placeholder="Trả cho" ng-model="item.service" />
                            </div>
                            <div class="col-xs-2 nopadding-right">
                                <div class="input-group">
                                    <input type="text" class="form-control" placeholder="Đơn giá" ng-model="item.unitPrice" data-control="inputmask" data-inputmask="'alias': 'numeric', 'groupSeparator': ',', 'autoGroup': true, 'digits': 2, 'digitsOptional': true, 'rightAlign':false"
                                        ng-change="calculateServiceOutside($index, item.unitPrice, item.quantity)" />
                                    <span class="input-group-addon">VND</span>
                                </div>
                            </div>
                            <div class="col-xs-2 nopadding-right" style="width: 12%">
                                <input type="text" class="form-control" placeholder="Số lượng" ng-model="item.quantity" ng-change="calculateServiceOutside($index, item.unitPrice, item.quantity)" />
                            </div>
                            <div class="col-xs-2 nopadding-right">
                                <div class="input-group">
                                    <input type="text" class="form-control" placeholder="Thành tiền" ng-model="item.totalPrice" data-control="inputmask" data-inputmask="'alias': 'numeric', 'groupSeparator': ',', 'autoGroup': true, 'digits': 2, 'digitsOptional': true, 'rightAlign':false" ng-change="calculateTotalServiceOutside()" />
                                    <span class="input-group-addon">VND</span>
                                </div>
                            </div>
                            <div class="col-xs-1 nopadding-right">
                                <div class="checkbox" style="margin-top: 0px; margin-bottom: 0px">
                                    <label>
                                        <input type="checkbox" ng-model="item.vat">VAT
                                    </label>
                                </div>
                            </div>
                            <div class="col-xs-3 nopadding-right nopadding-left" style="width: 14%">
                                <button type="button" class="btn btn-primary" data-toggle="modal" data-target=".modal-serviceOutsideDetail{{item.id}}">Chi tiết</button>
                                <button type="button" class="btn btn-primary" ng-click="removeServiceOutside($index)">
                                    Xóa
                                </button>
                            </div>
                        </div>
                    </div>
                    <div class="form-group" ng-show="listServiceOutside.length > 0">
                        <div class="row">
                            <div class="col-xs-offset-7 col-xs-4" style="margin-left: 57.333%">
                                Tổng : {{ calculateTotalServiceOutside() }}
                            </div>
                        </div>
                    </div>

                </div>
            </div>
            <div class="row" ng-hide="!canViewTotalDetails">
                <div class="col-xs-12">
                    <%if (!IsSeatingCruise)
                        {
                    %>
                    <asp:Repeater ID="rptSalesPriceInput" runat="server" OnItemDataBound="rptSalesPriceInput_ItemDataBound">
                        <ItemTemplate>
                            <asp:HiddenField ID="hiddenRoomClassId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "RoomClassId") %>' />
                            <asp:HiddenField ID="hiddenRoomTypeId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "RoomTypeId") %>' />
                            <table class="table" style="display: inline-block; width: auto; border: 1px solid #ccc; padding: 10px; margin-right: 10px">
                                <thead>
                                    <tr>
                                        <th><%#DataBinder.Eval(Container.DataItem, "RoomClassName") %> <%#DataBinder.Eval(Container.DataItem, "RoomTypeName") %></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td style="border-top: none; padding: 4px"><%#DataBinder.Eval(Container.DataItem, "NumberOfRooms") %> Room(s) Of Full</td>
                                        <td style="border-top: none; padding: 4px">Price&nbsp;<input id="txtNumberOfRoomsPrice" runat="server" class="form-control" data-inputmask="'alias': 'numeric', 'groupSeparator': ',', 'autoGroup': true, 'digits': 2, 'digitsOptional': true, 'placeholder': '0'" style="display: inline-block; width: 120px" value="0" ng-change="calculateTotalPrice()" />&nbsp;VND/Room</td>
                                    </tr>
                                    <tr>
                                        <td style="border-top: none; padding: 4px"><%#DataBinder.Eval(Container.DataItem, "NumberOfRoomsSingle") %> Room(s) Of Single</td>
                                        <td style="border-top: none; padding: 4px">Price&nbsp;<input id="txtNumberOfRoomsSinglePrice" runat="server" class="form-control" data-inputmask="'alias': 'numeric', 'groupSeparator': ',', 'autoGroup': true, 'digits': 2, 'digitsOptional': true, 'placeholder': '0'" style="display: inline-block; width: 120px" value="0" ng-change="calculateTotalPrice()" />&nbsp;VND/Room</td>
                                    </tr>
                                    <tr>
                                        <td style="border-top: none; padding: 4px"><%#DataBinder.Eval(Container.DataItem, "NumberOfAddAdult") %> Room(s) Of Add Adult</td>
                                        <td style="border-top: none; padding: 4px">Price&nbsp;<input id="txtNumberOfAddAdultPrice" runat="server" class="form-control" data-inputmask="'alias': 'numeric', 'groupSeparator': ',', 'autoGroup': true, 'digits': 2, 'digitsOptional': true, 'placeholder': '0'" style="display: inline-block; width: 120px" value="0" ng-change="calculateTotalPrice()" />&nbsp;VND/Adult</td>
                                    </tr>
                                    <tr>
                                        <td style="border-top: none; padding: 4px"><%#DataBinder.Eval(Container.DataItem, "NumberOfAddChild") %> Room(s) Of Add Child </td>
                                        <td style="border-top: none; padding: 4px">Price&nbsp;<input id="txtNumberOfAddChildPrice" runat="server" class="form-control" data-inputmask="'alias': 'numeric', 'groupSeparator': ',', 'autoGroup': true, 'digits': 2, 'digitsOptional': true, 'placeholder': '0'" style="display: inline-block; width: 120px" value="0" ng-change="calculateTotalPrice()" />&nbsp;VND/Child</td>
                                    </tr>
                                    <tr>
                                        <td style="border-top: none; padding: 4px"><%#DataBinder.Eval(Container.DataItem, "NumberOfAddBaby") %> Room(s) Of Add Baby</td>
                                        <td style="border-top: none; padding: 4px">Price&nbsp;<input id="txtNumberOfAddBabyPrice" runat="server" class="form-control" data-inputmask="'alias': 'numeric', 'groupSeparator': ',', 'autoGroup': true, 'digits': 2, 'digitsOptional': true, 'placeholder': '0'" style="display: inline-block; width: 120px" value="0" ng-change="calculateTotalPrice()" />&nbsp;VND/Baby</td>
                                    </tr>
                                    <tr>
                                        <td style="border-top: none; padding: 4px"><%#DataBinder.Eval(Container.DataItem, "NumberOfExtrabed") %> Room(s) Of Add Extrabed</td>
                                        <td style="border-top: none; padding: 4px">Price&nbsp;<input id="txtNumberOfExtrabedPrice" runat="server" class="form-control" data-inputmask="'alias': 'numeric', 'groupSeparator': ',', 'autoGroup': true, 'digits': 2, 'digitsOptional': true, 'placeholder': '0'" style="display: inline-block; width: 120px" value="0" ng-change="calculateTotalPrice()" />&nbsp;VND/Extrabed</td>
                                    </tr>
                                </tbody>
                            </table>
                        </ItemTemplate>
                    </asp:Repeater>
                    <%}
                        else
                        {  %>
                    <table class="table" style="display: inline-block; width: auto; border: 1px solid #ccc; padding: 10px; margin-right: 10px">
                        <thead>
                            <tr>
                                <th>Vdream</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td style="border-top: none; padding: 4px"><%= Booking.Adult %> Number Of Adult(s)</td>
                                <td style="border-top: none; padding: 4px" ng-init="numberOfAdult=<%= Booking.Adult %>">Price&nbsp;<input id="txtNumberOfAdultsPrice" runat="server" class="form-control" data-inputmask="'alias': 'numeric', 'groupSeparator': ',', 'autoGroup': true, 'digits': 2, 'digitsOptional': true, 'placeholder': '0'" style="display: inline-block; width: 120px" value="0" ng-model="txtNumberOfAdultsPrice" ng-change="calculateTotalPrice()" />&nbsp;VND/Person</td>
                            </tr>
                            <tr>
                                <td style="border-top: none; padding: 4px"><%= Booking.Child %> Number Of Child(s)</td>
                                <td style="border-top: none; padding: 4px" ng-init="numberOfChild=<%= Booking.Child %>">Price&nbsp;<input id="txtNumberOfChildsPrice" runat="server" class="form-control" data-inputmask="'alias': 'numeric', 'groupSeparator': ',', 'autoGroup': true, 'digits': 2, 'digitsOptional': true, 'placeholder': '0'" style="display: inline-block; width: 120px" value="0" ng-model="txtNumberOfChildsPrice" ng-change="calculateTotalPrice()" />&nbsp;VND/Person</td>
                            </tr>
                            <tr>
                                <td style="border-top: none; padding: 4px"><%= Booking.Baby %> Number Of Baby(s)</td>
                                <td style="border-top: none; padding: 4px" ng-init="numberOfBaby=<%= Booking.Baby %>">Price&nbsp;<input id="txtNumberOfBabysPrice" runat="server" class="form-control" data-inputmask="'alias': 'numeric', 'groupSeparator': ',', 'autoGroup': true, 'digits': 2, 'digitsOptional': true, 'placeholder': '0'" style="display: inline-block; width: 120px" value="0" ng-model="txtNumberOfBabysPrice" ng-change="calculateTotalPrice()" />&nbsp;VND/Person</td>
                            </tr>
                        </tbody>
                    </table>
                    <%} %>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-xs-12">
                    <asp:PlaceHolder ID="plhCruiseCabinControlPanel" runat="server">
                        <div runat="server" visible='<%# UserIdentity.UserName != "captain.os1" %>'>
                            <asp:Button ID="btnAddRoom" runat="server" Text="Add Room"
                                CssClass="btn btn-primary" />
                            <asp:Button ID="btnDeleteAllRoomNA" runat="server" Text="Delete all room N/A" OnClick="btnDeleteAllRoomNA_OnClick"
                                CssClass="btn btn-warning" />
                            <asp:Button ID="btnDeleteRoomSelect" runat="server" Text="Delete all room select" OnClick="btnDeleteRoomSelect_OnClick"
                                CssClass="btn btn-danger" OnClientClick="return confirm('Bạn có chắc chắn muốn xóa các phòng đã chọn, mọi dữ liệu khách theo phòng sẽ bị xóa?')" />
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhCruiseSeatingControlPanel" runat="server">
                        <asp:Button ID="btnAddAdult" runat="server" Text="Add Adult" CssClass="btn btn-primary" OnClick="btnAddAdult_Click" />
                        <asp:Button ID="btnAddChild" runat="server" Text="Add Child" CssClass="btn btn-warning" OnClick="btnAddChild_Click" />
                        <asp:Button ID="btnAddBaby" runat="server" Text="Add Baby" CssClass="btn btn-danger" OnClick="btnAddBaby_Click" />
                    </asp:PlaceHolder>
                </div>

            </div>
        </div>

        <div class="customerinfo-panel">
            <div class="panel panel-default">
                <asp:PlaceHolder ID="plhCruiseCabinCustomer" runat="server">
                    <asp:Repeater ID="rptRoomList" runat="server" OnItemDataBound="rptRoomList_ItemDataBound"
                        OnItemCommand="rptRoomList_ItemCommand">
                        <ItemTemplate>
                            <div class="roominfor-hiddenpanel">
                                <asp:HiddenField ID="hiddenBookingRoomId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "Id") %>' />
                                <asp:HiddenField ID="hiddenRoomClassId" runat="server" />
                                <asp:HiddenField ID="hiddenRoomTypeId" runat="server" />
                                <asp:HiddenField ID="hiddenRoomId" runat="server" />
                            </div>
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-xs-3 --width-auto">
                                        <label for="roomname" class="bkv-roomName">
                                            <asp:HyperLink runat="server" ID="hplRoomName" NavigateUrl="javascript:;"></asp:HyperLink>
                                            <asp:Label ID="label_RoomId" runat="server" Style="display: none;"></asp:Label>

                                        </label>
                                        <div class="checkbox-extrabed">
                                            <label class="checkbox-inline">
                                                <input id="checkboxAddExtraBed" runat="server" type="checkbox" />Add extra bed</label>
                                        </div>
                                    </div>
                                    <div class="col-xs-2">
                                        <asp:CheckBox runat="server" ID="chkDeleteRoom" ToolTip="select for delete" data-toggle="tooltip" data-placement="top" data-original-title="select for delete" />
                                        <asp:TextBox ID="txtRoomNumber" Visible="False" runat="server" CssClass="form-control" placeholder="Room Number" title="Room Number">
                                        </asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-xs-5">
                                        <div class="checkbox" style="margin-left: 0px">
                                            <label class="checkbox-inline">
                                                <input id="checkBoxAddAdult" runat="server" type="checkbox" />Add Adult</label>
                                            <label class="checkbox-inline">
                                                <input id="checkBoxAddChild" runat="server" type="checkbox" />Add Child</label>
                                            <label class="checkbox-inline">
                                                <input id="checkBoxAddBaby" runat="server" type="checkbox" />Add Baby</label>
                                            <label class="checkbox-inline">
                                                <input id="checkBoxSingle" runat="server" type="checkbox" />Single</label>
                                        </div>
                                    </div>
                                    <div class="col-xs-offset-1 col-xs-2 text-right">
                                        <asp:Label ID="labelRoomTypes" runat="server" Text="change room type to"></asp:Label>
                                    </div>
                                    <div class="col-xs-2 --no-padding-leftright">
                                        <asp:DropDownList ID="ddlRoomTypes" runat="server" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-xs-2" runat="server" visible='<%# UserIdentity.UserName != "captain.os1" %>'>
                                        <asp:Button ID="btnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                                            CommandName="delete" CssClass="btn btn-primary" Text="Delete this room" OnClientClick="return confirm('All unsaved customer data (included another rooms in this book) will be lost forever. Are you sure want to delete this room?')" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-xs-1">
                                        <label for="client1">Client 1</label>
                                    </div>
                                    <uc:customer ID="customer1" runat="server" ChildAllowed="true"></uc:customer>
                                    <div>
                                        <asp:Repeater ID="rptServices1" runat="server">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkService" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>'
                                                    CssClass="checkbox" /><asp:HiddenField ID="hiddenServiceId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "Id") %>' />
                                                <asp:HiddenField ID="hiddenId" runat="server" />
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                                <div class="row" id="trCustomer2" runat="server">
                                    <div class="col-xs-1">
                                        <label for="client1">Client 2</label>
                                    </div>
                                    <uc:customer ID="customer2" runat="server" ChildAllowed="true"></uc:customer>
                                    <div>
                                        <asp:Repeater ID="rptServices2" runat="server">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkService" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>'
                                                    CssClass="checkbox" /><asp:HiddenField ID="hiddenServiceId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "Id") %>' />
                                                <asp:HiddenField ID="hiddenId" runat="server" />
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                                <div class="row" id="trExtraBed" runat="server">
                                    <div class="col-xs-1">
                                        <label for="client1">Add adult</label>
                                    </div>
                                    <ucextra:customer ID="customerExtraBed" runat="server"></ucextra:customer>
                                    <div>
                                        <asp:Repeater ID="Repeater1" runat="server">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkService" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>'
                                                    CssClass="checkbox" /><asp:HiddenField ID="hiddenServiceId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "Id") %>' />
                                                <asp:HiddenField ID="hiddenId" runat="server" />
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                                <div class="row" id="trChild" runat="server">
                                    <div class="col-xs-1">
                                        <label for="client1">Child info</label>
                                    </div>
                                    <uc:customer ID="customerChild" runat="server"></uc:customer>
                                    <div>
                                        <asp:Repeater ID="rptServicesChild" runat="server">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkService" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>'
                                                    CssClass="checkbox" /><asp:HiddenField ID="hiddenServiceId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "Id") %>' />
                                                <asp:HiddenField ID="hiddenId" runat="server" />
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                                <div class="row" id="trBaby" runat="server">
                                    <div class="col-xs-1">
                                        <label for="client1">Baby info</label>
                                    </div>
                                    <uc:customer ID="customerBaby" runat="server"></uc:customer>
                                    <div>
                                        <asp:Repeater ID="rptServicesBaby" runat="server">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkService" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>'
                                                    CssClass="checkbox" /><asp:HiddenField ID="hiddenServiceId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "Id") %>' />
                                                <asp:HiddenField ID="hiddenId" runat="server" />
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhCruiseSeatingCustomer" runat="server">
                    <h3>&nbsp;&nbsp;<%= Booking.Adult %> Adult</h3>
                    <hr />
                    <asp:Repeater ID="rptAdults" runat="server" OnItemDataBound="rptAdults_ItemDataBound" OnItemCommand="rptAdults_ItemCommand">
                        <ItemTemplate>
                            <div class="row">
                                <div class="col-xs-1" style="width: 6%">
                                    <label for="client1">Adult <%# Container.ItemIndex + 1 %></label>
                                </div>
                                <uc:customer ID="customer1" runat="server" ChildAllowed="true" SeatingCruise="true"></uc:customer>
                                <div class="col-xs-1" style="width: 5%">
                                    <asp:Button ID="btnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                                        CommandName="delete" CssClass="btn btn-primary" Text="Delete" OnClientClick="return confirm('Are you sure?')" />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <h3>&nbsp;&nbsp;<%= Booking.Child %>  Children</h3>
                    <hr />
                    <asp:Repeater ID="rptChildren" runat="server" OnItemDataBound="rptChildren_ItemDataBound" OnItemCommand="rptChildren_ItemCommand">
                        <ItemTemplate>
                            <div class="row">
                                <div class="col-xs-1" style="width: 6%">
                                    <label for="client1">Child <%# Container.ItemIndex + 1 %></label>
                                </div>
                                <uc:customer ID="customer1" runat="server" ChildAllowed="true" SeatingCruise="true"></uc:customer>
                                <div class="col-xs-1" style="width: 5%">
                                    <asp:Button ID="btnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                                        CommandName="delete" CssClass="btn btn-primary" Text="Delete" OnClientClick="return confirm('Are you sure?')" />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <h3>&nbsp;&nbsp;<%= Booking.Baby %> Babies</h3>
                    <hr />
                    <asp:Repeater ID="rptBabies" runat="server" OnItemDataBound="rptBabies_ItemDataBound" OnItemCommand="rptBabies_ItemCommand">
                        <ItemTemplate>
                            <div class="row">
                                <div class="col-xs-1" style="width: 6%">
                                    <label for="client1">Baby <%# Container.ItemIndex + 1 %></label>
                                </div>
                                <uc:customer ID="customer1" runat="server" ChildAllowed="true" SeatingCruise="true"></uc:customer>
                                <div class="col-xs-1" style="width: 5%">
                                    <asp:Button ID="btnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                                        CommandName="delete" CssClass="btn btn-primary" Text="Delete" OnClientClick="return confirm('Are you sure?')" />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </asp:PlaceHolder>
            </div>
        </div>
        <div runat="server" visible='<%# UserIdentity.UserName != "captain.os1" %>'>
            <asp:PlaceHolder ID="plhAddRoom" runat="server">
                <div class="row">
                    <div class="col-xs-12">
                        <asp:DropDownList ID="ddlRoomTypes" Visible="False" CssClass="form-control" runat="server">
                        </asp:DropDownList>
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
         <div>
            <div class="modal fade modal-serviceOutsideDetail{{serviceOutside.id}}" tabindex="-1" role="dialog" aria-labelledby="gridSystemModalLabel" data-backdrop="static" data-keyboard="true" ng-init="loadServiceOutsideDetail(serviceOutside.id)" ng-repeat="serviceOutside in listServiceOutside">
                <div class="modal-dialog" role="document" style="width: 1100px">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                            <h3 class="modal-title">Chi tiết của dịch vụ {{serviceOutside.service}}
                            <button type="button" class="btn btn-primary" ng-click="addServiceOutsideDetail(serviceOutside.id)">Thêm</button></h3>
                        </div>
                        <div class="modal-body">
                            <div class="form-group" ng-show="serviceOutside.listServiceOutsideDetailDTO.length > 0">
                                <div class="row">
                                    <div class="col-xs-2 nopadding-right" style="width: 20%">
                                        Tên
                                    </div>
                                    <div class="col-xs-2 nopadding-right" style="width: 20%">
                                        Đơn giá
                                    </div>
                                    <div class="col-xs-2 nopadding-right" style="width: 20%">
                                        Số lượng
                                    </div>
                                    <div class="col-xs-2 nopadding-right" style="width: 20%">
                                        Thành tiền
                                    </div>
                                </div>
                            </div>
                            <div class="form-group" ng-repeat="serviceOutsideDetail in serviceOutside.listServiceOutsideDetailDTO">
                                <div class="row">
                                    <input type="hidden" ng-model="item.id">
                                    <div class="col-xs-2 nopadding-right" style="width: 20%">
                                        <input type="text" class="form-control" placeholder="Tên" ng-model="serviceOutsideDetail.name" />
                                    </div>
                                    <div class="col-xs-2 nopadding-right" style="width: 20%">
                                        <div class="input-group">
                                            <input type="text" class="form-control" placeholder="Đơn giá" ng-model="serviceOutsideDetail.unitPrice" data-control="inputmask" data-inputmask="'alias': 'numeric', 'groupSeparator': ',', 'autoGroup': true, 'digits': 2, 'digitsOptional': true, 'rightAlign':false"
                                                ng-change="calculateServiceOutsideDetail(serviceOutside, $index, serviceOutsideDetail.unitPrice, serviceOutsideDetail.quantity)" />
                                            <span class="input-group-addon">VND</span>
                                        </div>
                                    </div>
                                    <div class="col-xs-2 nopadding-right" style="width: 20%">
                                        <input type="text" class="form-control" placeholder="Số lượng" ng-model="serviceOutsideDetail.quantity" ng-change="calculateServiceOutsideDetail(serviceOutside, $index, serviceOutsideDetail.unitPrice, serviceOutsideDetail.quantity)" />
                                    </div>
                                    <div class="col-xs-2 nopadding-right" style="width: 20%">
                                        <div class="input-group">
                                            <input type="text" class="form-control" placeholder="Thành tiền" ng-model="serviceOutsideDetail.totalPrice" data-control="inputmask" data-inputmask="'alias': 'numeric', 'groupSeparator': ',', 'autoGroup': true, 'digits': 2, 'digitsOptional': true, 'rightAlign':false" ng-change="calculateTotalServiceOutside()" />
                                            <span class="input-group-addon">VND</span>
                                        </div>
                                    </div>

                                    <div class="col-xs-2" style="width: 20%">
                                        <button type="button" class="btn btn-primary" ng-click="removeServiceOutsideDetail($index,serviceOutside.id)">
                                            Xóa
                                        </button>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group" ng-show="serviceOutside.listServiceOutsideDetailDTO.length > 0">
                                <div class="row">
                                    <div class="col-xs-offset-7 col-xs-4" style="margin-left: 60%">
                                        Tổng : {{ calculateTotalServiceOutsideDetail(serviceOutside) }}
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-primary" data-dismiss="modal">OK</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal fade" id="addBookingModal" tabindex="-1" role="dialog" aria-labelledby="gridSystemModalLabel" data-backdrop="true" data-keyboard="true">
            <div class="modal-dialog" role="document" style="width: 100%">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <iframe frameborder="0" width="100%" scrolling="yes" onload="resizeIframe(this)"></iframe>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Scripts" ContentPlaceHolderID="Scripts" runat="server">
    <script type="text/javascript" src="/modules/sails/admin/bookingviewcontroller.js"></script>
    <script type="text/javascript">
        $(function () {
            $(function () {
                $("#<%= txtPickupTime.ClientID%>").datetimepicker({
                    datepicker: false,
                    timepicker: true,
                    format: 'H:i',
                    scrollInput: false,
                    scrollMonth: false
                });
            })
            $("#<%= txtStartDate.ClientID%>").datetimepicker({
                timepicker: false,
                format: 'd/m/Y',
                scrollInput: false,
                scrollMonth: false
            });

            $("#<%= txtDeadline.ClientID%>").datetimepicker({
                format: 'd/m/Y H:i',
                scrollImput: false,
                scrollMonth: false
            });
        })
    </script>
    <script type="text/javascript">  
        $(function () {
            //workaround datetimepicker do not show date table when off mousedown and focusin and blank input value
            $("[data-control = 'datepicker']").each(function (i, e) {
                if ($(e).val() == "") {
                    $(e).val("workaround");
                }
            })

            $("[data-control = 'datepicker']").datetimepicker({
                timepicker: false,
                format: 'd/m/Y',
                scrollInput: false,
                scrollMonth: false,
            })

            $("[data-control = 'datepicker']").each(function (i, e) {
                if ($(e).val() == "workaround") {
                    $(e).val("");
                }
            })

            $("[data-control = 'datepicker']").off("mousedown");
            $("[data-control = 'datepicker']").off("focusin");

            $(".fa-calendar").click(function () {
                $(this).siblings("input").datetimepicker("show");
            })
        });

    </script>

    <script>
        DropdownOptionSetVisible();
        $("#<%=ddlTrips.ClientID%>").change(DropdownOptionSetVisible);

        function DropdownOptionSetVisible() {
            $("#<%=ddlOptions.ClientID%>").hide();
            if ($("#<%=ddlTrips.ClientID%> option:selected").attr("data-option-visible") == "true") {
                $("#<%=ddlOptions.ClientID%>").show();
            }

        }
    </script>

    <script>
        TACodeSetVisible();
        $("#<%=ddlAgencies.ClientID%>").change(TACodeSetVisible);

        function TACodeSetVisible() {
            $("#<%=txtAgencyCode.ClientID%>").hide();
            if (!$("#<%=ddlAgencies.ClientID%> option:selected").is($("#<%=ddlAgencies.ClientID%> option:first-child"))) {
                $("#<%=txtAgencyCode.ClientID%>").show();
            }
        }
    </script>
    <script type="text/javascript">
        function toggleVisible(id) {
            item = document.getElementById(id);
            if (item.style.display == "") {
                item.style.display = "none";
            } else {
                item.style.display = "";
            }
        }
    </script>
    <script>
        function CheckVoucher() {
            PopupCenter(url, 'Check Voucher', 400, 600);
        }
    </script>
    <script>
        function statusInforShow() {
            var txtCutOffDays = $("#<%= txtCutOffDays.ClientID%>");
            var txtDeadline = $("#<%= txtDeadline.ClientID%>");
            var txtCancelledReason = $("#<%= txtCancelledReason.ClientID%>");
            var selectedStatus = $("#<%= ddlStatusType.ClientID%> option:selected");

            if (selectedStatus.html() === "Pending") {
                txtDeadline.show();
                txtCutOffDays.hide();
                txtCancelledReason.hide();
            }
            if (selectedStatus.html() === "CutOff") {
                txtDeadline.hide();
                txtCutOffDays.show();
                txtCancelledReason.hide();
            }
            if (selectedStatus.html() === "Cancelled") {
                txtDeadline.hide();
                txtCutOffDays.hide();
                txtCancelledReason.show();
            }
            if (selectedStatus.html() === "Approved") {
                txtDeadline.hide();
                txtCutOffDays.hide();
                txtCancelledReason.hide();
            }
        }

        statusInforShow();
        $("#<%= ddlStatusType.ClientID%>").change(statusInforShow);
    </script>

    <script>
        function setPersonalInfomation(control, ui) {
            control.val(ui.item.Fullname);
            var divparentControl = control.parents(".row");
            if (ui.item.HasGenderValue === true) {
                if (ui.item.IsMale === false) {
                    divparentControl.find(".ddlGender").val("Female");
                } else {
                    divparentControl.find(".ddlGender").val("Male");
                }
            } else {
                divparentControl.find(".ddlGender").children(":first").prop("selected", "selected");
            }


            if (ui.item.HasBirthdayValue === true) {
                divparentControl.find(".txtBirthday").val(ui.item.Birthday);
            } else {
                divparentControl.find(".txtBirthday").val("");
            }

            if (ui.item.HasNationality === true) {
                divparentControl.find(".ddlNationality").val(ui.item.NationId);
            } else {
                divparentControl.find(".ddlNationality").children(":first").prop("selected", "selected");
            }

            divparentControl.find(".txtVisaNo").val(ui.item.VisaNo);
            divparentControl.find(".txtPassport").val(ui.item.Passport);

            if (ui.item.HasVisaExpiredValue === true) {
                divparentControl.find(".txtVisaExpired").val(ui.item.VisaExpired);
            }

            divparentControl.find(".txtNguyenQuan").val(ui.item.NguyenQuan);

            divparentControl.find(".chkVietKieu").children("input").prop("checked", "checked");

        }

    </script>
    <script>
        var fullNameArray = [];
        $(document).ready(function () {
            $.each($(".acomplete"), function (i, e) {
                if ($(e).val() === "") {
                    fullNameArray.push({ selected: false, originFullName: $(e).val(), control: $(e) });
                } else {
                    fullNameArray.push({ selected: true, originFullName: $(e).val(), control: $(e) });
                }
            });
        });
    </script>
    <script>
        $(document).ready(function () {
            $.each($(".acomplete"), function (i, e) {
                $(e).autocomplete({
                    source: "SearchCustomer.aspx?NodeId=1&SectionId=15",
                    select: function (event, ui) {
                        $.each(fullNameArray, function (index, element) {
                            if ($(e).is(element["control"])) {
                                element["originFullName"] = ui.item.Fullname;
                                element["selected"] = true;
                            }
                        });
                        setPersonalInfomation($(this), ui);
                        return false;
                    }
                }).autocomplete("instance")._renderItem = function (ul, item) {
                    var itemElement = "<a>" + item.Fullname + "<br>";
                    if (item.HasGenderValue === true) {
                        if (item.IsMale === false) {
                            itemElement = itemElement + "<b>Gender : Female</b> ";
                        } else {
                            itemElement = itemElement + "<b>Gender : Male</b> ";
                        }
                    }

                    if (item.HasBirthdayValue === true) {
                        itemElement = itemElement + "<b>Birthday : " + item.Birthday + "</b> ";
                    }

                    if (item.HasNationality === true) {
                        itemElement = itemElement + "<b>Nationality : " + item.Nationality + "</b> ";
                    }
                    itemElement = itemElement + "</a>";
                    return $("<li>").append(itemElement).appendTo(ul);
                };
            });
        });
    </script>
    <script>
        function resetPersonInformation(control) {
            control.siblings(".hiddenId").val("");
        }
    </script>
    <script>
        $(function () {
            $.each($(".acomplete"), function (i, e) {
                $(e).keyup(function () {
                    $.each(fullNameArray, function (index, element) {
                        if ($(e).is(element["control"])) {
                            if (element["selected"] === true) {
                                if ($(e).val() !== element["originFullName"]) {
                                    resetPersonInformation($(e));
                                    element["selected"] = false;
                                }
                            }
                        }
                    });
                });
            });
        })
    </script>
    <script>
        //change all selector nationality follow first selector
        $(function () {
            $(".ddlNationality:first").change(function () {
                $(".ddlNationality").val($(".ddlNationality:first").val());
            });
        });
    </script>
    <script>
        $("#sendemail").colorbox({
            iframe: true,
            width: 1200,
            height: 600,
        });

        if (getParameterValues("confirm") == 1) {
            $("#sendemail").colorbox({
                iframe: true,
                width: 1200,
                height: 600,
                open: true
            });
        }
    </script>
    <script>
        $("#roomorganizer").colorbox({
            iframe: true,
            width: 1200,
            height: 600,
        });
    </script>
    <script>
        AgencyDropdownlistFillTitle();
        $("#<%=ddlAgencies.ClientID%>").change(function () {
            AgencyDropdownlistFillTitle();
        });
        function AgencyDropdownlistFillTitle() {
            $("#<%=ddlAgencies.ClientID%>").attr("title", $("#<%=ddlAgencies.ClientID%> option:selected").html());
        }
    </script>
    <script>
        $("#checkvoucher").click(function () {
            var code = document.getElementById('<%= txtAllVoucher.ClientID %>').value;
            var url = 'CheckVoucher.aspx?NodeId=1&SectionId=15&code=' + code + '&bookingid=' + <%= Request.QueryString["bi"] %>;
            $.colorbox({
                href: url,
                iframe: true,
                width: 1200,
                height: 600,
            })
        });
    </script>

    <script>
        $(document).ready(function () {
            var $controllerTransferServiceScope = angular.element(document.querySelector("[ng-controller='bookingViewController']")).scope();
            $controllerTransferServiceScope.Transfers = Transfers;
        })
    </script>
    <script>
        $(document).ready(function () {
            var $controllerTransferServiceScope = angular.element(document.querySelector("[ng-controller='bookingViewController']")).scope();
            $controllerTransferServiceScope.addServiceWatch = function (control) {
                $controllerTransferServiceScope.$watch('transfer_Service', function () {
                    if ($controllerTransferServiceScope.transfer_Service == "rbtTransferService_TwoWay") {
                        $controllerTransferServiceScope.transfer_DateTo = "<%= Booking.StartDate.ToString("dd/MM/yyyy")%>";
                        $controllerTransferServiceScope.transfer_DateBack = "<%= Booking.EndDate.ToString("dd/MM/yyyy")%>";
                    } else if ($controllerTransferServiceScope.transfer_Service == "rbtTransferService_OneWay") {
                        $controllerTransferServiceScope.transfer_DateTo = "<%= Booking.Transfer_DateTo.HasValue 
    ? Booking.Transfer_DateTo.Value.ToString("dd/MM/yyyy"): Booking.StartDate.ToString("dd/MM/yyyy")%>";
                        $controllerTransferServiceScope.transfer_DateBack = "";
                    }
                });
                angular.element(document.querySelector("[ng-controller='bookingViewController']")).scope().$apply();
            }
            $("#<%= rbtTransferService_OneWay.ClientID %>")
                .attr({ "ng-model": "transfer_Service" }).change(function () {
                    angular.element(document.querySelector("[ng-controller='bookingViewController']")).scope().addServiceWatch(this);
                });
            $("#<%= rbtTransferService_TwoWay.ClientID %>")
                .attr({
                    "ng-model": "transfer_Service", "ng-init": "transfer_Service='"
                        +
                    "<%= String.IsNullOrEmpty(Booking.Transfer_Service) 
    ? "rbtTransferService_TwoWay" : (Booking.Transfer_Service == "Two Way" 
    ? "rbtTransferService_TwoWay" : "rbtTransferService_OneWay")%>"
                        + "'"
                }).change(function () {
                    angular.element(document.querySelector("[ng-controller='bookingViewController']")).scope().addServiceWatch(this);
                });
            angular.element(document.querySelector("[ng-controller='bookingViewController']")).injector().invoke(function ($rootScope, $compile) {
                $compile(document.getElementById("<%= rbtTransferService_OneWay.ClientID %>"))($rootScope);
                $compile(document.getElementById("<%= rbtTransferService_TwoWay.ClientID %>"))($rootScope);
            });
        })
    </script>
    <script>
        $(document).ready(function () {
            var $controllerTransferServiceScope = angular.element(document.querySelector("[ng-controller='bookingViewController']")).scope();
            function LoadTransferDate() {
                if ($controllerTransferServiceScope.transfer_Service == "rbtTransferService_TwoWay") {
                    $controllerTransferServiceScope.transfer_DateTo = "<%= Booking.Transfer_DateTo.HasValue 
    ? Booking.Transfer_DateTo.Value.ToString("dd/MM/yyyy"): Booking.StartDate.ToString("dd/MM/yyyy")%>";
                    $controllerTransferServiceScope.transfer_DateBack = "<%= Booking.Transfer_DateBack.HasValue 
    ? Booking.Transfer_DateBack.Value.ToString("dd/MM/yyyy") : Booking.EndDate.ToString("dd/MM/yyyy")%>";
                } else
                    if ($controllerTransferServiceScope.transfer_Service == "rbtTransferService_OneWay") {
                        $controllerTransferServiceScope.transfer_DateTo = "<%= Booking.Transfer_DateTo.HasValue 
    ? Booking.Transfer_DateTo.Value.ToString("dd/MM/yyyy"): ""%>";
                        $controllerTransferServiceScope.transfer_DateBack = "<%= Booking.Transfer_DateBack.HasValue 
    ? Booking.Transfer_DateBack.Value.ToString("dd/MM/yyyy") : ""%>";
                    }
            }

            $controllerTransferServiceScope.transferDateBackChangedHandler = function () {
                if ($controllerTransferServiceScope.transfer_Service == "rbtTransferService_OneWay") {
                    $controllerTransferServiceScope.transfer_DateTo = "";
                }
            };
            $controllerTransferServiceScope.transferDateToChangedHandler = function () {
                if ($controllerTransferServiceScope.transfer_Service == "rbtTransferService_OneWay") {
                    $controllerTransferServiceScope.transfer_DateBack = "";
                }
            };
            LoadTransferDate();
            $controllerTransferServiceScope.$apply();
        })
    </script>
    <script>
        $("#btnSave").click(function () {
            if ($("#aspnetForm").valid()) {
                angular.element(document.querySelector("[ng-controller='bookingViewController']")).scope().save();
                angular.element(document.querySelector("[ng-controller='bookingViewController']")).scope().$apply();
            }
        })
        function closePoup(refesh) {
            $("#addBookingModal").modal('hide');
            if (refesh === 1) {
                window.location.href = window.location.href;
            }
        }
        function editRoom(bookingId, roomId) {
            var src = "/Modules/Sails/Admin/HomeChangeRoom.aspx?NodeId=1&SectionId=15&roomId=" + roomId + "&bookingId=" + bookingId + "&tripId=" + $("#<%=ddlTrips.ClientID%>").val();
            $("#addBookingModal iframe").attr('src', src);
            $("#addBookingModal").modal();
        }
        function selectRoom(bookingId, bkroomId) {
            var src = "/Modules/Sails/Admin/HomeChangeRoom.aspx?NodeId=1&SectionId=15&bookingRoomId=" + bkroomId + "&bookingId=" + bookingId + "&tripId=" + $("#<%=ddlTrips.ClientID%>").val();
            $("#addBookingModal iframe").attr('src', src);
            $("#addBookingModal").modal();
        }

        function changeBoat() {
            var src = "/Modules/Sails/Admin/HomeChangeAllRoom.aspx?NodeId=1&SectionId=15&change=boat&bookingId=" +
                '<%=Request["bi"]%>' +
                "&cruiseId=" +
                $("#<%=ddlCruises.ClientID%>").val();
            src = src + "&startdate=" + $("#<%=txtStartDate.ClientID%>").val();
            $("#addBookingModal iframe").attr('src', src);
            $("#addBookingModal").modal();
            return false;
        }
        function changeTrip() {
            var src = "/Modules/Sails/Admin/HomeChangeAllRoom.aspx?NodeId=1&SectionId=15&change=trip&bookingId=" +
                '<%=Request["bi"]%>' +
                "&cruiseId=" +
                $("#<%=ddlCruises.ClientID%>").val();
            src = src + "&startdate=" + $("#<%=txtStartDate.ClientID%>").val();
            $("#addBookingModal iframe").attr('src', src);
            $("#addBookingModal").modal();
            return false;
        }
        function addRoom(bookingId) {
            var src = "/Modules/Sails/Admin/BookingAddRoom.aspx?NodeId=1&SectionId=15&addBooking=1&bookingId=" + bookingId;
            $("#addBookingModal iframe").attr('src', src);
            $("#addBookingModal").modal();
            return false;
        }
    </script>
    <script>
        $("#addBookingModal").on('shown.bs.modal', function () {
            $(this).css('padding-right', 0);
        });
    </script>
</asp:Content>
