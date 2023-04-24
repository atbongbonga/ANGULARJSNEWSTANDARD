(function () {
    'use strict';
    angular.module('app').controller('PaymentCtrl', Controller);

    Controller.$inject = ['PaymentSvc', '$scope', '$filter','$localStorage'];
    function Controller($service, $scope, $filter, $local) {
        $scope.payment = {
            Header: {},
            Accounts: [],
            Invoices: [],
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

        $scope.postPayment = async function () {
            //Header 
            $scope.payment.Header = {
                DocNum: 0,
                DocDate: $("input[name='dtDocDate']").val(),
                DocDueDate: $("input[name='dtDocDueDate']").val(),
                DocType: $("select[name='select-type']").val() ?? "",
                PayNoDoc: "",
                NoDocAmt:0,
                CardCode: $("input[name='txtCode']").val() ?? "",
                CardName: $("input[name='txtName']").val() ?? $("input[name='txtsearch']").val(),
                Address: $("input[name='txtAddr']").val() ?? "",
                DocTotal: parseFloat($("input[name='txtTotal']").val().replace(/,/g, ''), 2) ?? 0,
                Ref1: "",
                Ref2: $("input[name='txtRef2']").val() ?? "",
                Comments: $("textarea[name='txtRemarks']").val() ?? "",
                JrnlMemo: $("input[name='txtJERemarks']").val() ?? "",
                U_APDocNo: "",
                U_ChkNum: $("input[name='txtCheck']").val() ?? "",
                U_CardCode: $("input[name='txtUCard']").val() ?? "",
                U_BranchCode: $("input[name='txtWhs']").val() ?? "",
                U_HPDVoucherNo: "",
                PMode: $("#cboPMean").val() ?? "",
                WhsCode: $("select[name='select-branch']").val() ?? "",
                BankCode: $("input[name='txtBank']").val() ?? "",
                DueDate: $("input[name='dtDueDate']").val() ?? "",
                AcctCode: "",
                CWPayee: $("input[name='txtCW']").val() ?? "",
                F2307Bill: $("#txtBillNo").val() ?? "",
                CheckPrintMode: $("select[name='cboMode']").val() ?? "",
                PaymentType: $("#cboPMode").val() ?? "",
                TaxGroup: $("#cboTaxGrp").val() ?? "",
                ATC: $("#cboATC").val() ?? "",
                OAReason: $("#cboNoRes").val() ?? "",
                TotalGross: parseFloat($("#txtTGross").val().replace(/,/g, ''), 2) ?? 0,
                TotalEWT: parseFloat($("#txtTEwt").val().replace(/,/g, ''), 2) ?? 0,
                TotalNet: parseFloat($("#txtTotal").val().replace(/,/g, ''), 2) ?? 0,
                PostedBy: "",
                CancelledBy: "",
                UpdatedBy: "",
            }

            //invoice details
            $scope.payment.Invoices = [];
            $("#table-Vendor tbody").find('tr').each(function (i) {
                var tr = $(this);
                var $tds = $(this).find("td");
                if (tr.hasClass("bg-blue") && parseFloat(tr.children("td.tdamt").text().replace(/,/g, ''), 2) + parseFloat(tr.children("td.tdEwt").text().replace(/,/g, ''), 2) > 0) {
                    var data = {
                        LineId: (i + 1),
                        DocNum: 0,
                        InvoiceId: 0,
                        DocEntry: tr.children("td.tdInvDoc").text() ?? 0,
                        InvType: tr.children("td.tdobj").text() ?? 0,
                        SumApplied: parseFloat(tr.children("td.tdamt").text().replace(/,/g, ''), 2) ?? 0,
                        DocTransId: 0,
                        Balance: parseFloat(tr.children("td.tdbal").text().replace(/,/g, ''), 2) ?? 0,
                        BrCode: tr.children("td.tdwhs").text() ?? 0,
                        GLAcct: tr.children("td.tdglacct").text() ?? 0,
                        ATC: tr.children("td.tdatc").text() ?? "",
                        EWT: parseFloat(tr.children("td.tdEwt").text().replace(/,/g, ''), 2) ?? 0,
                        EWTTransId: tr.children("td.tdJE").text() ?? 0,
                    }
                    $scope.payment.Invoices.push(data);
                }
            });
            //End Invoice Details

            //Account Details
            $scope.payment.Accounts = [];
            $("#table-Account tbody").find('tr').each(function (i) {
                var tr = $(this);
                var $tds = $(this).find("td");

                var data = {
                    DocNum:0,
                    LineId: (i + 1),
                    AcctCode: tr.children("td.tdacct").text().substr(0, 5) + $tds.eq(3).text() + '000',
                    SumApplied: parseFloat(tr.children("td.tdamt").text().replace(/,/g, ''),2) ?? 0,
                    Description: tr.children("td.tddesc").text() ?? "",
                    U_EmpID: '',
                    U_DocLine: '',
                    GLAcct: '',
                    BrCode: tr.children("td.tdwhs").text() ?? "",
                    ManualCheck: 0,
                    NetVat: parseFloat(tr.children("td.tdnetvat").text().replace(/,/g, ''), 2) ?? 0,
                    Vat: parseFloat(tr.children("td.tdvat").text().replace(/,/g, ''), 2) ?? 0,
                    TaxGroup: tr.find(".tdtaxGrp").val() ?? "",
                    ATC: tr.children("td.tdatc").text() ?? "",
                    Rate: parseFloat(tr.children("td.tdrate").text().replace(/,/g, ''), 2) ?? 0,
                    EWT: parseFloat(tr.children("td.tdewt").text().replace(/,/g, ''), 2) ?? 0,
                }
                $scope.payment.Accounts.push(data);
            });
            //End Account Details

            $scope.$applyAsync();

            //CONFIRMATION
            swal.fire({
                title: "Post Payment?",
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
                            document.getElementById('btn-last-op').click();
                        }, 0);

                        toastr.success("Payment posted successfully.");
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