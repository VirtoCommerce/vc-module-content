﻿angular.module('virtoCommerce.contentModule')
    .controller('virtoCommerce.contentModule.editJsonController', ['$rootScope', '$scope', 'platformWebApp.validators', 'virtoCommerce.contentModule.contentApi', 'platformWebApp.dynamicProperties.api', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.dynamicProperties.dictionaryItemsApi', 'platformWebApp.settings',
        function ($rootScope, $scope, validators, contentApi, dynamicPropertiesApi, bladeNavigationService, dialogService, dictionaryItemsApi, settings) {
    var blade = $scope.blade;
    blade.updatePermission = 'content:update';
    blade.designerUrl = 'http://localhost:59855/';
    $scope.validators = validators;

    var userName = 'draft';

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
            contentApi.get({
                contentType: blade.contentType,
                storeId: blade.storeId,
                relativeUrl: blade.currentEntity.relativeUrl
            }, 
            function (data) {
                blade.isLoading = false;
                blade.currentEntity.content = JSON.parse(data.data);

                console.log(blade.currentEntity.content);
            
                $scope.blade.currentEntity.blocks = blade.currentEntity.content;
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

        contentApi.saveMultipartContent({
            contentType: blade.contentType,
            storeId: blade.storeId,
            folderUrl: blade.folderUrl || ''
        }, blade.currentEntity,
            function () {
                blade.isLoading = false;
                blade.origEntity = angular.copy(blade.currentEntity);
                if (blade.isNew) {
                    $scope.bladeClose();
                    $rootScope.$broadcast("cms-statistics-changed", blade.storeId);
                }

                blade.parentBlade.refresh();
                window.open(blade.designerUrl + '?file=' + fileName + '&user=' + userName, '_blank');
            }, function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
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
            },
            {
                name: "content.commands.open-designer", icon: 'fa fa-crop',
                executeMethod: function () {
                    if (blade.designerUrl) {
                        var fileNameArray = blade.currentEntity.relativeUrl.split('.');
                        var fileName = _.first(fileNameArray);
                        var locale = '';
                        if (_.size(fileNameArray) > 2)
                            locale = fileNameArray[1];
                        var contentType = blade.contentType;

                        // overwrite the name for now
                        fileName = $scope.blade.currentEntity.name;
                        window.open(blade.designerUrl + '?file=' + fileName + '&user=' + userName + '&locale=' + locale, '_blank');
                    }
                    else {
                        var dialog = {
                            id: "noUrlInStore",
                            title: "content.dialogs.set-store-url.title",
                            message: "content.dialogs.set-store-url.message"
                        }
                        dialogService.showNotificationDialog(dialog);
                    }
                },
                canExecuteMethod: function () { return true; }
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
        bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "content.dialogs.page-save.title", "content.dialogs.page-save.message");
    };

    var formScope;
    $scope.setForm = function (form) { $scope.formScope = formScope = form; };

    $scope.languages = settings.getValues({ id: 'VirtoCommerce.Core.General.Languages' });

    blade.headIcon = 'fa-inbox';

    blade.initialize();
}]);