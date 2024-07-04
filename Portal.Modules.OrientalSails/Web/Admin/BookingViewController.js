moduleBookingView.controller("bookingViewController", ["$rootScope", "$scope", "$http", "$location", "$filter", "$timeout"
    , function ($rootScope, $scope, $http, $location, $filter, $timeout) {
        $scope.calculateTotal = function () {
            let total = 0;
            for (i = 0; i < $scope.roomPriceSalesInputs.length; i++) {
                total += Number($scope.$eval(`txtNumberOfRoomsPrice${$scope.roomPriceSalesInputs[i].RoomClassId}${$scope.roomPriceSalesInputs[i].RoomTypeId}`).toString().replace(/\,/g, '')) * $scope.roomPriceSalesInputs[i].NumberOfRooms
                    + Number($scope.$eval(`txtNumberOfRoomsSinglePrice${$scope.roomPriceSalesInputs[i].RoomClassId}${$scope.roomPriceSalesInputs[i].RoomTypeId}`).toString().replace(/\,/g, '')) * $scope.roomPriceSalesInputs[i].NumberOfRoomsSingle
                    + Number($scope.$eval(`txtNumberOfAddAdultPrice${$scope.roomPriceSalesInputs[i].RoomClassId}${$scope.roomPriceSalesInputs[i].RoomTypeId}`).toString().replace(/\,/g, '')) * $scope.roomPriceSalesInputs[i].NumberOfAddAdult
                    + Number($scope.$eval(`txtNumberOfAddChildPrice${$scope.roomPriceSalesInputs[i].RoomClassId}${$scope.roomPriceSalesInputs[i].RoomTypeId}`).toString().replace(/\,/g, '')) * $scope.roomPriceSalesInputs[i].NumberOfAddChild
                    + Number($scope.$eval(`txtNumberOfAddBabyPrice${$scope.roomPriceSalesInputs[i].RoomClassId}${$scope.roomPriceSalesInputs[i].RoomTypeId}`).toString().replace(/\,/g, '')) * $scope.roomPriceSalesInputs[i].NumberOfAddBaby
                    + Number($scope.$eval(`txtNumberOfExtrabedPrice${$scope.roomPriceSalesInputs[i].RoomClassId}${$scope.roomPriceSalesInputs[i].RoomTypeId}`).toString().replace(/\,/g, '')) * $scope.roomPriceSalesInputs[i].NumberOfExtrabed
            }
            $scope.total = total;
        }

        $scope.calculateTotalSeatingCruise = function () {
            let total = 0;
            total = $scope.numberOfAdult * Number($scope.txtNumberOfAdultsPrice.toString().replace(/\,/g, ''))
                + $scope.numberOfChild * Number($scope.txtNumberOfChildsPrice.toString().replace(/\,/g, ''))
                + $scope.numberOfBaby * Number($scope.txtNumberOfBabysPrice.toString().replace(/\,/g, ''))

            $scope.total = total;
        }

        //Xử lý trích ngoái
        $scope.listCommission = []
        $scope.loadCommission = function () {
            $http({
                method: "POST",
                url: "WebMethod/BookingViewWebMethod.asmx/CommissionGetAllByBookingId",
                data: {
                    "bookingId": $scope.bookingId,
                },
            }).then(function (response) {
                $scope.listCommission = JSON.parse(response.data.d);
            }, function (response) {
            })
            $timeout(function () {
                $("[data-control='inputmask']").inputmask({
                    'alias': 'numeric',
                    'groupSeparator': ',',
                    'autoGroup': true,
                    'digits': 2,
                    'digitsOptional': true,
                    'placeholder': '0',
                    'rightAlign': false
                })
                $('input[type="text"]').keydown(function () {
                    $(this).trigger('input');
                    $(this).trigger('change');
                });
            }, 0);
        }

        $scope.addCommission = function () {
            $scope.listCommission.push({ id: -1, payFor: "", amount: 0, paymentVoucher: "", transfer: false, bookingId: $scope.bookingId })
            $timeout(function () {
                $("[data-control='inputmask']").inputmask({
                    'alias': 'numeric',
                    'groupSeparator': ',',
                    'autoGroup': true,
                    'digits': 2,
                    'digitsOptional': true,
                    'placeholder': '0',
                    'rightAlign': false
                })
                $('input[type="text"]').keydown(function () {
                    $(this).trigger('input');
                    $(this).trigger('change');
                });
            }, 0);
        }
        $scope.removeCommission = function (index) {
            $scope.listCommission.splice(index, 1);
        }
        $scope.calculateTotalCommission = function () {
            var total = 0;
            for (var i = 0; i < $scope.listCommission.length; i++) {
                var commission = $scope.listCommission[i];
                total += parseFloat(commission.amount.toString().replace(/,/g, ''));
            }
            $scope.totalCommission = total;
            return total.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "VND";
        }

        //XỬ lý dịch vụ ngoài
        $scope.listServiceOutside = []
        $scope.loadServiceOutside = function () {
            $http({
                method: "POST",
                url: "WebMethod/BookingViewWebMethod.asmx/ServiceOutsideGetAllByBookingId",
                data: {
                    "bookingId": $scope.bookingId,
                },
            }).then(function (response) {
                $scope.listServiceOutside = JSON.parse(response.data.d);
            }, function (response) {
            })
            $timeout(function () {
                $("[data-control='inputmask']").inputmask({
                    'alias': 'numeric',
                    'groupSeparator': ',',
                    'autoGroup': true,
                    'digits': 2,
                    'digitsOptional': true,
                    'placeholder': '0',
                    'rightAlign': false
                })
                $('input[type="text"]').keydown(function () {
                    $(this).trigger('input');
                    $(this).trigger('change');
                });
            }, 0);
        }
        var id = -1;
        $scope.addServiceOutside = function () {
            $scope.listServiceOutside.push({ id: id, service: "", unitPrice: 0, quantity: 0, totalPrice: 0, bookingId: $scope.bookingId, vat: $scope.bookingVAT, listServiceOutsideDetailDTO: [] })
            id += -1;
            $timeout(function () {
                $("[data-control='inputmask']").inputmask({
                    'alias': 'numeric',
                    'groupSeparator': ',',
                    'autoGroup': true,
                    'digits': 2,
                    'digitsOptional': true,
                    'placeholder': '0',
                    'rightAlign': false
                })
                $('input[type="text"]').keydown(function () {
                    $(this).trigger('input');
                    $(this).trigger('change');
                });
            }, 0);
        }
        $scope.removeServiceOutside = function (index) {
            $scope.listServiceOutside.splice(index, 1);
        }
        $scope.calculateServiceOutside = function (index, unitPrice, quantity) {
            $scope.listServiceOutside[index].totalPrice = parseFloat(unitPrice.replace(/,/g, '')) * parseInt(quantity);
            $scope.listServiceOutside[index].totalPrice = $scope.listServiceOutside[index].totalPrice.replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        }
        $scope.totalServiceOutside = 0;
        $scope.$watch('totalServiceOutside', function () {
            $scope.calculateTotalPrice();
        })
        $scope.calculateTotalServiceOutside = function () {
            var total = 0;
            for (var i = 0; i < $scope.listServiceOutside.length; i++) {
                var serviceOutside = $scope.listServiceOutside[i];
                total += parseFloat(serviceOutside.totalPrice.toString().replace(/,/g, ''));
            }
            $scope.totalServiceOutside = total;
            return total.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "VND";
        }
        //Xử lý chi tiết dịch vụ ngoài
        $scope.loadServiceOutsideDetail = function (serviceOutsideId) {
            $http({
                method: "POST",
                url: "WebMethod/BookingViewWebMethod.asmx/ServiceOutsideDetailGetAllByServiceOutsideId",
                data: {
                    "serviceOutsideId": serviceOutsideId,
                },
            }).then(function (response) {
                for (var i = 0; i < $scope.listServiceOutside.length; i++) {
                    if ($scope.listServiceOutside[i].id == serviceOutsideId) {
                        $scope.listServiceOutside[i].listServiceOutsideDetailDTO = JSON.parse(response.data.d);
                    }
                }
            }, function (response) {
            })
            $timeout(function () {
                $("[data-control='inputmask']").inputmask({
                    'alias': 'numeric',
                    'groupSeparator': ',',
                    'autoGroup': true,
                    'digits': 2,
                    'digitsOptional': true,
                    'placeholder': '0',
                    'rightAlign': false
                });
                $('input[type="text"]').keydown(function () {
                    $(this).trigger('input');
                    $(this).trigger('change');
                });
            }, 0);
        }
        $scope.addServiceOutsideDetail = function (serviceOutsideId) {
            for (var i = 0; i < $scope.listServiceOutside.length; i++) {
                if ($scope.listServiceOutside[i].id == serviceOutsideId) {
                    $scope.listServiceOutside[i].listServiceOutsideDetailDTO.push({ id: -1, name: "", unitPrice: 0, quantity: 0, totalPrice: 0 })
                    $timeout(function () {
                        $("[data-control='inputmask']").inputmask({
                            'alias': 'numeric',
                            'groupSeparator': ',',
                            'autoGroup': true,
                            'digits': 2,
                            'digitsOptional': true,
                            'placeholder': '0',
                            'rightAlign': false
                        });
                        $('input[type="text"]').keydown(function () {
                            $(this).trigger('input');
                            $(this).trigger('change');
                        });
                    }, 0);
                }
            }
        }
        $scope.removeServiceOutsideDetail = function (index, serviceOutsideId) {
            for (var i = 0; i < $scope.listServiceOutside.length; i++) {
                if ($scope.listServiceOutside[i].id == serviceOutsideId) {
                    $scope.listServiceOutside[i].listServiceOutsideDetailDTO.splice(index, 1);
                }
            }

        }
        $scope.calculateServiceOutsideDetail = function (serviceOutside, index, unitPrice, quantity) {
            serviceOutside.listServiceOutsideDetailDTO[index].totalPrice = parseFloat(unitPrice.replace(/,/g, '')) * parseInt(quantity);
            serviceOutside.listServiceOutsideDetailDTO[index].totalPrice = serviceOutside.listServiceOutsideDetailDTO[index].totalPrice.replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        }
        $scope.calculateTotalServiceOutsideDetail = function (serviceOutside) {
            var total = 0;
            if (serviceOutside.listServiceOutsideDetailDTO == null) {
                return total.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "VND";
            }
            for (var i = 0; i < serviceOutside.listServiceOutsideDetailDTO.length; i++) {
                var serviceOutsideDetail = serviceOutside.listServiceOutsideDetailDTO[i];
                total += parseFloat(serviceOutsideDetail.totalPrice.toString().replace(/,/g, ''));
            }
            return total.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + "VND";
        }

        //Xử lý save
        $scope.commissionSaveState = "undone";
        $scope.serviceOutsideSaveState = "undone";
        $scope.save = function () {
            $http({
                method: "POST",
                url: "WebMethod/BookingViewWebMethod.asmx/CommissionSave",
                data: {
                    "listCommissionDTO": $scope.listCommission,
                    "bookingId": $scope.bookingId,
                },
            }).then(function (response) {
                $scope.commissionSaveState = "done";
            }, function (response) {
            })
            $http({
                method: "POST",
                url: "WebMethod/BookingViewWebMethod.asmx/ServiceOutsideSave",
                data: {
                    "listServiceOutsideDTO": $scope.listServiceOutside,
                    "bookingId": $scope.bookingId,
                },
            }).then(function (response) {
                $scope.serviceOutsideSaveState = "done";
            }, function (response) {
            })
            $scope.$watchGroup(["commissionSaveState", "serviceOutsideSaveState", "guideSaveState", "bookerSaveState"], function (newValues, oldValues, scope) {
                if ($scope.commissionSaveState == "done"
                    && $scope.serviceOutsideSaveState == "done") {
                    setTimeout(function () { __doPostBack($("#btnSave").attr("data-uniqueId"), "OnClick"); }, 1);
                }
            })
        };

        //Xử lý tính total
        $scope.calculateTotalPrice = function () {
            var totalServiceOutside = 0.0;
            try {
                totalServiceOutside = parseFloat($scope.totalServiceOutside.toString().replace(/,/g, ''));
            } catch (Exception) { }
            var totalPriceOfSet = 0.0;
            try {
                totalPriceOfSet = parseFloat($scope.totalPriceOfSet.toString().replace(/,/g, ''));
            } catch (Exception) { }
          
            if ($scope.isSeatingCruise) {
                $scope.calculateTotalSeatingCruise();
            } else {
                $scope.calculateTotal()
            }

            $scope.totalPrice = totalServiceOutside + totalPriceOfSet + $scope.total;
            $scope.totalPrice = $scope.totalPrice.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        }

        //Xử lý tính thực thu
        $scope.actuallyCollected = 0;
        $scope.totalCommission = 0;
        $scope.calculateActuallyCollected = function () {
            $scope.actuallyCollected = parseFloat($scope.totalPrice.toString().replace(/,/g, '')) - parseFloat($scope.totalCommission.toString().replace(/,/g, ''));
            $scope.actuallyCollected = $scope.actuallyCollected.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",")
        }
        $scope.$watch('totalPrice', function () {
            if ($scope.startCalculateTotalPrice) {
                $scope.calculateActuallyCollected();
            }
            $scope.startCalculateTotalPrice = true;
        })
        $scope.$watch('totalCommission', function () {
            if ($scope.startCalculateTotalCommission) {
                $scope.calculateActuallyCollected();
            }
            $scope.startCalculateTotalCommission = true;
        })
       
    }])
