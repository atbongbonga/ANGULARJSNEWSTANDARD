(function () {
    angular
        .module("app", ['ngStorage'])
        .constant('CONFIG', {
            get: { headers: { 'Content-Type': 'text/plain; charset=utf-8' } },
            post: { headers: { 'Content-Type': 'application/x-www-form-urlencoded;charset=utf-8' } },
            file: { transformRequest: angular.identity, headers: { 'Content-Type': undefined } },
        })
        .constant('SOURCE_API', 'http://192.171.3.2:86/hpsources/') //LIVE
        //.constant('API', 'http://192.171.3.29:84/disbursements_api/') //LIVE
        //.constant('API', 'http://192.171.3.2:86/disbursements_api/') //TEST
        .constant('API', 'http://localhost:7155/') //DEV



})();