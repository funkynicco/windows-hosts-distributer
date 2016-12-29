/// <reference path="../app.ts" />

angular
    .module('stores', [])
    .component('stores', {
        templateUrl: '/views/stores.html',
        controller: ['$http', '$mdDialog', function Controller($http, $mdDialog) {
            var self = this;

            function loadStores() {
                $http.get('/api/store').then(function (response) {
                    self.stores = response.data;
                })
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
            }

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
            }

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
                })

                e.preventDefault();
                return false;
            }
        }]
    })