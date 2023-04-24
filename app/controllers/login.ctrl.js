(function () {
    'use strict';
    angular.module('app').controller('LogInCtrl', Controller);

    Controller.$inject = ['AuthSvc', '$scope', '$filter', '$localStorage'];
    function Controller($service, $scope, $filter, $local) {
        $scope.user = {};


        $scope.logIn = async () => {
            try {
                var result = await $service.logIn($scope.user);
                $local.COPSToken = result.token;
            } catch (e) {
                toastr.warning(e.message);
            }
        }

    }
})();