(function () {
    'use strict';
    angular.module('app').controller('utilitypaymentCtrl', Controller);

    Controller.$inject = ['utilitypaymentSvc', '$scope', '$filter','$localStorage'];
    function Controller($service, $scope, $filter, $local) {
        console.log("ALL START HERE");
        console.log($local.COPSToken);
        $scope.payment = {
            Header: {},
            Accounts: [],
            Checks: [],
            JournalEntries: [],
        };

        /*ag-grid renderers */
        $scope.render = {
            date: (params) => $filter('date')(params.value, 'yyyy-MM-dd'),
            dateTime: (params) => $filter('date')(params.value, 'yyyy-MM-dd h:mm a'),
            int: (params) => $filter('number')(params.value, 0),
            float: (params) => $filter('number')(params.value, 2),
        }

        $scope.get = async() => {
            return await $service.get();
        }

        $scope.postUtilityPayment = async function () {
            //Header 
            var header = {
                DocNum: 0,
                DocDate: $("#txtDate").val(),
                DocDueDate: $("#txtDate").val(),
                DocType: 'A',
                PayNoDoc: "",
                NoDocAmt:0,
                CardCode: $("#txtCode").val(),
                CardName: $("#txtVendor").val(),
                Address: "",
                DocTotal: parseFloat($("#txtTPaid").val().replace(/,/g, ''), 2),
                CashAmt: parseFloat($("#txtPAmt").val().replace(/,/g, ''), 2) - parseFloat($("#txtTEWT").val().replace(/,/g, ''), 2),
                CreditAmt: 0,
                CheckAmt:0,
                CheckAcct: "",
                TransferAmt: 0,
                TransferAcct: "",
                //TransferDate: null,
                Ref1: "",
                Ref2:"",
                Comments: $("#txtRem").val(),
                JrnlMemo:"",
                U_APDocNo: "",
                U_ChkNum: $("#txtChk").val(),
                U_CardCode: $("#txtCode").val(),
                U_BranchCode: $("#txtWhs").val(),
                U_HPDVoucherNo: "",
                CreditCard: 0,
                CreditAcct: "",
                WhsCode: $("#cbowhs").val(),
                PMode: $("#cboMeans").val(),
                PayTo: $("#txtPayTo").val(),
                CheckPrint: $("#cboMode").val(),
                CheckRemarks: $("#cboReason").val(),
                CheckPrintRemarks: "",
                CheckPrintName: $("#txtPayee").val(),
                PCFDocNum:  0,
                Bank: $("#txtBank").val(),
                BankCode: "",
                UID:"",
                UName: "",
                CAOARes: $("#cboNoRes").val(),
                AdvDueJE: $("#chkJE").prop('checked') == true ? 1 : 0,
                FBillNo: $("#txtFBill").val(),
                OPUtilDocEntry: 0,
                OPId: $("#txtopid").val()
            }
            $scope.payment.Header = header;

            //Account Details
            $scope.payment.Accounts = [];
            $("#table-list tbody").find('tr').each(function (i) {
                var tr = $(this);
                var $tds = $(this).find("td");

                var data = {
                    DocNum: 0,
                    LineId: (i + 1),
                    AcctCode: tr.children("td.tdglacct").text(),
                    SumApplied: parseFloat(tr.children("td.tdapp").text().replace(/,/g, ''), 2),
                    Description: tr.children("td.tdrem").text(),
                    U_EmpID: '',
                    U_DocLine: '',
                    AcctNo: tr.children("td.tdacct").text(),
                    InvNo: tr.children("td.tdinv").text(),
                    BrWhs: tr.children("td.tdwhs").text(),
                    GLAcct: '',
                    BrCode: tr.children("td.tdwhs").text(),
                    BillDate: tr.data("billdate") == null ? '01/01/1900' : tr.data("billdate"),
                    IsManualVat: false,
                    NetVat: parseFloat(tr.children("td.tdnetvat").text().replace(/,/g, ''), 2),
                    Vat: parseFloat(tr.children("td.tdvat").text().replace(/,/g, ''), 2),
                    TaxGroup: tr.find('.tdgrp').val(),
                    ATC: tr.children("td.tdatc").text(),
                    Rate: tr.children("td.tdrate").text(),
                    EWT:0,
                    ARAmt: parseFloat(tr.children("td.tdaramt").text().replace(/,/g, ''), 2),
                    ARRemarks: tr.children("td.tdarrem").text(),
                    WTE: parseFloat(tr.children("td.tdewt").text().replace(/,/g, ''), 2),
                    ManualVat: tr.find("#chkVAT").prop("checked") == true ? true : false,
                    OPUtilDocEntry:0,
                    DetId: tr.data("id"),

                }
                $scope.payment.Accounts.push(data);
            });
            //End Account Details
            $scope.$applyAsync();
            //VALIDATIONS
            if (!$scope.payment.Header.CardCode) { toastr.warning("Invalid CardCode."); return; }
            if (!$scope.payment.Header.PMode) { toastr.warning("Invalid Payment Means."); return; }
            if (!$scope.payment.Header.Bank) { toastr.warning("Invalid Bank."); return; }
            if (!$scope.payment.Header.Comments) { toastr.warning("Invalid Remarks."); return; }
            if ($("#txtAType").val() == 'CA') {
                if (!$scope.payment.Header.PMode) {
                    toastr.warning("Please select check print mode.");
                    return;
                }
                else {
                    if ($scope.payment.Header.PMode =="MANUAL CHECK") {
                        toastr.warning("Please indicated reason for Manual Check.");
                        return;
                    }
                }
            }

            $scope.payment.Accounts.forEach(function (i) {
                if (!i.SumApplied) {
                    toastr.warning("Applied Amount must not be zero")
                }
            });

            if (!$scope.payment.Accounts) { toastr.warning("Nothing to Post"); return; };
            //END VALIDATION
            //CONFIRMATION
            swal.fire({
                title: "Post Utility Payment?",
                text: "",
                type: "info",
                showCancelButton: true,
                confirmButtonClass: 'btn-primary btn-md waves-effect waves-light',
                confirmButtonText: "Yes"
            }).then(async (result) => {
                if (result.value) {
                    window.swal({
                        title: "Processing...",
                        //text: "Please wait",
                        imageUrl: "/COPSWeb/Themes/assets/images/Loading.gif",
                        showConfirmButton: false,
                        allowOutsideClick: false
                    });

                    try {

                        await $service.post($scope.payment);
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
           //END CONFIRMATION
            
        }

        
    }
})();