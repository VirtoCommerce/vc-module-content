angular
    .module('virtoCommerce.contentModule')
    .controller('virtoCommerce.contentModule.storeUrlListController', ['$scope', 'platformWebApp.bladeNavigationService', '$localStorage', 'platformWebApp.validators', function ($scope, bladeNavigationService, $localStorage, validators) {
        const blade = $scope.blade;
        blade.headIcon = 'fa-key';
        const nameStoreUrlList = 'storeUrlList-' + blade.storeId;

        blade.refresh = function() {
            if ($localStorage[nameStoreUrlList]) {
                blade.currentEntities = $localStorage[nameStoreUrlList];
            } else {
                blade.currentEntities = [];
            }

            blade.isLoading = false;
        }

        $scope.selectNode = function(storeUrl) {
            $scope.selectNodeId = storeUrl;
            openUrlForPreview(storeUrl);
        }

        function openUrlForPreview(url) {
            const fileNameArray = blade.relativeUrl.split('.');
            const fileName = _.first(fileNameArray);
            let locale = '';

            if (_.size(fileNameArray) > 2) {
                locale = '/' + fileNameArray[1];
            }

            const contentType = '/' + blade.contentType;

            window.open(url.replace(/\/$/, '') + locale + contentType + fileName, '_blank');
        }

        function editstoreUrls() {
            const newBlade = {
                id: "editRedirectUris",
                updatePermission: 'platform:security:update',
                data: blade.currentEntities,
                validator: validators.uriWithoutQuery,
                headIcon: 'fa-plus-square-o',
                controller: 'platformWebApp.editArrayController',
                template: '$(Platform)/Scripts/common/blades/edit-array.tpl.html',
                onChangesConfirmedFn: function (values) {
                    $localStorage[nameStoreUrlList] = angular.copy(values);
                    blade.refresh();
                }
            };

            bladeNavigationService.showBlade(newBlade, blade);
        }

        blade.toolbarCommands = [
            {
                name: "platform.commands.refresh",
                icon: "fa fa-refresh",
                executeMethod: blade.refresh,
                canExecuteMethod: function() { return true; }
            },
            {
                name: "platform.commands.manage",
                icon: "fa fa-edit",
                executeMethod: editstoreUrls,
                canExecuteMethod: function () { return true; }
            }
        ];

        blade.refresh();
    }]);
