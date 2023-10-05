/**
 * ng-common js on 15-Nov-2018
 */

/*ngGridApp.controller('ngGridCtrl', ['$scope', '$http', '$log', '$filter', '$timeout', '$interval', '$rootScope', 'modal', function ($scope, $http, $log, $filter, $timeout, $interval, $rootScope, modal) {
        

    // Modal Filter By RowId
    $scope.modalFilterByRowId = function (renderableRows) {
        var matcher = new RegExp($rootScope.RowId.toString());
        renderableRows.forEach(function (row) {
            var match = false;
            ['RowId'].forEach(function (field) {
                if (row.entity[field] && row.entity[field] == $rootScope.RowId) {
                    match = true;
                }
            });
            if (!match) {
                row.visible = false;
            }
        });
        return renderableRows;
    };

}]);*/