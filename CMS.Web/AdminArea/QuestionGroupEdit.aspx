<%@ Page Language="C#" MasterPageFile="Main.Master" AutoEventWireup="true"
    CodeBehind="QuestionGroupEdit.aspx.cs" Inherits="CMS.Web.AdminArea.QuestionGroupEdit"
    Title="Question Group Adding" ValidateRequest="false" %>

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
                            Độ ưu tiên
                        </div>
                        <div class="col-sm-1">
                            <asp:TextBox runat="server" ID="txtPriority" CssClass="form-control form-control-sm" placeholder="Độ ưu tiên"></asp:TextBox>
                        </div>
                        <div class="col-sm-1">Chủ đề</div>
                        <div class="col-sm-5">
                            <asp:TextBox ID="txtSubject" runat="server" CssClass="form-control form-control-sm" placeholder="Chủ đề"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-sm-1">
                            Loại feedback
                        </div>
                        <div class="col-sm-2">
                            <asp:DropDownList ID="ddlLoaiFeedback" runat="server" CssClass="form-control form-control-sm">
                            </asp:DropDownList>
                        </div>
                        <div class="col-sm-3">
                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" runat="server" id="chkIsInDayboatForm">
                                <label class="form-check-label" for="<%= chkIsInDayboatForm.ClientID %>">Thuộc feedback dayboat</label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-sm-12 col-form-label">Những lựa chọn:</label>
                    </div>
                    <div class="form-group row">
                        <div class="col-sm-1">Lựa chọn 1</div>
                        <div class="col-sm-2">
                            <asp:TextBox runat="server" ID="txtSelection1" CssClass="form-control form-control-sm" placeholder="Lựa chọn 1"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-sm-1">Lựa chọn 2</div>
                        <div class="col-sm-2">
                            <asp:TextBox runat="server" ID="txtSelection2" CssClass="form-control form-control-sm" placeholder="Lựa chọn 2"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-sm-1">Lựa chọn 3</div>
                        <div class="col-sm-2">
                            <asp:TextBox runat="server" ID="txtSelection3" CssClass="form-control form-control-sm" placeholder="Lựa chọn 3"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-sm-1">Lựa chọn 4</div>
                        <div class="col-sm-2">
                            <asp:TextBox runat="server" ID="txtSelection4" CssClass="form-control form-control-sm" placeholder="Lựa chọn 4"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-sm-1">Lựa chọn 5</div>
                        <div class="col-sm-2">
                            <asp:TextBox runat="server" ID="txtSelection5" CssClass="form-control form-control-sm" placeholder="Lựa chọn 5"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group row">
                        <div class="col-sm-1">Lựa chọn tốt nhất</div>
                        <div class="col-sm-2">
                            <asp:DropDownList runat="server" ID="ddlGoodChoice" CssClass="form-control form-control-sm">
                                <asp:ListItem Value="1">Lựa chọn 1</asp:ListItem>
                                <asp:ListItem Value="2">Lựa chọn 2</asp:ListItem>
                                <asp:ListItem Value="3">Lựa chọn 3</asp:ListItem>
                                <asp:ListItem Value="4">Lựa chọn 4</asp:ListItem>
                                <asp:ListItem Value="5">Lựa chọn 5</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group row">
                        <label class="col-sm-12 col-form-label">
                            Câu hỏi con
                        </label>
                    </div>
                    <asp:Repeater ID="rptSubCategory" runat="server" OnItemDataBound="rptSubCategory_ItemDataBound">
                        <ItemTemplate>
                            <div class="form-group row">
                                <asp:HiddenField ID="hiddenId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "Id") %>' />
                                <div class="col-sm-1">
                                    Câu hỏi
                                </div>
                                <div class="col-sm-2">
                                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control form-control-sm" placeholder="Câu hỏi"></asp:TextBox>
                                </div>
                                <div class="col-sm-1">
                                    Mô tả
                                </div>
                                <div class="col-sm-5">
                                    <asp:TextBox ID="txtContent" runat="server" CssClass="form-control form-control-sm" TextMode="MultiLine" placeholder="Mô tả"></asp:TextBox>
                                </div>
                                <div class="col-sm-2">
                                    <asp:Button ID="btnRemove" runat="server" CssClass="btn btn-danger btn-sm" Text="Remove" OnClick="btnRemove_Click" />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <div class="form-group row">
                        <div class="col-xs-12">
                            <asp:Button ID="btnAddSubCategory" runat="server" CssClass="btn btn-success btn-sm" Text="Thêm câu hỏi con" OnClick="btnAddSubCategory_Click" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="row">
                            <div class="col-xs-12">
                                <asp:Button ID="btnSave" runat="server" CssClass="btn btn-success btn-sm" Text="Lưu" OnClick="btnSave_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="form-group">
        <div class="row">
            <div class="col-xs-11">
            </div>
        </div>
    </div>

</asp:Content>
