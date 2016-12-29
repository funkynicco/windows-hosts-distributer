/// <reference path="app.ts" />

angular
    .module('whdApp')
    .filter('addCommas', function () {
        return function (str: string) {
            return addCommas(str);
        }
    })

angular
    .module('whdApp')
    .filter('microTime', function () {
        return function (micro: number) {

            if (micro >= 1000.0) {
                micro /= 1000.0;

                if (micro >= 1000.0) {
                    micro /= 1000.0;
                    return addCommas('' + micro) + ' seconds';
                }

                return addCommas('' + micro) + ' ms';
            }

            return addCommas('' + micro) + ' us';
        }
    })