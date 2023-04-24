(() => {
    'use strict';
    angular.module('app').factory('jgmanSvc', Service);

    Service.$inject = ['$http', '$httpParamSerializerJQLike', '$localStorage', 'API', 'SOURCE_API', 'CONFIG'];
    function Service($http, $serialize, $local, $API, $SOURCE_API, $CONFIG) {

        return {
            postPayment: (model) => {
                return new Promise((resolve, reject) => {
                    $http.defaults.headers.common.Authorization = 'Bearer ' + cookie.get("COPSToken");
                    $http.post($API + 'api/jgman/payments/post', model)
                        .then(function (response) {
                            resolve(response.data);
                        }, function (error) { 
                            console.log(error);
                            reject({ status: error.status, message: !!(typeof (error.data) == 'string') ? error.data : error.data.Message });
                        });
                });
            }


        }; /*end of return */
    }

})();