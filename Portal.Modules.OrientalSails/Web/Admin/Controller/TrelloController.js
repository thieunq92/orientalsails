moduleTrello.controller("trelloController", [
    "$rootScope",
    "$scope",
    "$http",
    "$compile",
    "trelloService",
    function ($rootScope, $scope, $http, $compile, trelloService) {
        $scope.trelloService = trelloService;
        $scope.trelloService.setShowBoard(true);
        $scope.trelloService.setShowList(false);
        $scope.trelloService.setSelectedBoard({});
        $scope.trelloService.setDanhSachList([]);
        $scope.showButtonThemMoiDanhSach = true;
        $scope.showFormThemMoiDanhSach = false;
        $scope.tieuDeDanhSach = "";
        $scope.showFormThemMoiThe = [];
        $scope.showButtonThemMoiThe = [];
        $scope.tieuDeThe = [];
        let scope = angular.element($("#trello-controller")).scope();
        //authenticate trello
        var authenticationSuccess = function () {
            $scope.showBoard = true;
            console.log("Successful authentication");
        };

        var authenticationFailure = function () {
            console.log("Failed authentication");
        };

        window.Trello.authorize({
            type: "popup",
            name: "Getting Started Application",
            scope: {
                read: "true",
                write: "true",
            },
            expiration: "never",
            success: authenticationSuccess,
            error: authenticationFailure,
        });

        //Liet ke danh sach cac board
        let container = document.getElementById("board-tile-component");
        let scriptSrc = window.customElements
            ? "/board-tile.min.js"
            : "board-tile-polyfilled.min.js";
        let boardTileJs = document.createElement("script");
        boardTileJs.crossOrigin = "anonymous";
        boardTileJs.src = "https://p.trellocdn.com" + scriptSrc;
        boardTileJs.onload = function () {
            $("#board-tile-component").empty();
            window.Trello.members.get(
                "me/boards",
                {},
                function (boards) {
                    container.innerHTML = "";
                    for (let i = 0; i < boards.length; i++) {
                        const boardTileEl = document.createElement("trello-board-tile");
                        boardTileEl.board = boards[i];
                        container.appendChild(boardTileEl);
                        let style = document.createElement("style");
                        style.innerHTML =
                            ".board-background {width: 100% !important; height:96px; border-radius:3px} .board-tile-container {width: 100% !important;} .logo-footer{display:none}";
                        boardTileEl.shadowRoot.appendChild(style);
                        //Loai bo duong dan cua board
                        boardTileEl.addEventListener("click", (e) => {
                            e.preventDefault();
                            scope.$apply(function () {
                                scope.trelloService.setShowBoard(false);
                                scope.trelloService.setShowList(true);
                                scope.trelloService.selectedBoard = boards[i];
                            });
                            //Load danh sach list
                            window.Trello.boards.get(
                                boards[i].id + "/lists",
                                {},
                                function (lists) {
                                    scope.$apply(function () {
                                        scope.trelloService.setDanhSachList(lists);
                                    });
                                },
                                function () {

                                }
                            );
                        });
                    }
                    var themBangMoiButton =
                        "<div class='boards-page-board-section-list-item'>";
                    themBangMoiButton = themBangMoiButton.concat(
                        "<div ng-click='openAddBoardDialog()' class='board-tile mod-add'>",
                        "<p><span>Tạo bảng mới</span></p>",
                        "</div>"
                    );
                    themBangMoiButton = themBangMoiButton.concat("</div>");
                    $("#board-tile-component").append(themBangMoiButton);
                    $("#board-tile-component").remove(".rounded-bottom.logo-footer");
                    $compile($("#board-tile-component"))($scope);
                },
                null
            );
        };

        document.head.appendChild(boardTileJs);

        $scope.openAddBoardDialog = function () {
            $("#addBoardModal").modal();
        };

        $scope.dongBang = function () {
            window.Trello.delete("/boards/" + $scope.trelloService.selectedBoard.id);
        };

        $scope.onNutThemMoiDanhSachClick = function () {
            $scope.showFormThemMoiDanhSach = true;
            $scope.showButtonThemMoiDanhSach = false;
        };

        $scope.onNutCloseFormClick = function () {
            $scope.showFormThemMoiDanhSach = false;
            $scope.showButtonThemMoiDanhSach = true;
        };

        $scope.themDanhSach = function () {
            window.Trello.post(
                "/lists/",
                { name: $scope.tieuDeDanhSach, idBoard: $scope.trelloService.selectedBoard.id },
                function () {
                    window.Trello.boards.get(
                        $scope.trelloService.selectedBoard.id + "/lists",
                        {},
                        function (lists) {
                            //Load lai danh sach list sau khi them danh sach
                            $scope.tieuDeDanhSach = "";
                            $scope.trelloService.setDanhSachList(lists);
                            $scope.showFormThemMoiDanhSach = false;
                            $scope.showButtonThemMoiDanhSach = true;
                            $scope.$apply();
                        },
                        function () {

                        }
                    );
                },
                function () { }
            );
        };

        $scope.onDanhSachListInit = function ($index) {
            $scope.showFormThemMoiThe[$index] = false;
            $scope.showButtonThemMoiThe[$index] = true;
        };

        $scope.onNutThemTheClick = function ($index) {
            $scope.showFormThemMoiThe[$index] = true;
            $scope.showButtonThemMoiThe[$index] = false;
        };

        $scope.onDongThemTheClick = function ($index) {
            $scope.showFormThemMoiThe[$index] = false;
            $scope.showButtonThemMoiThe[$index] = true;
        };

        $scope.onNutThemTheSubmitClick = function ($index, idList) {
            window.Trello.post(
                "/cards/",
                { name: $scope.tieuDeThe[$index], idList: idList },
                function () {
                    //Load lai danh sach the sau khi them the
                    $scope.tieuDeThe[$index] = "";
                    $scope.showButtonThemMoiThe[$index] = true;
                    $scope.showFormThemMoiThe[$index] = false;
                    $scope.$apply();
                    window.Trello.lists.get(
                        idList + "/cards",
                        {},
                        function (cards) {
                            let containerCards = $("#card-component-example").get($index);
                            containerCards.innerHTML = "";
                            for (let i = 0; i < cards.length; i++) {
                                const cardEl = document.createElement("trello-card");
                                cardEl.card = cards[i];
                                let style = document.createElement("style");
                                style.innerHTML =
                                    ".card {margin-bottom: 8px!important;} .card.show-label-text .card-label { float: left; font-size: 12px; font-weight: 700; height: 8px; line-height: 100px; margin: 0 4px 4px 0; max-width: 40px; min-width: 40px; padding: 0; text-shadow: none; width: auto; }";
                                cardEl.shadowRoot.appendChild(style);
                                containerCards.append(cardEl);
                            }
                        },
                        function () {
                        }
                    );
                },
                null
            );
        };
    },
]);

