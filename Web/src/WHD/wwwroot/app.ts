interface IAngular {
    module(name: string): IAngular;
    module(name: string, modules: string[]): IAngular;
    config(parameters: any[]): IAngular;
    component(name: string, object: any): IAngular;
    element(val: any): IAngular;
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

////////////////////////////////////////////////

angular
    .module('main', [])
    .component('main', {
        templateUrl: '/views/home.html',
        controller: ['$http', function Controller($http) {
            var self = this;
        }]
    })

angular
    .module('domains', [])
    .component('domains', {
        templateUrl: '/views/domains.html',
        controller: ['$http', '$scope', function Controller($http, $scope) {
            var self = this;

            self.currentPage = 1;
            self.totalCount = 0;
            self.pageSize = 15;
            self.domains = [];

            self.filterStore = '0';
            self.filterDomain = '';
            self.filterAddress = '';

            self.isLoading = false;

            var match = /d?store=(\d+)/g.exec(location.hash);
            if (match != null)
                self.filterStore = match[1];

            self.load = function (page) {
                var url = '/api/domain?page=' + page + '&ipp=' + self.pageSize;

                url += '&filter-store=' + self.filterStore;
                url += '&filter-domain=' + escape(self.filterDomain);
                url += '&filter-address=' + escape(self.filterAddress);

                //console.log(url);

                self.isLoading = true;
                $http.get(url).then(function (response) {
                    self.totalCount = response.data.total;
                    self.domains = response.data.domains;
                    self.isLoading = false;
                })
            }

            self.pageChangeHandler = function (num) {
                self.load(num);
            }

            $http.get('/api/store').then(function (response) {
                self.stores = response.data;
            })

            //self.load(1);

            $scope.$watch('$ctrl.filterStore + $ctrl.filterDomain + $ctrl.filterAddress', function () {
                //console.log('on change detected');
                self.currentPage = 1;
                self.load(1);
            }, true)
        }]
    })

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

angular
    .module('hits', [])
    .component('hits', {
        templateUrl: '/views/hits.html',
        controller: ['$http', function Controller($http) {
            var self = this;
        }]
    })

angular
    .module('dnsserver', [])
    .component('dnsserver', {
        templateUrl: '/views/dns-server.html',
        controller: ['$http', '$interval', function Controller($http, $interval) {
            var self = this;

            self.status = null;

            self.isLoading = false;
            self.load = function () {
                if (self.isLoading)
                    return;
                self.isLoading = true;
                $http.get('/api/service').then(function (response) {
                    self.status = response.data;
                    self.isLoading = false;
                })
            }

            //self.load();
            $interval(self.load, 2000);
            self.load();
        }]
    })

angular
    .module('stores', [])
    .component('stores', {
        templateUrl: '/views/stores.html',
        controller: ['$http', '$mdDialog', function Controller($http, $mdDialog) {
            var self = this;

            $http.get('/api/store').then(function (response) {
                self.stores = response.data;
            })

            self.renameStoreItem = function (e, id, name) {
                var dlg = $mdDialog.prompt()
                    .clickOutsideToClose(true)
                    .title('Rename store')
                    .textContent('Enter new name of store')
                    .placeholder('Store name')
                    .ok('Rename')
                    .cancel('Cancel');
                $mdDialog.show(dlg).then(function (result) {
                    console.log('rename to => ' + result);
                }, function () {
                    console.log('cancel rename');
                });

                e.preventDefault();
                return false;
            }

            self.removeStoreItem = function (e, id, name) {

                $mdDialog.show({
                    controller: DialogController,
                    templateUrl: '/dialogs/confirm-delete-store.html',
                    clickOutsideToClose: true
                }).then(function (result) {
                    if (result) {
                        console.log('>> DO DELETE');
                    }
                }, function () {
                })

                e.preventDefault();
                return false;
            }
        }]
    })