(() => {
    'use strict';
    angular.module('app').factory('PaymentSvc', Service);

    Service.$inject = ['$http', '$httpParamSerializerJQLike', '$localStorage', 'API', 'SOURCE_API','CONFIG'];
    function Service($http, $serialize, $local, $API, $SOURCE_API, $CONFIG) {
        $http.defaults.headers.common.Authorization = 'Bearer ' + $.cookie("COPSToken");

        return {
            get: () => {
                $http.defaults.headers.common.Authorization = 'Bearer ' + $.cookie("COPSToken");
                return new Promise((resolve, reject) => {
                    $http.get($API + 'api/payments/get')
                    .then(function (response) {
                        resolve(response.data);
                    }, function (error) {
                        console.error(error);
                        reject({ status: error.status, message: !!(typeof error.data == 'string') ? error.data : error.statusText });
                    });
                });
            },

            post: (model) => {
                $http.defaults.headers.common.Authorization = 'Bearer ' + $.cookie("COPSToken");
                return new Promise((resolve, reject) => {
                    $http.post($API + 'api/payments/post',model)
                        .then(function (response) {
                            resolve(response.data);
                        }, function (error) {
                            console.error(error);
                            reject({ status: error.status, message: !!(typeof error.data == 'string') ? error.data : error.statusText });
                        });
                });
            }

            
        }; /*end of return */
    }

})();