moduleTrello.controller("taoBangController", [
    "$rootScope",
    "$scope",
    "$http",
    "$compile",
    "trelloService",
    function ($rootScope, $scope, $http, $compile, trelloService) {
        $scope.trelloService = trelloService;
        $scope.themBang = function () {
            window.Trello.post(
                "/boards/",
                { name: $scope.tieuDeBang },
                //success
                function (board) {
                    $("#addBoardModal").modal("hide");
                    $scope.trelloService.setShowBoard(false);
                    $scope.trelloService.setShowList(true);
                    $scope.trelloService.setSelectedBoard(board);
                    $scope.$apply();
                    window.Trello.boards.get(
                        $scope.trelloService.selectedBoard.id + "/lists",
                        {},
                        function (lists) {
                            //Load lai danh sach list sau khi them bang
                            $scope.trelloService.setDanhSachList(lists);
                            $scope.$apply();
                        },
                        function () {

                        }
                    );
                },
                null
            );
        };
    },
]);

moduleTrello.service("trelloService", [
    function () {
        let obj = {};
        obj.setShowBoard = function (value) {
            obj.showBoard = value;
        };

        obj.getShowBoard = function () {
            return obj.showBoard;
        };

        obj.setShowList = function (value) {
            obj.showList = value;
        };

        obj.getShowList = function () {
            return obj.showList;
        }

        obj.setSelectedBoard = function (value) {
            obj.selectedBoard = value;
        }

        obj.getSelectedBoard = function () {
            return obj.selectedBoard;
        }

        obj.setDanhSachList = function (value) {
            obj.danhSachList = value;
        }

        obj.getDanhSachList = function () {
            return obj.danhSachList;
        }

        return obj;
    },
]);

moduleTrello.directive("loadCards", function () {
    return function (scope, element, attrs) {
        //Load danh sach card
        let containerCards = $(angular.element(element)).find(
            "#card-component-example"
        );
        scriptSrc = window.customElements
            ? "/card.min.js"
            : "/card-polyfilled.min.js";
        let cardJs = document.createElement("script");
        cardJs.crossOrigin = "anonymous";
        cardJs.src = "https://p.trellocdn.com" + scriptSrc;
        cardJs.onload = function () {
            window.Trello.lists.get(
                scope.trelloService.danhSachList[scope.$index].id + "/cards",
                {},
                function (cards) {
                    containerCards.innerHTML = "";
                    for (let i = 0; i < cards.length; i++) {
                        const cardEl = document.createElement("trello-card");
                        cardEl.card = cards[i];
                        let style = document.createElement("style");
                        style.innerHTML =
                            ".card {margin-bottom: 8px!important;} .card.show-label-text .card-label { float: left; font-size: 12px; font-weight: 700; height: 8px; line-height: 100px; margin: 0 4px 4px 0; max-width: 40px; min-width: 40px; padding: 0; text-shadow: none; width: auto; }";
                        cardEl.shadowRoot.appendChild(style);
                        containerCards.append(cardEl);
                    }
                },
                function () {
                }
            );
        };

        document.head.appendChild(cardJs);
    };
});
