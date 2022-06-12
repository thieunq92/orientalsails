<%@ Page Language="C#" MasterPageFile="Main.Master" AutoEventWireup="true"
    CodeBehind="QuestionView.aspx.cs" Inherits="CMS.Web.AdminArea.QuestionView"
    Title="Question Group Overview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentHeader" runat="server">
    <div class="container-fluid">
        <div class="row mb-2">
            <div class="col-sm-6">
                <h1>Nhóm câu hỏi</h1>
            </div>
            <div class="col-sm-6">
                <ol class="breadcrumb float-sm-right">
                    <li class="breadcrumb-item"><a href="#">Home</a></li>
                    <li class="breadcrumb-item active">Blank Page</li>
                </ol>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="content" runat="server">
    <div class="container-fluid">
        <div class="row">
            <div class="col-md-12">
                <div class="form-horizontal">
                    <div class="form-group row">
                        <div class="col-sm-1">
                            Loại feedback 
                        </div>
                        <div class="col-sm-2">
                            <asp:DropDownList ID="ddlLoaiFeedback" runat="server" CssClass="form-control form-control-sm" AutoPostBack="true">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <asp:Repeater ID="rptGroups" runat="server" OnItemDataBound="rptGroups_ItemDataBound">
                        <ItemTemplate>
                            <div class="form-group row">
                                <div class="col-sm-12">
                                    <asp:HiddenField ID="hiddenId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "QUESTION_GROUP_ID") %>' />
                                    <asp:Literal ID="litGroupName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "QUESTION_GROUP_NAME") %>'></asp:Literal>
                                    <asp:HyperLink ID="hplEdit" runat="server" CssClass="btn btn-success btn-sm">Sửa</asp:HyperLink>
                                    <asp:LinkButton ID="lbtDelete" runat="server" OnClick="lbtDelete_Click" CssClass="btn btn-danger btn-sm">Xóa</asp:LinkButton>
                                </div>
                            </div>
                            <ul style="list-style-type: none">
                                <asp:Repeater ID="rptQuestions" runat="server">
                                    <ItemTemplate>
                                        <li><%# DataBinder.Eval(Container.DataItem, "QUESTION_NAME") %></li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ul>
                        </ItemTemplate>
                    </asp:Repeater>
                    <div class="row">
                        <div class="col-sm-12">
                            <h4>Form dayboat</h4>
                        </div>
                    </div>
                    <asp:Repeater ID="rptDayboatGroup" runat="server" OnItemDataBound="rptGroups_ItemDataBound">
                        <ItemTemplate>
                            <div class="form-group row">
                                <div class="col-sm-12">
                                    <asp:HiddenField ID="hiddenId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "QUESTION_GROUP_ID") %>' />
                                    <asp:Literal ID="litGroupName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "QUESTION_GROUP_NAME") %>'></asp:Literal>
                                    <asp:HyperLink ID="hplEdit" runat="server" CssClass="btn btn-success btn-sm">Sửa</asp:HyperLink>
                                    <asp:LinkButton ID="lbtDelete" runat="server" OnClick="lbtDelete_Click" CssClass="btn btn-danger btn-sm">Xóa</asp:LinkButton>
                                </div>
                            </div>
                            <ul style="list-style-type: none">
                                <asp:Repeater ID="rptQuestions" runat="server">
                                    <ItemTemplate>
                                        <li><%# DataBinder.Eval(Container.DataItem, "QUESTION_NAME") %></li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ul>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
