<%@ Control Language="c#" AutoEventWireup="false" Inherits="CMS.Web.UI.BaseTemplate" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>
        <asp:Literal ID="PageTitle" runat="server"></asp:Literal></title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="refresh" content="0; url=/Modules/Sails/Admin/Home.aspx?NodeId=1&SectionId=15">
    <asp:Literal ID="MetaTags" runat="server" />
    <asp:Literal ID="Stylesheets" runat="server" />
    <asp:Literal ID="JavaScripts" runat="server" />
</head>
<body>
    <form id="t" method="post" runat="server">
       <asp:PlaceHolder ID="maincontent" runat="server"></asp:PlaceHolder>
    </form>
</body>
</html>
