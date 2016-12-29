/// <reference path="../app.ts" />

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
                            })
                        } else {
                            $mdDialog.hide();
                        }
                    }
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
                })

                e.preventDefault();
                return false;
            }

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
                    })

                e.preventDefault();
                return false;
            }

            self.addDomainModel = {
                store: '1',
                domain: '',
                ip: '',
                description: ''
            }
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
            }
        }]
    })