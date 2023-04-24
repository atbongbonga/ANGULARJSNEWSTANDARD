(() => {
    'use strict';
    angular.module('app').factory('AuthSvc', Service);

    Service.$inject = ['$http', '$httpParamSerializerJQLike', '$localStorage', 'API', 'SOURCE_API', 'CONFIG'];
    function Service($http, $serialize, $local, $API, $SOURCE_API, $CONFIG) {
        //$http.defaults.headers.common.Authorization = 'Bearer ' + $local.COPSToken;

        return {
            logIn: (model) => {
                return new Promise((resolve, reject) => {
                    $http.defaults.headers.common.Authorization = undefined; 

                    $http.post($API + 'api/auth/login', model)
                        .then(function (response) {
                            console.log(response);
                            resolve(response.data);
                        }, function (error) {
                            console.error(error);
                            reject({ status: error.status, message: !!(typeof (error.data) == 'string') ? error.data : error.data.Message });
                        });
                });
            }


        }; /*end of return */
    }

})();