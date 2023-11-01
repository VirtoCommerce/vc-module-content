var moduleTemplateName = "virtoCommerce.contentModule";

angular.module(moduleTemplateName)
    .controller('virtoCommerce.contentModule.chooseActionController',
        ['$scope', function ($scope) {

            var blade = $scope.blade;

            $scope.runHandler = function (handler) {
                handler.execute(blade, blade.parentBlade);
            };

            $scope.blade.isLoading = false;
        }]);
