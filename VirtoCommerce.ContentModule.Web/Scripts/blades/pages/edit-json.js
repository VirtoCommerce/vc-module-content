angular.module('virtoCommerce.contentModule')
.controller('virtoCommerce.contentModule.editJsonController', ['$rootScope', '$scope', 'platformWebApp.validators', 'virtoCommerce.contentModule.cmsGitApi', 'platformWebApp.dynamicProperties.api', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.dynamicProperties.dictionaryItemsApi', 'platformWebApp.settings', function ($rootScope, $scope, validators, cmsGitApi, dynamicPropertiesApi, bladeNavigationService, dialogService, dictionaryItemsApi, settings) {
    var blade = $scope.blade;
    blade.updatePermission = 'content:update';
    $scope.validators = validators;

    var userName = 'draft';
    var cmsDesignerUrl = 'http://localhost:49578/?';

    blade.initialize = function () {
        if (blade.isNew) {
            blade.isLoading = false;
            $scope.blade.currentEntity.blocks = [{ type: 'settings', title: '', permalink: '' }];
            $scope.blade.currentEntity.settings = $scope.blade.currentEntity.blocks[0];

            $scope.blade.currentEntity.content = JSON.stringify($scope.blade.currentEntity.blocks);
        } else {
            var fileName = $scope.blade.currentEntity.name;
            if (!fileName.endsWith('.json')) {
                fileName += '.json';
            }

            cmsGitApi.get({
                content: content,
                storeId: blade.storeId,
                userName: userName,
                fileName: fileName
            },
            null,
            function (data) {
                blade.isLoading = false;
                blade.currentEntity.content = JSON.parse(data.content);

                console.log(blade.currentEntity.content);
            
                $scope.blade.currentEntity.blocks = JSON.parse(blade.currentEntity.content);
                $scope.blade.currentEntity.settings = $scope.blade.currentEntity.blocks[0];
                $scope.blade.currentEntity.content = JSON.stringify($scope.blade.currentEntity.blocks);

                console.log($scope.blade.currentEntity.blocks);

                blade.origEntity = angular.copy(blade.currentEntity);
            },
            function (error) {
                bladeNavigationService.setError('Error ' + error.status, $scope.blade); blade.isLoading = false;
            });
        }
    };

    $scope.saveChanges = function () {
        var fileName = $scope.blade.currentEntity.name;
        if (!fileName.endsWith('.json')) {
            fileName += '.json';
        }

        var originFileName = fileName;

        if (!blade.isNew) {
            originFileName = blade.origEntity.name;
            if (!originFileName.endsWith('.json')) {
                originFileName += '.json';
            }
        }

        $scope.blade.currentEntity.content = JSON.stringify($scope.blade.currentEntity.blocks);
        var content = $scope.blade.currentEntity.content;

        blade.isLoading = true;

        cmsGitApi.exists(
            {
                storeId: blade.storeId,
                userName: userName,
                fileName: fileName
            },
            null,
            function (data) {
                blade.isLoading = false;

                if (data.exists && originFileName != fileName ) {
                    dialogService.showNotificationDialog({
                        id: "fileExists",
                        title: "content.dialogs.json-file-exists.title",
                        message: "content.dialogs.json-file-exists.message"
                    });
                }
                else {
                    cmsGitApi.set({
                        content: content,
                        storeId: blade.storeId,
                        userName: userName,
                        fileName: fileName
                    },
                    null,
                    function (data) {
                        blade.isLoading = false;
                        blade.origEntity = angular.copy(blade.currentEntity);
                        if (blade.isNew) {
                            $scope.bladeClose();
                            $rootScope.$broadcast("cms-statistics-changed", blade.storeId);
                        }

                        blade.parentBlade.refresh();

                        window.open(cmsDesignerUrl + 'file=' + fileName + '&user=' + userName, '_blank');
                    },
                    function (error) {
                        bladeNavigationService.setError('Error ' + error.status, $scope.blade); blade.isLoading = false;
                    });
                }
                blade.parentBlade.refresh();
            },
            function (error) {
                bladeNavigationService.setError('Error ' + error.status, $scope.blade);
                blade.isLoading = false;
            }
        );
    };

    if (!blade.isNew) {
        blade.toolbarCommands = [
            {
                name: "platform.commands.save", icon: 'fa fa-save',
                executeMethod: $scope.saveChanges,
                canExecuteMethod: canSave,
                permission: blade.updatePermission
            },
            {
                name: "platform.commands.reset", icon: 'fa fa-undo',
                executeMethod: function () {
                    angular.copy(blade.origEntity, blade.currentEntity);
                },
                canExecuteMethod: isDirty,
                permission: blade.updatePermission
            }
        ];
    }

    blade.toolbarCommands = blade.toolbarCommands || [];

    function isDirty() {
        return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
    }

    function canSave() {
        return isDirty() && formScope && formScope.$valid;
    }

    blade.onClose = function (closeCallback) {
        bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "content.dialogs.json-save.title", "content.dialogs.json-save.message");
    };

    var formScope;
    $scope.setForm = function (form) { $scope.formScope = formScope = form; };

    $scope.languages = settings.getValues({ id: 'VirtoCommerce.Core.General.Languages' });

    blade.headIcon = 'fa-inbox';

    blade.initialize();
}]);