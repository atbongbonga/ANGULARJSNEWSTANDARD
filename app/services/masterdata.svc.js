(() => {
    'use strict';
    angular.module('app').factory('masterdataSvc', Service);

    Service.$inject = ['$http', '$localStorage', 'API', 'SOURCE_API'];
    function Service($http, $local, $API, $SOURCE_API) {
        $http.defaults.headers.common.Authorization = 'Bearer ' + $local.token;

        return {
            getEmployees: () => {
                return new Promise((resolve, reject) => {
                    $http.get($SOURCE_API + 'api/employees/get')
                    .then(function (response) {
                        resolve(response.data);
                    });
                });
            },
            getBranches: () => {
                return new Promise(function (resolve, reject) {
                    $http.get($SOURCE_API + 'api/branches/get')
                    .then(function (response) {
                        resolve(response.data);
                    }, function (response) {
                        reject(response);
                    });
                });
            },
            getCompanies: () => {
                return new Promise(function (resolve, reject) {
                    $http.get($SOURCE_API + 'api/companies/get')
                    .then(function (response) {
                        resolve(response.data);
                    }, function (response) {
                        reject(response);
                    });
                });
            },
            getTransTypes: () => {
                return new Promise((resolve, reject) => {
                    $http.get($SOURCE_API + 'api/transtypes/get')
                    .then(function (response) {
                        resolve(response.data);
                    });
                });
            },
            getBanksByBranch: (branch) => {
                return new Promise((resolve, reject) => {
                    $http.get($SOURCE_API + 'api/chartofaccounts/banks/get?branch=' + branch)
                    .then(function (response) {
                        resolve(response.data);
                    });
                });
            },
            getParentAccounts: () => {
                return new Promise(function (resolve, reject) {
                    $http.get($SOURCE_API + 'api/chartofaccounts/parents')
                    .then(function (response) {
                        resolve(response.data);
                    }, function (response) {
                        reject(response);
                    });
                });
            },
            getAccountsByCompany: (company) => {
                return new Promise(function (resolve, reject) {
                    $http.get($SOURCE_API + 'api/chartofaccounts/get?company=' + company)
                    .then(function (response) {
                        resolve(response.data);
                    }, function (response) {
                        reject(response);
                    });
                });
            },
            getPModes: () => {
                return new Promise(function (resolve, reject) {
                    $http.get($SOURCE_API + 'api/paymentmodes/get')
                    .then(function (response) {
                        resolve(response.data);
                    });
                });
            },
            getMonths: () => {
                return new Promise((resolve, reject) => {
                    resolve([
                        { Name: 'January', Abbrev: 'Jan', Value: 1 },
                        { Name: 'February', Abbrev: 'Feb', Value: 2 },
                        { Name: 'March', Abbrev: 'Mar', Value: 3 },
                        { Name: 'April', Abbrev: 'Apr', Value: 4 },
                        { Name: 'May', Abbrev: 'May', Value: 5 },
                        { Name: 'June', Abbrev: 'Jun', Value: 6 },
                        { Name: 'July', Abbrev: 'Jul', Value: 7 },
                        { Name: 'August', Abbrev: 'Aug', Value: 8 },
                        { Name: 'September', Abbrev: 'Sep', Value: 9 },
                        { Name: 'October', Abbrev: 'Oct', Value: 10 },
                        { Name: 'November', Abbrev: 'Nov', Value: 11 },
                        { Name: 'December', Abbrev: 'Dec', Value: 12 }
                    ]);
                });
            },
            getQuarters: () => {
                var quarters = [
                    { Name: "1st", Value: 1 },
                    { Name: "2nd", Value: 2 },
                    { Name: "3rd", Value: 3 },
                    { Name: "4th", Value: 4 }
                ];

                return new Promise(function (resolve, reject) {
                    resolve(quarters);
                });
            },
            getYears: () => {
                var year = new Date().getFullYear() + 1;
                var years = [];
                for (var i = year; i >= 2016; i--) years.push(i);

                return new Promise(function (resolve, reject) {
                    resolve(years);
                });
            },
        }; /*end of return */
    }

})();