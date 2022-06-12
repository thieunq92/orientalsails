<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Trello.aspx.cs" MasterPageFile="MO.Master" Inherits="Portal.Modules.OrientalSails.Web.Admin.Trello" %>

<asp:Content ID="Head" ContentPlaceHolderID="Head" runat="server">
    <style>
        trello-board-tile {
            display: inline-block;
            width: 194px;
            padding: 0;
            margin: 0 1% 1% 0;
            transform: translate(0);
        }

        .trello-list-bgcolor {
            background-color: rgb(0, 121, 191);
        }
        
        .navbar {
            margin-bottom: 0;
        }
    </style>
    <title>Trello</title>
</asp:Content>
<asp:Content ID="AdminContent" ContentPlaceHolderID="AdminContent" runat="server">
    <div ng-controller="trelloController" ng-class="trelloService.getShowList() ? 'trello-list-bgcolor' : ''" id="trello-controller" class="trello" style="width: auto; overflow-x: auto; height: calc(100vh - 161px); padding-top: 0">
        <div ng-show="trelloService.getShowBoard()" class="panel-board"  style="padding: 10px 0 0 10px">
            <div id="board-tile-component">
                <!-- Board Tile Component Will Be Inserted Here -->
            </div>
        </div>
        <div ng-show="trelloService.getShowList()" class="panel-list">
            <div class="board-header u-clearfix js-board-header" style="height: 47px">
                <div class="board-header-btn mod-board-name inline-rename-board js-rename-board">
                    <h1 class="js-board-editing-target board-header-btn-text" dir="auto" style="margin: 0">{{trelloService.selectedBoard.name}}</h1>
                    <input class="board-name-input js-board-name-input hidden" spellcheck="false" dir="auto" maxlength="512" value="">
                </div>
                <div class="js-board-header-btn-org-wrapper board-header-btn-org-wrapper">
                    <div class="board-header-btns mod-left">
                        <span class="board-header-btn-divider"></span>
                        <span id="workspaces-preamble-board-header-button">
                            <div class="js-react-root">
                                <a ng-click="dongBang()" class="board-header-btn board-header-btn-without-icon js-add-board-to-team" href="javascript:void(0)">
                                    <span class="board-header-btn-text">Đóng bảng</span>
                                </a>
                            </div>
                        </span>
                    </div>

                </div>
            </div>
            <div style="white-space: nowrap; display: flex">
                <div load-cards ng-repeat="list in trelloService.getDanhSachList()" ng-init="onDanhSachListInit($index)" style="display: inline-block;">
                    <div style="width: 272px; background-color: #dfe1e6; padding: 8px; border-radius: 3px; float: left; white-space: normal; margin: 0 4px; padding: 7px 7px 0 7px;">
                        <span class="mod-list-name">{{list.name}}</span>
                        <div id="card-component-example" style="overflow-y: auto;max-height: calc(100vh - 298px);">
                            <!-- Card Component Will Be Inserted Here -->
                        </div>
                        <div ng-show="showFormThemMoiThe[$index]" class="card-composer">
                            <div class="list-card js-composer">
                                <div class="list-card-details u-clearfix">
                                    <div class="list-card-labels u-clearfix js-list-card-composer-labels"></div>
                                    <textarea ng-model="tieuDeThe[$index]" class="list-card-composer-textarea js-card-title" dir="auto" placeholder="Nhập tiêu đề cho thẻ này…" style="overflow: hidden; overflow-wrap: break-word; height: 40px;"></textarea>
                                    <div class="list-card-members js-list-card-composer-members"></div>
                                </div>
                            </div>
                            <div class="cc-controls u-clearfix">
                                <div class="cc-controls-section" style="display:flex">
                                    <input ng-click="onNutThemTheSubmitClick($index, list.id)" class="primary confirm mod-compact js-add-card" style="padding: 6px 12px; display: inline-block; margin: 0;"
                                        type="button" value="Thêm thẻ">
                                    <a ng-click="onDongThemTheClick($index)" class="icon-lg dark-hover js-cancel" href="javascript:void(0)"><i class="fas fa-times"></i></a>
                                </div>
                            </div>
                        </div>
                        <div ng-show="showButtonThemMoiThe[$index]" class="card-composer-container js-card-composer-container">
                            <a ng-click="onNutThemTheClick($index)" class="open-card-composer js-open-card-composer" href="#" style="margin: 2px 0 8px 0">
                                <i class="fas fa-plus icon-sm icon-add"></i>
                                <span class="js-add-a-card">Thêm một thẻ</span>
                            </a>
                        </div>
                    </div>
                </div>
                <div ng-class="showButtonThemMoiDanhSach ? 'is-idle' : ''" class="js-add-list list-wrapper mod-add " style="height: max-content; min-width: 272px">
                    <a ng-show="showButtonThemMoiDanhSach" ng-click="onNutThemMoiDanhSachClick()" class="open-add-list js-open-add-list" href="javascript:void(0)">
                        <span class="placeholder">
                            <i class="fas fa-plus icon-sm icon-add"></i>Tạo thêm danh sách</span>
                    </a>
                    <div ng-show="showFormThemMoiDanhSach">
                        <input ng-model="tieuDeDanhSach" class="list-name-input" type="text" name="name" placeholder="Nhập tiêu đề danh sách..." autocomplete="off" dir="auto" maxlength="512">
                        <div class="list-add-controls u-clearfix">
                            <input ng-click="themDanhSach()" class="primary mod-list-add-button js-save-edit" type="button" value="Thêm danh sách" style="display: inline-block">
                            <a ng-click="onNutCloseFormClick()" class="icon-lg dark-hover js-cancel-edit" href="javascript:void(0)">
                                <i class="fas fa-times"></i>
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div ng-controller="taoBangController">
        <div class="modal fade trello" id="addBoardModal" tabindex="-1" role="dialog" aria-labelledby="gridSystemModalLabel" data-backdrop="dynamic" data-keyboard="true">
            <div class="modal-dialog" role="document" style="width: 25vw; height: 19vh">
                <div class="modal-content" style="background-color: transparent; box-shadow: none; border: none;">
                    <div class="modal-body">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"></button>
                        <div class="board-tile create-board-tile" style="background-color: rgb(0, 121, 191);">
                            <div>
                                <input ng-model="tieuDeBang" placeholder="Thêm tiêu đề bảng" class="subtle-input" value="">
                            </div>
                        </div>
                        <div class="action-items">
                            <button ng-click="themBang()" class="button primary" type="button">
                                <span class="logo-loading subtle"></span>
                                <span>Tạo bảng</span>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Scripts" ContentPlaceHolderID="Scripts" runat="server">
    <script type="text/javascript" src="/modules/sails/admin/controller/trellocontroller.js"></script>
</asp:Content>
