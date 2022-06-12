<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Feedback.aspx.cs" Inherits="CMS.Web.ClientArea.Feedback" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>Feedback</title>
    <link rel="icon" href="<%= Page.ResolveUrl("~/ClientArea/images/capture_EbL_icon.ico") %>">
    <!-- Latest compiled and minified CSS -->
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
    <!-- Optional theme -->
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap-theme.min.css">
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css">
    <link rel="stylesheet" href="<%= Page.ResolveUrl("~/ClientArea/jquery-ui-1.12.1/jquery-ui.theme.min.css") %>">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
    <script src="<%= Page.ResolveUrl("~/ClientArea/jquery-ui-1.12.1/datepicker-vi.js") %>"></script>
    <script src="<%= Page.ResolveUrl("~/ClientArea/jquery-ui-1.12.1/datepicker-en-GB.js") %>"></script>
    <link href="https://fonts.googleapis.com/css?family=Montserrat" rel="stylesheet">
    <link rel="stylesheet" href="<%= Page.ResolveUrl("~/ClientArea/FeedBack/form.css") %>">
    <script src="<%= Page.ResolveUrl("~/ClientArea/FeedBack/form.js") %>"></script>
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Dancing+Script&display=swap');
    </style>
    <script>
        // Check that service workers are supported
        if ('serviceWorker' in navigator) {
            // Use the window load event to keep the page load performant
            window.addEventListener('load', () => {
                navigator.serviceWorker.register('./sw.bundle.js');
            });
        }
    </script>
