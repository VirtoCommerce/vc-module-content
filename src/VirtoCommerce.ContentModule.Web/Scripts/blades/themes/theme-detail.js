angular.module('virtoCommerce.contentModule')
    .controller('virtoCommerce.contentModule.themeDetailController', ['$rootScope', '$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.contentModule.contentApi', function ($rootScope, $scope, bladeNavigationService, contentApi) {
        var blade = $scope.blade;

        blade.refresh = function (parentRefresh) {
            if (blade.isNew) {
                initializeBlade({});
            } else {
                initializeBlade(blade.data);
                if (parentRefresh) {
                    blade.parentBlade.refresh();
                }
            }
        };

        function initializeBlade(data) {
            if (blade.isNew) {
                blade.title = 'content.blades.theme-detail.title-new';
                blade.subtitle = 'content.blades.theme-detail.subtitle-new';
            } else {
                blade.title = data.name;
                blade.subtitle = 'content.blades.theme-detail.subtitle';
            }
            blade.currentEntity = angular.copy(data);
            blade.origEntity = data;
            blade.isLoading = false;
        }

        function isDirty() {
            return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
        }

        function canSave() {
            return isDirty() && $scope.formScope && $scope.formScope.$valid;
        }

        $scope.saveChanges = function () {
            blade.isLoading = true;

            if (blade.isNew) {
                if (blade.currentEntity.defaultTheme) { // create from default
                    contentApi.copy({
                        srcPath: 'Themes/' + blade.currentEntity.defaultTheme,
                        destPath: 'Themes/' + blade.storeId + '/' + blade.currentEntity.name
                    }, onAfterThemeCreated,
                        function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
                } else { // create empty
                    contentApi.createFolder({
                        contentType: 'themes',
                        storeId: blade.storeId
                    }, blade.currentEntity, onAfterThemeCreated,
                        function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
                }
            } else {
                var newUrl = '/' + blade.currentEntity.name;

                var isDefault = blade.origEntity.name === blade.defaultThemeName;

                contentApi.move({
                    contentType: 'themes',
                    storeId: blade.storeId,
                    oldUrl: blade.origEntity.relativeUrl,
                    newUrl: newUrl
                }, function (data) {
                    blade.currentEntity.url = newUrl;
                    blade.data = blade.currentEntity;

                    if (isDefault) {
                        setDefaultTheme(blade.currentEntity.name);
                    }

                    refreshParentAndClose();
                }, function (error) {
                    bladeNavigationService.setError('Error ' + error.status, blade);
                });
            }
        };

        function onAfterThemeCreated() {
            if (blade.isActivateAfterSave) {
                setDefaultTheme(blade.currentEntity.name);
            } else {
                refreshParentAndClose();
            }
        }

        function setDefaultTheme(themeName) {
            var prop = _.findWhere(blade.store.dynamicProperties, { name: 'DefaultThemeName' });
            if (prop) {
                prop.values = [{ value: themeName }];
            }

            blade.store.$update(refreshParentAndClose, function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
        }

        function refreshParentAndClose() {
            angular.copy(blade.currentEntity, blade.origEntity);
            $scope.bladeClose();
            blade.parentBlade.refresh();
            $rootScope.$broadcast("cms-statistics-changed", blade.storeId);
        }

        $scope.setForm = function (form) { $scope.formScope = form; };

        blade.onClose = function (closeCallback) {
            bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "content.dialogs.theme-save.title", "content.dialogs.theme-save.message");
        };

        if (!blade.isNew) {
            blade.toolbarCommands = [
                {
                    name: "platform.commands.save",
                    icon: 'fa fa-save',
                    executeMethod: $scope.saveChanges,
                    canExecuteMethod: canSave,
                    permission: blade.updatePermission
                },
                {
                    name: "platform.commands.reset",
                    icon: 'fa fa-undo',
                    executeMethod: function () {
                        angular.copy(blade.origEntity, blade.currentEntity);
                    },
                    canExecuteMethod: isDirty,
                    permission: blade.updatePermission
                }
            ];
        }

        blade.refresh(false);
    }]);
