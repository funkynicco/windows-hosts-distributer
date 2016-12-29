/// <reference path="../app.ts" />

angular
    .module('hits', [])
    .component('hits', {
        templateUrl: '/views/hits.html',
        controller: ['$http', '$scope', function Controller($http, $scope) {
            var self = this;

            self.currentPage = 1;
            self.totalCount = 0;
            self.pageSize = 15;
            self.hits = [];

            self.filterDomain = '';

            self.isLoading = false;

            self.load = function (page) {
                var url = '/api/hits?page=' + page + '&ipp=' + self.pageSize;

                url += '&filter-domain=' + escape(self.filterDomain);

                self.isLoading = true;
                $http.get(url).then(function (response) {
                    self.totalCount = response.data.total;
                    self.hits = response.data.hits;
                    self.isLoading = false;
                })
            }

            self.pageChangeHandler = function (num) {
                self.load(num);
            }

            $scope.$watch('$ctrl.filterDomain', function () {
                self.currentPage = 1;
                self.load(1);
            }, true)
        }]
    })