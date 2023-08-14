<%@ Page Language="C#" MasterPageFile="MO.Master" AutoEventWireup="true"
    CodeBehind="AddBookingRandom.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.AddBookingRandom" Title="Booking Adding Random" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="form-group">
        <div class="row">
            <div class="col-xs-1">
                Start date
           
            </div>
            <div class="col-xs-2 --no-padding-left">
                <asp:TextBox ID="txtDate" runat="server" data-control="datetimepicker" class="form-control" placeholder="Start date (dd/mm/yyyy)" AutoPostBack="true" OnTextChanged="txtDate_TextChanged" autocomplete="off">
                </asp:TextBox>
            </div>
            <div class="col-xs-1">
                Trip
           
            </div>
            <div class="col-xs-4 --no-padding-left">
                <asp:UpdatePanel runat="server" ID="updatePanel2">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlTrips" runat="server" AutoPostBack="true" class="form-control" OnSelectedIndexChanged="ddlTrips_SelectedIndexChanged">
                        </asp:DropDownList>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="txtDate" EventName="TextChanged" />
                    </Triggers>
                </asp:UpdatePanel>

            </div>
        </div>
    </div>
    <div class="form-group">
        <div class="row">
            <div class="col-xs-1">
                Agency
           
            </div>
            <div class="col-xs-3 --no-padding-left">
                <input type="text" name="txtAgency" id="<%= agencySelector.ClientID %>nameid" class="form-control"
                    readonly placeholder="Click to select agency" />
                <input id="agencySelector" type="hidden" runat="server" />
            </div>
            <div class="col-xs-1 nopadding-left">
                <asp:TextBox ID="txtAgencyCode" runat="server" class="form-control" placeholder="TA Code"></asp:TextBox>
            </div>
            <div class="col-xs-2">
                <asp:Repeater ID="rptExtraServices" runat="server" OnItemDataBound="rptExtraServices_ItemDataBound">
                    <ItemTemplate>
                        <div class="checkbox" style="margin-left: 0">
                            <label>
                                <input id="chkService" runat="server" type="checkbox" /><%#Eval("Name") %></label>
                        </div>
                        <asp:HiddenField ID="hiddenId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Id") %>' />
                        <asp:HiddenField ID="hiddenValue" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Price") %>' />
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div style="display: none">
                <asp:DropDownList ID="ddlCruises" runat="server">
                </asp:DropDownList>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12">
            <asp:UpdatePanel runat="server" ID="updatePanel1">
                <ContentTemplate>
                    <em>Click vào tên tàu để bắt đầu nhập thông tin phòng</em>
                    <table class="table table-bordered table-hover">
                        <tr class="active">
                            <th>Tên tàu
                            </th>
                            <th>Số (phòng / ghế) trống
                            </th>
                            <th>Trong đó
                            </th>
                        </tr>
                        <asp:Repeater ID="rptCruises" runat="server" OnItemDataBound="rptCruises_ItemDataBound">
                            <ItemTemplate>
                                <tr id="trCruise" runat="server">
                                    <td>
                                        <asp:LinkButton ID="lbtCruiseName" runat="server" OnClick="lbtCruiseName_Click"></asp:LinkButton>
                                        <asp:Literal ID="litName" runat="server"></asp:Literal>
                                    </td>
                                    <td>
                                        <asp:Literal ID="litRoomCount" runat="server"></asp:Literal>
                                    </td>
                                    <td>
                                        <asp:Literal ID="litRoomDetail" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                    <asp:PlaceHolder runat="server" ID="plhPending" Visible="False"><em>Booking pending</em>
                        <table class="table table-bordered table-hover">
                            <tr class="active">
                                <th>Booking code
                                </th>
                                <th>Rooms
                                </th>
                                <th>Trip
                                </th>
                                <th>Partner
                                </th>
                                <th>Created by
                                </th>
                                <th>Sale in charge
                                </th>
                                <th>Pending until
                                </th>
                            </tr>
                            <asp:Repeater runat="server" ID="rptPendings" OnItemDataBound="rptPendings_ItemDataBound">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <asp:HyperLink runat="server" ID="hplCode"></asp:HyperLink>
                                        </td>
                                        <td>
                                            <asp:Literal runat="server" ID="litRooms"></asp:Literal>
                                        </td>
                                        <td>
                                            <asp:Literal runat="server" ID="litTrip"></asp:Literal>
                                        </td>
                                        <td>
                                            <asp:HyperLink runat="server" ID="hplAgency"></asp:HyperLink>
                                        </td>
                                        <td>
                                            <asp:Label runat="server" ID="lblCreatedBy"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label runat="server" ID="lblSaleInCharge"></asp:Label>

                                        </td>
                                        <td>
                                            <asp:Literal runat="server" ID="litPending"></asp:Literal>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </table>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhCruiseName" runat="server" Visible="false">
                        <div class="form-group">
                            <div class="row">
                                <div class="col-xs-2 --no-padding-left">
                                    Chọn tàu <strong>
                                        <asp:Literal ID="litCurrentCruise" runat="server" /></strong>
                                </div>
                                <div class="col-xs-2 --no-padding-left">
                                    <asp:CheckBox runat="server" ID="chkCharter" Text=" Charter Booking"></asp:CheckBox>
                                </div>
                            </div>
                        </div>
                        <asp:Repeater ID="rptClass" runat="server" OnItemDataBound="rptClass_ItemDataBound">
                            <ItemTemplate>
                                <asp:HiddenField ID="hiddenId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem,"Id") %>' />
                                <asp:Repeater ID="rptTypes" runat="server" OnItemDataBound="rptTypes_ItemDataBound">
                                    <ItemTemplate>
                                        <div class="form-group">
                                            <div class="row">
                                                <div class="col-xs-2">
                                                    <asp:Label ID="labelName" runat="server"></asp:Label><asp:HiddenField ID="hiddenId"
                                                        runat="server" Value='<%# DataBinder.Eval(Container.DataItem,"Id") %>' />
                                                </div>
                                                <div class="col-xs-1 nopadding-left nopadding-right">
                                                    <asp:DropDownList ID="ddlAdults" runat="server" CssClass="form-control">
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-xs-1 nopadding-left nopadding-right">
                                                    <asp:DropDownList ID="ddlChild" runat="server" CssClass="form-control">
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-xs-1 nopadding-left nopadding-right">
                                                    <asp:DropDownList ID="ddlBaby" runat="server" CssClass="form-control">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:HiddenField ID="hidCruiseSelect" runat="server" />
                        <asp:HiddenField ID="hidDateSelect" runat="server" />
                        <asp:PlaceHolder ID="phRoomAvailableNight2" runat="server">
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-xs-2">
                                        <strong>Các phòng trống vào ngày hôm sau (không trống ngày đang chọn)</strong>
                                    </div>
                                </div>
                            </div>
                            <asp:Repeater ID="rptRoomAvailableNight2" runat="server">
                                <ItemTemplate>
                                    <div class="form-group">
                                        <div class="row">
                                            <div class="col-xs-2">
                                                <%#Eval("Name") %>
                                            </div>
                                            <div class="col-xs-1 nopadding-left nopadding-right">
                                                <input type="button" class="btn btn-primary" id="btnRoomPlan" onclick="viewRoomPlan(); return false;" value="Room Plan" />
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </asp:PlaceHolder>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phSeatingDeclaration" runat="server" Visible="false">
                        <div class="form-group">
                            <div class="row">
                                <div class="col-xs-2">
                                    <asp:Label ID="lblAdults" runat="server">Adults</asp:Label>
                                </div>
                                <div class="col-xs-1 nopadding-left nopadding-right">
                                    <asp:TextBox ID="txtAdults" runat="server" type="number" CssClass="form-control" />
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="row">
                                <div class="col-xs-2">
                                    <asp:Label ID="lblChilds" runat="server">Childs</asp:Label>
                                </div>
                                <div class="col-xs-1 nopadding-left nopadding-right">
                                    <asp:TextBox ID="txtChilds" runat="server" type="number" CssClass="form-control"/>
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="row">
                                <div class="col-xs-2">
                                    <asp:Label ID="lblBabies" runat="server">Babies</asp:Label>
                                </div>
                                <div class="col-xs-1 nopadding-left nopadding-right">
                                    <asp:TextBox ID="txtBabies" runat="server" type="number" CssClass="form-control"/>
                                </div>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="txtDate" EventName="TextChanged" />
                    <asp:AsyncPostBackTrigger ControlID="ddlTrips" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12">
            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" CssClass="btn btn-primary" />
        </div>
    </div>
    <div class="modal fade" id="osModal" tabindex="-1" role="dialog" aria-labelledby="gridSystemModalLabel" data-backdrop="static" data-keyboard="true">
        <div class="modal-dialog" role="document" style="width: 85vw; height: 80vh">
            <div class="modal-content">
                <div class="modal-header">
                    <%--                    <span>Add booking</span>--%>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <iframe frameborder="0" width="100%" style="height: 80vh" scrolling="yes"></iframe>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Scripts" ContentPlaceHolderID="Scripts" runat="server">
    <script>
        $("#<%= agencySelector.ClientID%>nameid").click(function () {
            var width = 800;
            var height = 600;
            window.open('/Modules/Sails/Admin/AgencySelectorPage.aspx?NodeId=1&SectionId=15&clientid=<%= agencySelector.ClientID%>', 'Agencyselect', 'width=' + width + ',height=' + height + ',top=' + ((screen.height / 2) - (height / 2)) + ',left=' + ((screen.width / 2) - (width / 2)));
        });
        function closePoup(refesh) {
            $("#osModal").modal('hide');
            if (refesh === 1) {
                var date = $('#<%=hidDateSelect.ClientID%>').val();
                window.location.href = window.location.href + "&date=" + date + "&cruiseId=" + $('#<%=hidCruiseSelect.ClientID%>').val()
            }
        }
        function viewRoomPlan() {
            var date = $('#<%=hidDateSelect.ClientID%>').val();
            var src = "/Modules/Sails/Admin/HomeRoomPlan.aspx?NodeId=1&SectionId=15&date=" + date + "&cruiseId=" + $('#<%=hidCruiseSelect.ClientID%>').val();
            $("#osModal iframe").attr('src', src);
            $("#osModal").modal();
            return false;
        }
        $(document).ready(function () {
            $("#aspnetForm").validate({
                rules: {
                    <%=txtDate.UniqueID%>: {
                required: true,
            },
                txtAgency: {
                required: true,
            },
                },
            messages: {
                    <%=txtDate.UniqueID%>: {
            required: "Yêu cầu chọn ngày khởi hành",
        },
            txtAgency: {
            required: "Yêu cầu chọn Agency",
        },
                },
            errorElement: "em",
            errorPlacement: function (error, element) {
                error.addClass("help-block");

                if (element.prop("type") === "checkbox") {
                    error.insertAfter(element.parent("label"));
                } else {
                    error.insertAfter(element);
                }

                if (element.siblings("span").prop("class") === "input-group-addon") {
                    error.insertAfter(element.parent()).css({ color: "#a94442" });
                }
            },
            highlight: function (element, errorClass, validClass) {
                $(element).closest("div").addClass("has-error").removeClass("has-success");
            },
            unhighlight: function (element, errorClass, validClass) {
                $(element).closest("div").removeClass("has-error");
            }
            });
        });
    </script>
</asp:Content>
