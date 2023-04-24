(function () {
    'use strict';
    angular.module('app').controller('lessorpaymentCtrl', Controller);

    Controller.$inject = ['lessorpaymentSvc', '$scope', '$filter','$localStorage'];
    function Controller($service, $scope, $filter, $local) {
        console.log($local.COPSToken);
        $scope.payment = {
            Header: {},
            Accounts: [],
            Checks: []
        };
        var paymentData = [];
        $scope.paymentData = {
            Lessor: []
        };

        /*ag-grid renderers */
        $scope.render = {
            date: (params) => $filter('date')(params.value, 'yyyy-MM-dd'),
            dateTime: (params) => $filter('date')(params.value, 'yyyy-MM-dd h:mm a'),
            int: (params) => $filter('number')(params.value, 0),
            float: (params) => $filter('number')(params.value, 2),
        }

        $scope.get = async () => {
            $scope.get = async () => {
                return await $service.get();
            }
        }

        $scope.postLessorPayment = async function () {
            //Header 

            $("#table-lsop tbody").find('tr').each(function (i) {

                var tr = $(this),
                chk = tr.find('.chkRow').prop('checked');
                if (chk == true) {
                    $scope.payment.Header = {};
                    $scope.payment.Accounts = [];

                    $scope.payment.Header = {
                        DocNum: 0,
                        DocDate: tr.children("td.tddate").text(),
                        DocDueDate: tr.children("td.tddate").text(),
                        DueDate: tr.children("td.tddate").text(),
                        DocType: 'A',
                        PayNoDoc: "",
                        NoDocAmt: 0,
                        CardCode: $("input[name='txtCode']").val(),
                        CardName: $("input[name='txtName']").val(),
                        Address: "",
                        DocTotal: parseFloat(tr.children("td.tdnet").text().replace(/,/g, ''), 2),
                        CashAmt: 0,
                        CreditAmt: 0,
                        CheckAmt: 0,
                        CheckAcct: "",
                        TransferAmt: 0,
                        TransferAcct: "",
                        Ref1: "",
                        Ref2: tr.data("lsno"),
                        Comments: $("#txtRem").val(),
                        JrnlMemo: $("input[name='txtName']").val(),
                        U_APDocNo: $("input[name='txtCode']").val(),
                        U_ChkNum: "",
                        U_CardCode: $("input[name='txtCode']").val(),
                        U_BranchCode: "",
                        U_HPDVoucherNo: "",
                        WhsCode: tr.children("td.tdwhs").text(),
                        Bank: tr.children("td.tdbank").text(),
                        BranchCode: tr.children("td.tdwhs").text(),
                        Remarks: tr.children("td.tddesc").text(),
                        CheckPrint: "LOA",
                        CheckRemarks: ""
                    };

                    //Account Details

                    var account = {
                        DocNum: 0,
                        LineId: (i + 1),
                        AcctCode: tr.children("td.tdacct").text(),
                        SumApplied: parseFloat(tr.children("td.tdamt").text().replace(/,/g, ''), 2),
                        Description: '',
                        U_EmpID: '',
                        U_DocLine: '',
                        AcctName: '',
                        WhsCode: tr.children("td.tdwhs").text(),
                        TaxGroup: 'NET',
                        ATC: tr.children("td.tdatc").text(),
                        Rate: tr.children("td.tdrate").text(),
                        VAT: parseFloat(tr.children("td.tdvat").text().replace(/,/g, ''), 2),
                        NetVAT: 0,
                        WTAX: parseFloat(tr.children("td.tdwtax").text().replace(/,/g, ''), 2),
                        EWT: (parseFloat(tr.children("td.tdvat").text().replace(/,/g, ''), 2) * parseFloat(tr.children("td.tdrate").text().replace(/,/g, ''), 2) / 100),
                        Proj: tr.data("id")
                    }
                    $scope.payment.Accounts.push(account);
                    //End Account Details

                    paymentData.push($scope.payment);
                }
            });

            $scope.paymentData.Lessor = paymentData;

            console.log($scope.paymentData.Lessor);

            $scope.$applyAsync();
            ////VALIDATIONS
            //if (!$scope.payment.Header.CardCode) { toastr.warning("Invalid CardCode."); return; }
            //if (!$scope.payment.Header.PMode) { toastr.warning("Invalid Payment Means."); return; }
            //if (!$scope.payment.Header.Bank) { toastr.warning("Invalid Bank."); return; }
            //if (!$scope.payment.Header.Comments) { toastr.warning("Invalid Remarks."); return; }
            //if ($("#txtAType").val() == 'CA') {
            //    if (!$scope.payment.Header.PMode) {
            //        toastr.warning("Please select check print mode.");
            //        return;
            //    }
            //    else {
            //        if ($scope.payment.Header.PMode =="MANUAL CHECK") {
            //            toastr.warning("Please indicated reason for Manual Check.");
            //            return;
            //        }
            //    }
            //}

            //$scope.payment.Accounts.forEach(function (i) {
            //    if (!i.SumApplied) {
            //        toastr.warning("Applied Amount must not be zero")
            //    }
            //});

            //if (!$scope.payment.Accounts) { toastr.warning("Nothing to Post"); return; };
            ////END VALIDATION

            ////CONFIRMATION
            swal.fire({
                title: "Post Lessor Payment?",
                text: "",
                type: "info",
                showCancelButton: true,
                confirmButtonClass: 'btn-primary btn-md waves-effect waves-light',
                confirmButtonText: "Yes"
            }).then(async (result) => {
                if (result.value) {
                    window.swal({
                        title: "Processing...",
                        imageUrl: "/COPSWeb/Themes/assets/images/Loading.gif",
                        showConfirmButton: false,
                        allowOutsideClick: false
                    });

                    try {

                        await $service.post($scope.paymentData);

                        swal.close();

                        setTimeout(function () {
                            document.getElementById('btn-load-oputil').click();
                        }, 0);

                        toastr.success("Payment Successfully Posted");
                    } catch (error) {
                        swal.close();
                        console.log(error);
                        toastr.error(error.message);
                    }

                }
            });
            ////END CONFIRMATION
        }
         
    }
    
})();