</head>
<body>
    <form role="form" runat="server" id="form1">
        <asp:ScriptManager ID="script1" runat="server"></asp:ScriptManager>
        <div class="container">
            <div class="imagebg"></div>
            <div class="row " style="margin-top: 50px">
                <div class="col-md-8 col-md-offset-2 form-container" style="min-height: calc(100vh - 100px); position: relative">
                    <img src="https://www.orientalsails.com/wp-content/themes/orientalsails2/img/logo.png" alt="ORIENTAL SAILS- YOUR BEST CHOICE FOR HALONG BAY CRUISES" style="margin: 0 auto; display: block" />
                    <div id="lang">
                        <div class="row-xGrid-yMiddle" style="transform: translate(-50%, -50%); -webkit-transform: translate(-50%, -50%); -moz-transform: translate(-50%, -50%); -o-transform: translate(-50%, -50%); -ms-transform: translate(-50%, -50%); position: absolute; top: 50%; left: 50%; width: 100%;">
                            <h5><i>Xin hãy lựa chọn ngôn ngữ / Please choose your language</i></h5>
                            <div class="row-xGrid iso-standard">
                                <button runat="server" type="submit" id="btnTiengViet" onserverclick="btnTiengViet_Click" class="btn-lang ctrl-standard typ-subhed fx-sliderIn">Tiếng Việt</button>
                            </div>
                            <div class="row-xGrid iso-standard">
                                <button runat="server" type="submit" id="btnTiengAnh" onserverclick="btnTiengAnh_Click" class="btn-lang ctrl-standard typ-subhed fx-sliderIn">English</button>
                            </div>
                        </div>
                    </div>
                    <asp:UpdatePanel runat="server" ID="upMain" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btnTiengViet" />
                            <asp:PostBackTrigger ControlID="btnTiengAnh" />
                            <asp:AsyncPostBackTrigger ControlID="btnSubmit" EventName="Click" />
                        </Triggers>
                        <ContentTemplate>
                            <asp:HiddenField runat="server" ID="hidLang" />
                            <asp:Panel runat="server" ID="pnlMain" Visible="false">
                                <div id="content">
                                    <%= Resources.Resource.Feedback_Address_Phone_Email %>
                                    <asp:Panel ID="pnlFeedback" runat="server">
                                        <%= Resources.Resource.Feedback_ThankYou %>
                                        <table class="table table-striped">
                                            <tr>
                                                <td style="width: 40%">
                                                    <label><%= Resources.Resource.Feedback_YourName %></label></td>
                                                <td style="width: 60%">
                                                    <div class="input-group-sm">
                                                        <asp:TextBox runat="server" ID="txtName" CssClass="form-control" autocomplete="off" />
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <label class="required"><%= Resources.Resource.Feedback_PhoneNumber %></label></td>
                                                <td>
                                                    <div class="input-group-sm">
                                                        <asp:TextBox runat="server" ID="txtPN" CssClass="form-control" MaxLength="11" autocomplete="off" onkeypress="return isNumberKey(event)" required="required" oninvalid="validatePhoneNumber(this)" oninput="validatePhoneNumber(this)" />
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <label class="required"><%= Resources.Resource.Feedback_StartDate %></label></td>
                                                <td>
                                                    <div class="input-group-sm">
                                                        <asp:TextBox runat="server" ID="txtStartDate" CssClass="form-control" autocomplete="off" required="required" oninvalid="validateStartDate(this)" onchange="validateStartDate(this)" oninput="validateStartDate(this)" />
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <label class="required"><%= Resources.Resource.Feedback_Cruise %></label></td>
                                                <td>
                                                    <div class="input-group-sm">
                                                        <asp:DropDownList runat="server" ID="ddlCruises" CssClass="form-control" AppendDataBoundItems="true" AutoPostBack=" true" OnTextChanged="ddlCruises_TextChanged" required="required" oninvalid="validateCruise(this)" onchange="validateCruise(this)">
                                                        </asp:DropDownList>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <label class="required"><%= Resources.Resource.Feedback_Room %></label></td>
                                                <td>
                                                    <asp:UpdatePanel ID="upRoom" runat="server" UpdateMode="Conditional">
                                                        <Triggers>
                                                            <asp:AsyncPostBackTrigger ControlID="ddlCruises" EventName="TextChanged" />
                                                        </Triggers>
                                                        <ContentTemplate>
                                                            <div class="input-group-sm">
                                                                <asp:DropDownList runat="server" ID="ddlRoom" CssClass="form-control" AppendDataBoundItems="true" required="required" oninvalid="validateRoom(this)" onchange="validateRoom(this)">
                                                                </asp:DropDownList>
                                                            </div>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:Repeater ID="rptGroups" runat="server" OnItemDataBound="rptGroups_ItemDataBound">
                                            <ItemTemplate>
                                                <div class="row">
                                                    <div class="col-sm-12 form-group">
                                                        <asp:HiddenField ID="hiddenId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "Id") %>' />
                                                        <label style="text-transform: uppercase">
                                                            <asp:Literal ID="litGroupName" runat="server"></asp:Literal></label>
                                                        <table style="width: 100%">
                                                            <tr>
                                                                <td></td>
                                                                <asp:Repeater ID="rptOptions" runat="server">
                                                                    <ItemTemplate>
                                                                        <td class="text-center">
                                                                            <%#DataBinder.Eval(Container, "DataItem") %>
                                                                        </td>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </tr>
                                                            <asp:Repeater ID="rptQuestions" runat="server" OnItemDataBound="rptQuestion_ItemDataBound">
                                                                <ItemTemplate>
                                                                    <tr>
                                                                        <td style="padding-right: 20px;">
                                                                            <asp:HiddenField ID="hiddenId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "Id") %>' />
                                                                            <asp:Literal ID="litQuestion" runat="server"></asp:Literal>
                                                                        </td>
                                                                        <asp:RadioButtonList runat="server" ID="radOptions" />
                                                                        <asp:Repeater ID="rptOptions" runat="server" OnItemDataBound="rptOptions_ItemDataBound">
                                                                            <ItemTemplate>
                                                                                <td style="width: 100px;">
                                                                                    <label style="width: 100%; cursor: pointer">
                                                                                        <asp:RadioButton runat="server" ID="radOption" CssClass="checkbox w100P" Text="" />
                                                                                    </label>
                                                                                </td>
                                                                            </ItemTemplate>
                                                                        </asp:Repeater>
                                                                    </tr>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                            <tr>
                                                                <td></td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="100">
                                                                    <span><%= Resources.Resource.Feedback_IsThereAnyThingYouWouldLikeToTellUs %> ?</span>
                                                                    <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        <div class="row">
                                            <div class="col-sm-12 form-group">
                                                <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-lg btn-warning btn-block" OnClick="btnSubmit_Click"></asp:Button>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </div>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdatePanel runat="server" ID="udpThank" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnSubmit" />
                        </Triggers>
                        <ContentTemplate>
                            <asp:Panel ID="pnlThank" runat="server" Visible="false">
                                <div style="transform: translate(-50%, -50%); -webkit-transform: translate(-50%, -50%); -moz-transform: translate(-50%, -50%); -o-transform: translate(-50%, -50%); -ms-transform: translate(-50%, -50%); position: absolute; top: 50%; left: 50%; width: 100%; text-align: center">
                                    <img src="https://encrypted-tbn0.gstatic.com/images?q=tbn%3AANd9GcRsZn3aBMosAkVEcLSkBvXYbc2KEEUzeYRp8w&usqp=CAU" style="width: 28em;">
                                    <p style="font-family: 'Dancing Script', cursive; font-weight: 700; margin-top: 1em; color: #008200; font-size: 3em">
                                        <%=Resources.Resource.Feedback_ThankYouForYourFeedback %> &nbsp; <span style="font-size: 0.5em">❤️</span>
                                    </p>
                                </div>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
        <div id="globalErrorMessage" class="alert alert-danger" role="alert" style="display: none">
            <strong>Error!</strong>
            <span id="message"></span>
        </div>
        <div id="loading" class="loading" style="display:none">
            <div class="spinner">
                <div class="rect1"></div>
                <div class="rect2"></div>
                <div class="rect3"></div>
                <div class="rect4"></div>
                <div class="rect5"></div>
            </div>
        </div>
    </form>
    <script>
        function SetUniqueRadioButton(nameregex, current) {
            for (i = 0; i < document.forms[0].elements.length; i++) {
                elm = document.forms[0].elements[i];
                if (elm.type == 'radio') {
                    if (elm.name.substring(elm.name.lastIndexOf('$')) == nameregex)
                        elm.checked = false;
                }
            }
            current.checked = true;
        }
    </script>
    <script>
        function pageLoad() {
            //Setup ngon ngu cho calendar
            $.datepicker.setDefaults($.datepicker.regional['<%= Session["lang"].ToString() %>']);
            $('#<%=txtStartDate.ClientID %>').datepicker();
        }

        if (typeof (Sys) != "undefined") {
            Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(initRequestHandler);
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandler);
        }

        function initRequestHandler(sender, args) {
            if (args.get_postBackElement().id == '<%= ddlCruises.ClientID%>') {
                return;
            }

            $("#loading").show();
        }

        function endRequestHandler(sender, args) {
            $("#loading").hide();
        }
    </script>
    <script>
        $(function () {
            if ($("#<%=ddlCruises.ClientID%>").prop("selectedIndex") == 0) {
                $('#<%=ddlRoom.ClientID%>').append(`<option><%= Resources.Resource.Feedback_PleaseChooseCruiseFirst%></option>`)
            }
        });
        $('#<%=ddlCruises.ClientID%>').change(function () {
            $('#<%=ddlRoom.ClientID%>').empty();
            if ($("#<%=ddlCruises.ClientID%>").prop("selectedIndex") == 0) {
                $('#<%=ddlRoom.ClientID%>').append(`<option><%= Resources.Resource.Feedback_PleaseChooseCruiseFirst%></option>`)
            } else {
                $('#<%=ddlRoom.ClientID%>').append(`<option>Loading...</option>`)
            }
        })
    </script>
    <script>
        //Hien thi loi validate
        function validatePhoneNumber(input) {
            if (input.value == '') {
                input.setCustomValidity('<%= Resources.Resource.Feedback_PleaseInputYourPhoneNumber%>');
            } else {
                input.setCustomValidity('');
            }

            return true;
        }

        function validateStartDate(input) {
            if (input.value == '') {
                input.setCustomValidity('<%= Resources.Resource.Feedback_PleaseChooseStartDate%>');
            } else {
                input.setCustomValidity('');
            }

            return true;
        }

        function validateCruise(input) {
            if (input.value == '') {
                input.setCustomValidity('<%= Resources.Resource.Feedback_PleaseChooseCruise%>');
            } else {
                input.setCustomValidity('');
            }

            return true;
        }

        function validateRoom(input) {
            if (input.value == '') {
                input.setCustomValidity('<%= Resources.Resource.Feedback_PleaseChooseRoom%>');
            } else {
                input.setCustomValidity('');
            }

            return true;
        }
    </script>
    <script>
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
    </script>
</body>
</html>

