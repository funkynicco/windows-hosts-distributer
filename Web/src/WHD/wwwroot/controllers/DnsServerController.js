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
//# sourceMappingURL=DnsServerController.js.map