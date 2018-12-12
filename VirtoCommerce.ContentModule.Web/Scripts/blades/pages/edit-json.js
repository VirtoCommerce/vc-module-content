angular.module('virtoCommerce.contentModule')
    .controller('virtoCommerce.contentModule.editJsonController', ['$rootScope', '$scope', 'platformWebApp.validators', 'virtoCommerce.contentModule.contentApi', 'platformWebApp.dynamicProperties.api', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.dynamicProperties.dictionaryItemsApi', 'platformWebApp.settings',
        function ($rootScope, $scope, validators, contentApi, dynamicPropertiesApi, bladeNavigationService, dialogService, dictionaryItemsApi, settings) {
    var blade = $scope.blade;
    blade.updatePermission = 'content:update';
    blade.designerUrl = null;
    var designerUrlPromise = settings.getValues({ id: 'VirtoCommerce.Content.DesignerUrl' }).$promise;
    $scope.validators = validators;

    var userName = 'draft';

    blade.initialize = function () {
        designerUrlPromise.then(function (promiseData) {
            blade.designerUrl = promiseData;
            if (blade.designerUrl == '')
                blade.designerUrl = null;
        });
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

                $scope.blade.currentEntity.blocks = blade.currentEntity.content;
                $scope.blade.currentEntity.settings = $scope.blade.currentEntity.blocks[0];
                $scope.blade.currentEntity.content = JSON.stringify($scope.blade.currentEntity.blocks);

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

        $scope.blade.currentEntity.name = originFileName;
        $scope.blade.currentEntity.content = JSON.stringify($scope.blade.currentEntity.blocks, null, 4);

        blade.isLoading = true;

        contentApi.saveMultipartContent({
            contentType: blade.contentType,
            storeId: blade.storeId,
            folderUrl: blade.folderUrl || ''
        }, $scope.blade.currentEntity,
            function () {
                blade.isLoading = false;
                blade.origEntity = angular.copy(blade.currentEntity);
                if (blade.isNew) {
                    $scope.bladeClose();
                    $rootScope.$broadcast("cms-statistics-changed", blade.storeId);
                }

                blade.parentBlade.refresh();
                if (blade.isNew) {
                    runDesigner(fileName);
                }
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
                name: "content.commands.preview-page", icon: 'fa fa-eye',
                executeMethod: function () {
                    if (blade.storeUrl) {
                        var fileNameArray = blade.currentEntity.relativeUrl.split('.');
                        var fileName = _.first(fileNameArray);
                        var locale = '';
                        if (_.size(fileNameArray) > 2)
                            locale = '/' + fileNameArray[1];
                        var contentType = '/' + blade.contentType;
                        var permalink = blade.currentEntity.settings.permalink;
                        var page = '/' + (permalink && permalink.length
                            ? permalink.replace(/^\/+|\/+$/g, '')
                            : fileName.replace(/^\/+|\/+$/g, ''));
                        window.open(blade.storeUrl + locale + contentType + page, '_blank');
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
            },
            {
                name: "content.commands.open-designer", icon: 'fa fa-crop',
                executeMethod: function () {
                    var fileNameArray = blade.currentEntity.relativeUrl.split('.');
                    var fileName = _.first(fileNameArray);
                    var locale = '';
                    if (_.size(fileNameArray) > 2)
                        locale = fileNameArray[1];
                    var contentType = blade.contentType;

                    // overwrite the name for now
                    fileName = $scope.blade.currentEntity.name;
                    runDesigner(fileName);
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

    function runDesigner(fileName) {
        if (blade.designerUrl) {
            window.open(blade.designerUrl + '?path=' + fileName + '&storeId=' + blade.storeId + '&contentType=' + blade.contentType, '_blank');
        } else {
            var dialog = {
                id: "noUrlInStore",
                title: "content.dialogs.set-designer-url.title",
                message: "content.dialogs.set-designer-url.message"
            };
            dialogService.showNotificationDialog(dialog);
        }
    }

    blade.onClose = function (closeCallback) {
        bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "content.dialogs.page-save.title", "content.dialogs.page-save.message");
    };

    var formScope;
    $scope.setForm = function (form) { $scope.formScope = formScope = form; };

    var getDictionaryValuesCallback;
    $scope.getDictionaryValues = function (property, callback) {
        getDictionaryValuesCallback = callback;
        dictionaryItemsApi.query({ id: property.objectType, propertyId: property.id }, callback);
    };

    $scope.languages = settings.getValues({ id: 'VirtoCommerce.Core.General.Languages' });
    // todo: read from blocks schema json
    $scope.options = [
        { label: "Theme", value: "theme" },
        { label: "Empty", value: "empty" },
        { label: "Custom", value: "custom" }
    ];
    blade.headIcon = 'fa-inbox';

    blade.initialize();
}]);
