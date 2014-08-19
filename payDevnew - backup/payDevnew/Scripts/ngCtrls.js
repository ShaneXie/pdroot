var appPM = angular.module('paymapApp', []);

appPM.controller('paymapCtrl', function ($scope, $http) {
    $scope.baseUrl = "http://localhost/";
    $scope.newTitle = "";
    $scope.newURL = "";
    $scope.newPath = "";
    $scope.newKeys = "";

    $http.get('/data/paymap.txt').success(function (data) {
        $scope.paymap = data;
    });

    $scope.save = function () {
        if ($scope.newTitle == "" || $scope.newURL == "" || $scope.newPath == "" || $scope.newKeys == "") {
            $scope.msg = 'Error : All these four field are required.'
            return;
        }

        var str = '[{"title":"' + $scope.newTitle + '","path":"' + $scope.newPath + '","url":"' + $scope.newURL + '","keys":"' + $scope.newKeys + '"}]';
        var json = JSON.parse(str);
        $scope.paymap = $scope.paymap.concat(json);

        var pData = { 'data': JSON.stringify($scope.paymap) };

        var theTitle = $scope.newTitle;

        $scope.newTitle = "";
        $scope.newURL = "";
        $scope.newPath = "";
        $scope.newKeys = "";

        $http.post('/ajaxHandler.aspx', $scope.pData).success(function (data) {
            $scope.msg = 'New Page [' + theTitle + '] Saved!';
        });


    }
});