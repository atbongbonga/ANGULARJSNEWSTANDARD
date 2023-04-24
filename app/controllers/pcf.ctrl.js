(function () {
    'use strict';
    angular.module('app').controller('pcfCtrl', Controller);

    Controller.$inject = ['pcfSvc', '$scope', '$filter'];
    function Controller($service, $scope, $filter) {
        $scope.data = {
            Header: {},
            Details: []
        };

     
        $scope.PostJE = async() => {

            //Variables
            let remarks = $("#txtRemarks").val(),
                ref1 = $("#txtRef1").val(),
                ref2 = $("#txtRef2").val(),
                ref3 = $("#txtRef3").val(),
                //type = "PCF",
                debit = $("#txtTDebit").val(),
                credit = $("#txtTCredit").val(),
                tableList = $("#table-list tbody"),
                docentry = $("#txtDoc").val() == "NEW" ? 0 : $("#txtDoc").val(),
                refDate = $("#txtRefDate").val(),
                transId = $("#txtTransID").val() == "" ? 0 : $("#txtTransID").val(),
                opNum = $("#txtOP").val(),
                pcfOP = $("#txtOP").val(),
                pcfWhs = $("#txtPCFWhs").val(),
                pcfDoc = $("#txtPCFDoc").val(),
                stringEmpty = "";


            //VALIDATIONS

            if (remarks == stringEmpty) {
                toastr.warning("Remarks is required")
                return
            }
            if (ref1 == stringEmpty) {
                toastr.warning("Ref1 is required")
                return
            }
            if (ref2 == stringEmpty) {
                toastr.warning("Ref2 is required")
                return
            }
            if (ref3 == stringEmpty) {
                toastr.warning("Ref3 is required")
                return
            }

            var _debit = jsTonum(debit)
            var _credit = jsTonum(credit)

            if (_debit != _credit) {
                toastr.warning("Total Debit and Credit must be equal")
                return
            }
            //VALIDATIONS


      
            $scope.data.Details = [];
            tableList.find('tr').each(function (i) {
                var tr = $(this)
                var _model = {
                    Docentry: 0,
                    AcctCode: tr.attr("data-acct"),
                    FormatCode: tr.children("td.tdacct").text(),
                    BrCode: tr.children("td.tdwhs").text(),
                    ShortName: tr.children("td.tdcode").text(),
                    Ref1: tr.children("td.tdref1").text(),
                    Ref2: tr.children("td.tdref2").text(),
                    Ref3: tr.children("td.tdref3").text(),
                    LineMemo: tr.children("td.tdrem").text(),
                    Debit: jsTonum(tr.children("td.tddebit").text()),
                    Credit: jsTonum(tr.children("td.tdcredit").text()),
                    Amount: jsTonum(tr.children("td.tddebit").text()) == 0 ? -jsTonum(tr.children("td.tdcredit").text()) : jsTonum(tr.children("td.tddebit").text()),
                    Account: "",
                    U_EmpID: ""

                }
                $scope.data.Details.push(_model)
            });

           var Header = {
                Docentry: 0,
                RefDate: refDate,
                Memo: remarks,
                Ref1: ref1,
                Ref2: ref2,
                Ref3: ref3,
                PCFOP:  pcfOP ,
                PCFWhs: pcfWhs,
                PCFDoc: pcfDoc,
                U_FTDocNo: "",
                BrCode: "",
                PostBy: ""
         
            }
            $scope.data.Header = Header
            console.log($scope.data)
  



            swal.fire({
                title: "Post selected transaction(s)?",
                text: "",
                type: "info",
                showCancelButton: true,
                confirmButtonClass: 'btn-primary btn-md waves-effect waves-light',
                confirmButtonText: "Yes"
            }).then(async (result) => {
                if (result.value) {
                    window.swal({
                        title: "Processing...",
                        text: "Please wait",
                        imageUrl: "/COPSWeb/Themes/assets/images/Loading.gif",
                        showConfirmButton: false,
                        allowOutsideClick: false
                    });

                    try {
                        await $service.post($scope.data);
                        toastr.success("record successfully posted");
                        swal.clickCancel();
                    } catch (error) {
                        toastr.error(error.message);
                        swal.clickCancel();
                    }

                }
            });





        }


        function jsTonum(stringValue, locale) {
            var parts = Number(1111.11).toLocaleString(locale).replace(/\d+/g, '').split('');
            if (stringValue === null)
                return null;
            if (parts.length == 1) {
                parts.unshift('');
            }
            return Number(String(stringValue).replace(new RegExp(parts[0].replace(/\s/g, ' '), 'g'), '').replace(parts[1], "."));
        }
    }
})();