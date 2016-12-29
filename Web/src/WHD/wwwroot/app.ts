interface IAngular {
    module(name: string): IAngular;
    module(name: string, modules: string[]): IAngular;
    config(parameters: any[]): IAngular;
    component(name: string, object: any): IAngular;
    element(val: any): IAngular;
    filter(name: string, func: any): IAngular;
}

declare var angular: IAngular;
declare function escape(path: string): any;

function DialogController($scope, $mdDialog) {
    $scope.hide = function () {
        $mdDialog.hide();
    }

    $scope.cancel = function () {
        $mdDialog.cancel();
    }

    $scope.answer = function (answer) {
        $mdDialog.hide(answer);
    }
}

angular.module('whdApp', [
    'ngMaterial',
    'ngRoute',
    'angularUtils.directives.dirPagination',
    'main',
    'domains',
    'domainsView',
    'hits',
    'dnsserver',
    'stores'
]);

angular
    .module('whdApp')
    .config(['$locationProvider', '$routeProvider', function config($locationProvider, $routeProvider) {
        $locationProvider.hashPrefix('!');

        $routeProvider
            .when('/home', {
                template: '<main></main>'
            })
            .when('/domains', {
                template: '<domains></domains>'
            })
            .when('/domains/:domain', {
                template: '<domains-view></domains-view>'
            })
            .when('/hits', {
                template: '<hits></hits>'
            })
            .when('/dns-server', {
                template: '<dnsserver></dnsserver>'
            })
            .when('/stores', {
                template: '<stores></stores>'
            })
            .otherwise('/home');
    }])