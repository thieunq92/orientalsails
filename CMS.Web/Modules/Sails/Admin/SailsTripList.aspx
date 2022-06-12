<%@ Page Language="C#" MasterPageFile="MO.Master" AutoEventWireup="true" CodeBehind="SailsTripList.aspx.cs" Inherits="Portal.Modules.OrientalSails.Web.Admin.SailsTripList" Title="Trip Management" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="page-header">
        <h3>
            <asp:Label ID="titleSailsTripList" runat="server"></asp:Label></h3>
    </div>
    <div class="row">
        <div class="col-xs-12">
            <table class="table table-bordered table-hover table-common">
                <asp:Repeater ID="rptTripList" runat="server" OnItemDataBound="rptTripList_ItemDataBound" OnItemCommand="rptTripList_ItemCommand">
                    <headertemplate>
                            <tr class="active">
                                <th>
                                    Name
                                </th>
                                <th>
                                    Number of day
                                </th>
                                <th>
                                    Number of option
                                </th>
                                <th>
                                    Price
                                </th>
                                <th>Facilitie</th>
                                <th>Document</th>
                                <th>Images</th>
                                <th>Reviews</th>
                                <th>                                 
                                </th>
                            </tr>
                        </headertemplate>
                    <itemtemplate>
                            <tr class="item">
                                <td>
                                    <asp:HyperLink ID="hyperLink_Name" runat="server"></asp:HyperLink>                                
                                </td>
                                <td>
                                    <asp:Label ID="label_NumberOfDays" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="label_NumberofOption" runat="server"></asp:Label>
                                </td>      
                                <td>
                                    <table style="width:auto;">                                    
                                    <asp:Repeater ID="rptOptions" runat="server" OnItemDataBound="rptOptions_ItemDataBound">
                                        <ItemTemplate>
                                            <tr>
                                                <td><asp:Literal ID="litOption" runat="server"></asp:Literal></td>
                                                <asp:Repeater ID="rptCruises" runat="server" OnItemDataBound="rptCruises_ItemDataBound">
                                                    <ItemTemplate>
                                                        <td><asp:HyperLink ID="hplCruise" runat="server"></asp:HyperLink></td>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    </table>
                                    <!--<asp:DropDownList ID="ddlOption" runat="server"></asp:DropDownList>
                                    <asp:ImageButton ID="imageButtonPrice" runat="server" ToolTip="Price" AlternateText="Price" ImageAlign="AbsMiddle" CssClass="image_button16" CommandName="Price" ImageUrl="../Images/price.gif" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"Id") %>'/>-->
                                </td>     
                                <td>
                                    <a href="javascript:;" onclick='editfacilitie("<%#Eval("Id") %>")'>Facilitie</a>
                                </td>
                                <td>
                                    <a href="javascript:;" onclick='editDocument("<%#Eval("Id") %>")'>Document</a>
                                </td>
                                <td>
                                    <a href="javascript:;" onclick='editImages("<%#Eval("Id") %>")'>Images</a>
                                </td>
                                <td>
                                    <a href="javascript:;" onclick='editReviews("<%#Eval("Id") %>")'>Reviews</a>
                                </td>                     
                                <td>
                                    <asp:HyperLink ID="hyperLinkEdit" runat="server">
                                        <i class="fa fa-edit fa-lg" aria-hidden="true" data-toggle="tooltip" data-placement="top" title="" data-original-title="Edit"></i>
                                    </asp:HyperLink>
                                    <asp:LinkButton runat="server" ID="imageButtonDelete" CommandName="Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"Id") %>' OnClientClick="javascript: return confirm('Are you sure?')">
                                        <i class="fa fa-window-close fa-lg text-danger" aria-hidden="true" title="" data-toggle="tooltip" data-placement="top" data-original-title="Delete"></i>
                                    </asp:LinkButton>       
                                </td>
                            </tr>
                        </itemtemplate>
                </asp:Repeater>
            </table>
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
    <script>
        function editfacilitie(id) {
            var src = "/Modules/Sails/Admin/FacilitieMapList.aspx?NodeId=1&SectionId=15&type=TRIP&id=" + id;
            $("#osModal iframe").attr('src', src);
            $("#osModal").modal();
            return false;
        }
        function editDocument(id) {
            var src = "/Modules/Sails/Admin/DocumentMapList.aspx?NodeId=1&SectionId=15&type=TRIP&id=" + id;
            $("#osModal iframe").attr('src', src);
            $("#osModal").modal();
            return false;
        }
        function editImages(id) {
            var src = "/Modules/Sails/Admin/ImageMapList.aspx?NodeId=1&SectionId=15&type=TRIP&id=" + id;
            $("#osModal iframe").attr('src', src);
            $("#osModal").modal();
            return false;
        }
        function editReviews(id) {
            var src = "/Modules/Sails/Admin/ReviewMapList.aspx?NodeId=1&SectionId=15&type=TRIP&id=" + id;
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
