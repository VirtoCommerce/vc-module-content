angular.module('virtoCommerce.contentModule')
    .controller('virtoCommerce.contentModule.pageDetailController', ['$rootScope',
        '$scope',
        'platformWebApp.validators',
        'platformWebApp.dialogService',
        'virtoCommerce.contentModule.contentApi',
        '$timeout',
        'platformWebApp.bladeNavigationService',
        'platformWebApp.dynamicProperties.api',
        'platformWebApp.settings',
        'FileUploader',
        'platformWebApp.dynamicProperties.dictionaryItemsApi',
        'platformWebApp.i18n',
        function ($rootScope,
            $scope,
            validators,
            dialogService,
            contentApi,
            $timeout,
            bladeNavigationService,
            dynamicPropertiesApi,
            settings,
            FileUploader,
            dictionaryItemsApi,
            i18n) {
            var blade = $scope.blade;
            blade.currentLanguage = i18n.getLanguage();
            blade.frontMatterHeaders = 'VirtoCommerce.ContentModule.Core.Model.FrontMatterHeaders'

            blade.dynamicPropertiesTotalCount = 0;
            blade.currentEntity.dynamicProperties = [];

            $scope.validators = validators;
            var contentType = blade.contentType.substr(0, 1).toUpperCase() +
                blade.contentType.substr(1, blade.contentType.length - 1);
            $scope.fileUploader = new FileUploader({
                url: 'api/assets?folderUrl=cms-content/' + contentType + '/' + blade.storeId + '/assets',
                headers: { Accept: 'application/json' },
                autoUpload: true,
                removeAfterUpload: true,
                onBeforeUploadItem: function (fileItem) {
                    blade.isLoading = true;
                },
                onSuccessItem: function (fileItem, response, status, headers) {
                    $scope.$broadcast('filesUploaded', { items: response });
                },
                onErrorItem: function (fileItem, response, status, headers) {
                    bladeNavigationService.setError(
                        fileItem._file.name + ' failed: ' + (response.message ? response.message : status),
                        blade);
                },
                onCompleteAll: function () {
                    blade.isLoading = false;
                }
            });

            blade.editAsMarkdown = true;
            blade.editAsHtml = false;

            var formScopes = [];
            $scope.setForm = function (form) {
                formScopes.push(form);
            };

            blade.initializeBlade = function () {
                if (blade.isNew) {
                    fillMetadata({});
                } else {
                    contentApi.getWithMetadata({
                        contentType: blade.contentType,
                        storeId: blade.storeId,
                        relativeUrl: blade.currentEntity.relativeUrl
                    },
                        fillMetadata,
                        function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
                }
            };

            function fillMetadata(data) {
                var blobName = blade.currentEntity.name || '';
                var idx = blobName.lastIndexOf('.');
                if (idx >= 0) {
                    blobName = blobName.substring(0, idx);
                    idx = blobName.lastIndexOf('.'); // language
                    if (idx >= 0) {
                        blade.currentEntity.language = blobName.substring(idx + 1);
                    }
                }

                blade.currentEntity.content = data.content;
                $scope.metadata = data.metadata;

                getDynamicProperties();
            }

            function getDynamicProperties(take, skip) {
                blade.isLoading = true;

                dynamicPropertiesApi.search({ objectType: 'VirtoCommerce.ContentModule.Core.Model.FrontMatterHeaders', take: take || 200, skip: skip || 0 },
                    function (results) {
                        fillDynamicProperties($scope.metadata, results.results);

                        $scope.dynamicPropertiesTotalCount = results.totalCount;

                        $scope.$broadcast('resetContent', { body: blade.currentEntity.content });
                        $scope.$broadcast('scrollCompleted');

                        $timeout(function () {
                            blade.origEntity = angular.copy(blade.currentEntity);
                        });

                        blade.isLoading = false;
                    },
                    function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
            }

            function fillDynamicProperties(metadata, props) {
                _.each(props,
                    function (x) {
                        var metadataRecord = _.findWhere(metadata, { name: x.name });
                        if (metadataRecord && x.isMultilingual && !x.isDictionary) {
                            metadataRecord.values = _.pluck(metadataRecord.values, 'value');
                        }

                        x.values = metadataRecord ? metadataRecord.values : [];
                    });

                if (props)
                    blade.currentEntity.dynamicProperties = blade.currentEntity.dynamicProperties.concat(props);
            }

            $scope.scrolled = () => {
                if ($scope.dynamicPropertiesTotalCount > blade.currentEntity.dynamicProperties.length) {
                    getDynamicProperties(200, blade.currentEntity.dynamicProperties.length);
                }
            };

            $scope.saveChanges = function () {
                blade.isLoading = true;

                contentApi.saveWithMetadata({
                    contentType: blade.contentType,
                    storeId: blade.storeId,
                    folderUrl: blade.folderUrl || ''
                },
                    blade.currentEntity,
                    function () {
                        blade.isLoading = false;
                        blade.origEntity = angular.copy(blade.currentEntity);
                        if (blade.isNew) {
                            $scope.bladeClose();
                            $rootScope.$broadcast("cms-statistics-changed", blade.storeId);
                        }

                        blade.parentBlade.refresh();
                    },
                    function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
            };

            blade.deleteEntry = function () {
                var dialog = {
                    id: "confirmDelete",
                    title: "content.dialogs.page-delete.title",
                    message: "content.dialogs.page-delete.message",
                    callback: function (remove) {
                        if (remove) {
                            blade.isLoading = true;

                            contentApi.delete({
                                contentType: blade.contentType,
                                storeId: blade.storeId,
                                urls: [blade.currentEntity.url]
                            },
                                function () {
                                    blade.currentEntity = blade.origEntity;
                                    $scope.bladeClose();
                                    blade.parentBlade.refresh();
                                });
                        }
                    }
                };
                dialogService.showConfirmationDialog(dialog);
            };

            if (!blade.isNew) {
                blade.toolbarCommands = [
                    {
                        name: "platform.commands.save",
                        icon: 'fa fa-save',
                        executeMethod: $scope.saveChanges,
                        canExecuteMethod: function () { return isDirty() && formScopes.length && formScopes.every(f => f.$valid); },
                        permission: blade.updatePermission
                    },
                    {
                        name: "platform.commands.reset",
                        icon: 'fa fa-undo',
                        executeMethod: function () {
                            angular.copy(blade.origEntity, blade.currentEntity);
                            $scope.$broadcast('resetContent', { body: blade.currentEntity.content });
                        },
                        canExecuteMethod: isDirty,
                        permission: blade.updatePermission
                    },
                    {
                        name: "content.commands.edit-as-markdown",
                        icon: 'fa fa-code',
                        executeMethod: function () {
                            blade.editAsMarkdown = true;
                            blade.editAsHtml = false;
                            $scope.$broadcast('changeEditType', { editAsMarkdown: true, editAsHtml: false });
                        },
                        canExecuteMethod: function () { return !blade.editAsMarkdown; },
                        permission: blade.updatePermission
                    },
                    {
                        name: "content.commands.edit-as-html",
                        icon: 'fa fa-code',
                        executeMethod: function () {
                            blade.editAsHtml = true;
                            blade.editAsMarkdown = false;
                            $scope.$broadcast('changeEditType', { editAsHtml: true, editAsMarkdown: false });
                        },
                        canExecuteMethod: function () { return !blade.editAsHtml; },
                        permission: blade.updatePermission
                    },
                    {
                        name: "content.commands.preview-page",
                        icon: 'fa fa-eye',
                        executeMethod: function () {
                            const urlListBlade = {
                                id: "storeUrlList",
                                storeId: blade.storeId,
                                contentType: blade.contentType,
                                relativeUrl: blade.currentEntity.relativeUrl,
                                headIcon: 'fa-plus-square-o',
                                controller: 'virtoCommerce.contentModule.storeUrlListController',
                                template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/pages/store-url-list.tpl.html'
                            };

                            bladeNavigationService.showBlade(urlListBlade, blade);
                        },
                        canExecuteMethod: function () { return true; }
                    }
                ];
            }

            blade.toolbarCommands = blade.toolbarCommands || [];
            blade.toolbarCommands.push(
                {
                    name: "content.commands.manage-metadata", icon: 'fa fa-edit',
                    executeMethod: function () {
                        var newBlade = {
                            id: 'dynamicPropertyList',
                            objectType: 'VirtoCommerce.ContentModule.Core.Model.FrontMatterHeaders',
                            parentRefresh: function (props) { fillDynamicProperties(blade.currentEntity.dynamicProperties, props); },
                            controller: 'platformWebApp.dynamicPropertyListController',
                            template: '$(Platform)/Scripts/app/dynamicProperties/blades/dynamicProperty-list.tpl.html'
                        };
                        bladeNavigationService.showBlade(newBlade, blade);
                    },
                    canExecuteMethod: function () { return true; }
                }
            );

            function isDirty() {
                return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
            }

            function canSave() {
                return blade.currentEntity && blade.currentEntity.name && ((isDirty() && !blade.isNew) || (blade.currentEntity.content && blade.isNew));
            }

            blade.onClose = function (closeCallback) {
                bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "content.dialogs.page-save.title", "content.dialogs.page-save.message");
            };

            // dynamic properties (metadata)
            $scope.editDictionary = function (property) {
                var newBlade = {
                    id: "propertyDictionary",
                    isApiSave: true,
                    currentEntity: property,
                    controller: 'platformWebApp.propertyDictionaryController',
                    template: '$(Platform)/Scripts/app/dynamicProperties/blades/property-dictionary.tpl.html',
                    onChangesConfirmedFn: function () {
                        dictionaryItemsApi.query({ id: property.objectType, propertyId: property.id }, getDictionaryValuesCallback);
                    }
                };
                bladeNavigationService.showBlade(newBlade, blade);
            };

            var getDictionaryValuesCallback;
            $scope.getDictionaryValues = function (property, callback) {
                getDictionaryValuesCallback = callback;
                dictionaryItemsApi.query({ id: property.objectType, propertyId: property.id }, callback);
            };

            //settings.getValues({ id: 'VirtoCommerce.Core.General.Languages' }, function (data) {
            //    $scope.languages = data;
            //});
            $scope.languages = settings.getValues({ id: 'VirtoCommerce.Core.General.Languages' });

            blade.headIcon = 'fa fa-file-o';
            blade.initializeBlade();
        }]);
