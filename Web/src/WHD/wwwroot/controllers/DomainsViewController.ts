/// <reference path="../app.ts" />

angular
    .module('domainsView', [])
    .component('domainsView', {
        templateUrl: '/views/domains-view.html',
        controller: ['$http', '$routeParams', function Controller($http, $routeParams) {
            var self = this;
            this.domain = $routeParams.domain;

            $http.get('/api/domain/' + $routeParams.domain).then(function (response) {
                console.log(response);
                self.details = response.data;
            })
        }]
    })