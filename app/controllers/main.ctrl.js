(function () {
    'use strict';
    angular.module('app').controller('MainCtrl', Controller);

    Controller.$inject = ['masterdataSvc', '$scope', '$filter'];
    function Controller($service, $scope, $filter) {
        $scope.excelMediaType = "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,";
        $scope.zipMediaType = "data:application/zip;base64,";
        $scope.genericMediaType = "data:application/octet-stream;base64,";
        
        $scope.getBase64 = (file) => {
            return new Promise((resolve, reject) => {
                const reader = new FileReader();
                reader.readAsDataURL(file);
                reader.onload = response => resolve(reader.result.split(',')[1]);
                reader.onerror = error => reject(error);
            });
        }

        /*ag-grid renderers */
        $scope.render = {
            date: (params) => $filter('date')(params.value, 'yyyy-MM-dd'),
            dateTime: (params) => $filter('date')(params.value, 'yyyy-MM-dd h:mm a'),
            int: (params) => $filter('number')(params.value, 0),
            float: (params) => $filter('number')(params.value, 2),
        }

        $scope.getEmployees = async() => {
            return await $service.getEmployees();
        }

        $scope.getBranches = async() => {
            return await $service.getBranches();
        }

        $scope.getCompanies = async() => {
            return await $service.getCompanies();
        }

        $scope.getTransTypes = async() => {
            return await $service.getTransTypes();
        }

        $scope.getBanksByBranch = async(branch) => {
            return await $service.getBanksByBranch(branch);
        }

        $scope.getParentAccounts = async() => {
            return await $service.getParentAccounts();
        }

        $scope.getAccountsByCompany = async(company) => {
            return await $service.getAccountsByCompany(company);
        }

        $scope.getPModes = async() => {
            return await $service.getPModes();
        }

        $scope.getParentAccounts = async() => {
            return await $service.getParentAccounts();
        }

        $scope.getMonths = async() => {
            return await $service.getMonths();
        }

        $scope.getQuarters = async() => {
            return await $service.getQuarters();
        }

        $scope.getYears = async() => {
            return await $service.getYears();
        }
    }
})();