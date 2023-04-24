(function () {
    'use strict';
    angular.module('app').controller('jgmanCtrl', Controller);

    Controller.$inject = ['jgmanSvc', '$scope', '$filter', '$localStorage'];
    function Controller($service, $scope, $filter , $local) {
        $scope.excelMediaType = "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,";
        $scope.zipMediaType = "data:application/zip;base64,";
        $scope.genericMediaType = "data:application/octet-stream;base64,";

       
        $scope.postPayment = async () => {

            $scope.data = [];
                $("#table-list tbody").find('tr').each(function (i) {
                    var tr = $(this)
                    var check = $(this).find('.chkRow').prop('checked')
                    if (check == true) {
                        var postdetail = {
                            GenId: $("#txtGenID").val(),
                            CorpCode: "",
                            CorpName: "",
                            BrCode: tr.data("whs"),
                            BrName: "",
                            AcctType: tr.data("bk"),
                            Amount: 0,
                            DocNum: 0,
                            DocDate: $("#dtDate").val(),
                            StartDate: $("#txtFrom").val(),
                            EndDate: $("#txtTo").val(),
                            CollnRemarks: "",
                            OffRcptNo: "",
                            OffRcptDate: null
                        }
                        $scope.data.push(postdetail);
                    }
                })

            if ($scope.data.length == 0) { toastr.info("Please check atleast one(1) item to proceed."); return; };

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
                        await $service.postPayment($scope.data);
                        toastr.success("record successfully posted");
                        $(".btn-Load").trigger("click");
                        swal.clickCancel();
                    } catch (error) {
                        toastr.error(error.message);
                        swal.clickCancel();
                    }

                }
            });
            
        }

        
    }
})();