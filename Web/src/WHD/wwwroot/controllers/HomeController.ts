/// <reference path="../app.ts" />

angular
    .module('main', [])
    .component('main', {
        templateUrl: '/views/home.html',
        controller: ['$http', function Controller($http) {
            var self = this;
        }]
    })