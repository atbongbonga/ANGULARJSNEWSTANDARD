(() => {
    'use strict';
    angular.module('app').factory('pcfSvc', Service);

    Service.$inject = ['$http', '$httpParamSerializerJQLike', '$localStorage', 'API', 'SOURCE_API', 'CONFIG'];
    function Service($http, $serialize, $local, $API, $SOURCE_API, $CONFIG) {

        return {
            post: (model) => {
                $http.defaults.headers.common.Authorization = 'Bearer ' + cookie.get("COPSToken");
                return new Promise((resolve, reject) => {
                    $http.post($API + 'api/pcfje/post', model)
                        .then(function () {
                            resolve();
                        }, function (response) {
                            reject(response);
                        });
                }, (error) => {
                    console.log(error);
                    reject({ status: error.status, message: !!(typeof (error.data) == 'string') ? error.data : error.data.Message });
                });
            }
        } /*end of return */
    }

})();