function DialogController($scope, $mdDialog) {
    $scope.hide = function () {
        $mdDialog.hide();
    };
    $scope.cancel = function () {
        $mdDialog.cancel();
    };
    $scope.answer = function (answer) {
        $mdDialog.hide(answer);
    };
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
    }]);
angular
    .module('main', [])
    .component('main', {
    templateUrl: '/views/home.html',
    controller: ['$http', function Controller($http) {
            var self = this;
        }]
});
angular
    .module('domains', [])
    .component('domains', {
    templateUrl: '/views/domains.html',
    controller: ['$http', '$scope', '$mdDialog', function Controller($http, $scope, $mdDialog) {
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
                self.isLoading = true;
                $http.get(url).then(function (response) {
                    self.totalCount = response.data.total;
                    self.domains = response.data.domains;
                    self.isLoading = false;
                });
            };
            self.pageChangeHandler = function (num) {
                self.load(num);
            };
            $http.get('/api/store').then(function (response) {
                self.stores = response.data;
            });
            $scope.$watch('$ctrl.filterStore + $ctrl.filterDomain + $ctrl.filterAddress', function () {
                self.currentPage = 1;
                self.load(1);
            }, true);
            self.editDomain = function (e, domain) {
                function EditDomainController($http, $scope, $mdDialog, stores, domain) {
                    $scope.stores = stores;
                    $scope.store = '0';
                    $scope.domain = domain;
                    $scope.ip = '';
                    $scope.description = '';
                    $scope.isLoading = true;
                    $http.get('/api/domain/' + domain).then(function (response) {
                        $scope.store = '' + response.data.store;
                        $scope.ip = response.data.ip;
                        $scope.description = response.data.description;
                        $scope.isLoading = false;
                    }, function () {
                        console.log('error');
                    });
                    $scope.answer = function (result) {
                        if (result) {
                            $mdDialog.hide({
                                store: $scope.store,
                                domain: $scope.domain,
                                ip: $scope.ip,
                                description: $scope.description
                            });
                        }
                        else {
                            $mdDialog.hide();
                        }
                    };
                }
                $mdDialog.show({
                    controller: EditDomainController,
                    templateUrl: '/dialogs/edit-domain.html',
                    clickOutsideToClose: true,
                    locals: {
                        stores: self.stores,
                        domain: domain
                    }
                }).then(function (result) {
                    if (result) {
                        $http({
                            method: 'PUT',
                            url: '/api/domain/' + domain,
                            data: JSON.stringify(result)
                        }).then(function (response) {
                            self.load(self.currentPage);
                        }, function (response) {
                            $mdDialog.show($mdDialog.alert()
                                .clickOutsideToClose(true)
                                .title('Failed')
                                .textContent('Failed to edit domain.')
                                .ok('Close'));
                        });
                    }
                }, function () {
                });
                e.preventDefault();
                return false;
            };
            self.removeDomain = function (e, domain) {
                $mdDialog.show($mdDialog.confirm()
                    .clickOutsideToClose(true)
                    .title('Remove domain')
                    .textContent('Are you sure you wish to remove this domain? This action cannot be reversed.')
                    .ok('Remove')
                    .cancel('Cancel'))
                    .then(function (result) {
                    if (result) {
                        $http({
                            method: 'DELETE',
                            url: '/api/domain/' + domain
                        }).then(function (response) {
                            self.load(self.currentPage);
                        }, function (response) {
                            $mdDialog.show($mdDialog.alert()
                                .clickOutsideToClose(true)
                                .title('Failed')
                                .textContent('Failed to delete domain.')
                                .ok('Close'));
                        });
                    }
                }, function () {
                    console.log('cancel remove');
                });
                e.preventDefault();
                return false;
            };
            self.addDomainModel = {
                store: '1',
                domain: '',
                ip: '',
                description: ''
            };
            self.addDomain = function (e) {
                $http({
                    method: 'POST',
                    url: '/api/domain',
                    data: JSON.stringify(self.addDomainModel)
                }).then(function (response) {
                    self.addDomainModel.domain = '';
                    self.addDomainModel.ip = '';
                    self.addDomainModel.description = '';
                    self.load(self.currentPage);
                }, function (response) {
                    $mdDialog.show($mdDialog.alert()
                        .clickOutsideToClose(true)
                        .title('Failed')
                        .textContent('Failed to add domain.')
                        .ok('Close'));
                });
                e.preventDefault();
            };
        }]
});
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
            });
        }]
});
angular
    .module('hits', [])
    .component('hits', {
    templateUrl: '/views/hits.html',
    controller: ['$http', function Controller($http) {
            var self = this;
        }]
});
angular
    .module('whdApp')
    .filter('addCommas', function () {
    return function (str) {
        return addCommas(str);
    };
});
angular
    .module('whdApp')
    .filter('microTime', function () {
    return function (micro) {
        if (micro >= 1000.0) {
            micro /= 1000.0;
            if (micro >= 1000.0) {
                micro /= 1000.0;
                return addCommas('' + micro) + ' seconds';
            }
            return addCommas('' + micro) + ' ms';
        }
        return addCommas('' + micro) + ' us';
    };
});
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
                });
            };
            $interval(self.load, 2000);
            self.load();
        }]
});
angular
    .module('stores', [])
    .component('stores', {
    templateUrl: '/views/stores.html',
    controller: ['$http', '$mdDialog', function Controller($http, $mdDialog) {
            var self = this;
            function loadStores() {
                $http.get('/api/store').then(function (response) {
                    self.stores = response.data;
                });
            }
            loadStores();
            self.addStore = function (storeName) {
                if (storeName) {
                    $http({
                        method: 'POST',
                        url: '/api/store',
                        data: JSON.stringify({
                            name: storeName
                        })
                    }).then(function (response) {
                        loadStores();
                    }, function (response) {
                        $mdDialog.show($mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Failed')
                            .textContent('Failed to add store. Make sure the store doesn\' already exist.')
                            .ok('Close'));
                    });
                }
            };
            self.renameStoreItem = function (e, id, name) {
                var dlg = $mdDialog.prompt()
                    .clickOutsideToClose(true)
                    .title('Rename store')
                    .textContent('Enter new name of store')
                    .placeholder('Store name')
                    .ok('Rename')
                    .cancel('Cancel');
                $mdDialog.show(dlg).then(function (result) {
                    $http({
                        method: 'PUT',
                        url: '/api/store/' + id,
                        data: JSON.stringify({
                            name: result
                        })
                    }).then(function (response) {
                        loadStores();
                    }, function (response) {
                        $mdDialog.show($mdDialog.alert()
                            .clickOutsideToClose(true)
                            .title('Failed')
                            .textContent('Failed to rename store. Make sure the store doesn\' already exist.')
                            .ok('Close'));
                    });
                }, function () {
                });
                e.preventDefault();
                return false;
            };
            self.removeStoreItem = function (e, id, name) {
                $mdDialog.show({
                    controller: DialogController,
                    templateUrl: '/dialogs/confirm-delete-store.html',
                    clickOutsideToClose: true
                }).then(function (result) {
                    if (result) {
                        $http({
                            method: 'DELETE',
                            url: '/api/store/' + id
                        }).then(function (response) {
                            loadStores();
                        }, function (response) {
                            $mdDialog.show($mdDialog.alert()
                                .clickOutsideToClose(true)
                                .title('Failed')
                                .textContent('Failed to delete store.')
                                .ok('Close'));
                        });
                    }
                }, function () {
                });
                e.preventDefault();
                return false;
            };
        }]
});
//# sourceMappingURL=app.js.map