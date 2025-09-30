angular.module('virtoCommerce.contentModule')
    .controller('virtoCommerce.contentModule.pageDetailController', ['$rootScope',
        '$scope',
        'platformWebApp.validators',
        'platformWebApp.dialogService',
        'virtoCommerce.contentModule.contentApi',
        'platformWebApp.bladeNavigationService',
        'platformWebApp.dynamicProperties.api',
        'platformWebApp.settings',
        'FileUploader',
        'platformWebApp.dynamicProperties.dictionaryItemsApi',
        'platformWebApp.i18n',
        'virtoCommerce.searchModule.searchIndexation',
        'moment',
        'virtoCommerce.contentModule.broadcastChannelFactory', 'virtoCommerce.contentModule.files-draft',
        function ($rootScope,
            $scope,
            validators,
            dialogService,
            contentApi,
            bladeNavigationService,
            dynamicPropertiesApi,
            settings,
            FileUploader,
            dictionaryItemsApi,
            i18n,
            searchApi,
            moment, broadcastChannelFactory, filesDraftService) {

            var extension = 'md';
            var momentFormat = "YYYYMMDDHHmmss";

            var blade = $scope.blade;
            blade.currentLanguage = i18n.getLanguage();
            blade.frontMatterHeaders = 'VirtoCommerce.ContentModule.Core.Model.FrontMatterHeaders'

            blade.dynamicPropertiesTotalCount = 0;
            blade.currentEntity.dynamicProperties = [];
            $scope.searchEnabled = false;
            var channel;

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
                    getDocumentIndex();
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

            var formScope;
            $scope.setForm = function (form) {
                $scope.formScope = formScope = form;
            };

            $scope.isInvalid = function () {
                return formScope !== undefined && formScope.$invalid;
            }

            $scope.copyToClipboard = function (elementId) {
                var text = document.getElementById(elementId);
                text.focus();
                text.select();
                document.execCommand('copy');
            }

            blade.initializeBlade = function () {
                channel = broadcastChannelFactory(blade);
                if (blade.isNew) {
                    fillMetadata({});
                } else {
                    contentApi.getWithMetadata({
                        contentType: blade.contentType,
                        storeId: blade.storeId,
                        relativeUrl: blade.currentEntity.relativeUrl
                    },
                        function (data) {
                            fillMetadata(data);
                            updateToolbarCommands();
                        },
                        function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });

                    loadSearchIndex();
                }
            };

            function fillMetadata(data) {
                var blobName = blade.currentEntity.name || '';

                var blobNameParts = blobName.split('.');
                var extension = blobNameParts.length > 1 ? blobNameParts.pop() : ''; // ignore extension

                if (blade.languages && blade.languages.length) {
                    var possibleFileLanguage = blobNameParts.length > 1 ? blobNameParts[blobNameParts.length - 1] : '';

                    var language = blade.languages.find(function (lang) { return lang.toLowerCase() === possibleFileLanguage.toLowerCase(); });

                    if (language) {
                        blobNameParts.pop();
                        blade.currentEntity.language = language;
                    }
                }

                blade.currentEntity.pageName = blobNameParts.join('.');

                blade.currentEntity.content = data.content;
                blade.origEntity = angular.copy(blade.currentEntity);

                blade.hasChanges = blade.currentEntity.hasChanges;
                blade.published = blade.currentEntity.published;

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

                        blade.isLoading = false;
                    },
                    function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
            }

            function fillDynamicProperties(metadata, props) {
                _.each(props,
                    function (x) {
                        var metadataRecord = _.findWhere(metadata, { name: x.name });
                        var values = metadataRecord ? metadataRecord.values : [];

                        if (x.isMultilingual && !x.isDictionary) {
                            if (metadataRecord) {
                                values = _.pluck(metadataRecord.values, 'value');
                            } else {
                                values = _.map(x.displayNames, item => ({ locale: item.locale, value: null }));
                            }
                        }

                        x.values = values;
                    });
                if (props) {
                    blade.currentEntity.dynamicProperties = blade.currentEntity.dynamicProperties.concat(props);
                    blade.origEntity.dynamicProperties = blade.origEntity.dynamicProperties.concat(angular.copy(props));
                }
            }

            $scope.scrolled = () => {
                if ($scope.dynamicPropertiesTotalCount > blade.currentEntity.dynamicProperties.length) {
                    getDynamicProperties(200, blade.currentEntity.dynamicProperties.length);
                }
            };

            $scope.saveChanges = function () {
                blade.isLoading = true;

                var oldRelativeUrl = blade.origEntity && blade.origEntity.relativeUrl;

                blade.currentEntity.name = [blade.currentEntity.pageName, blade.currentEntity.language, extension].filter(x => x).join('.');

                contentApi.saveWithMetadata({
                        contentType: blade.contentType,
                        storeId: blade.storeId,
                        folderUrl: blade.folderUrl || ''
                    },
                    blade.currentEntity,
                    function (result) {
                        blade.isLoading = false;
                        var needRefresh = true;
                        blade.currentEntity = Object.assign(blade.currentEntity, result[0]);
                        fillMetadata(result);
                        //angular.copy(blade.currentEntity, blade.origEntity);
                        if (blade.isNew) {
                            $scope.bladeClose();
                            blade.parentBlade.refresh();
                            $rootScope.$broadcast("cms-statistics-changed", blade.storeId);
                        } else if (oldRelativeUrl && oldRelativeUrl !== blade.currentEntity.relativeUrl) {
                            needRefresh = false;
                            contentApi.delete({
                                contentType: blade.contentType,
                                storeId: blade.storeId,
                                urls: [oldRelativeUrl]
                            }, function () {
                                blade.parentBlade.refresh();
                            });
                        }
                        updateToolbarCommands();
                        broadcastChanges({ published: blade.published, hasChanges: true });
                        if (needRefresh) {
                            blade.parentBlade.refresh();
                        }
                    },
                    function (error) { bladeNavigationService.setError('Error ' + error.status, blade); }
                );
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
                                    setTimeout(blade.parentBlade.refresh, 1000);
                                });
                        }
                    }
                };
                dialogService.showConfirmationDialog(dialog);
            };

            var toolbarCommands = [];

            var publishCommand = {
                name: "content.commands.publish", icon: 'fa fa-file',
                executeMethod: function () {
                    contentApi.publish({
                        contentType: blade.contentType,
                        storeId: blade.storeId,
                        relativeUrl: blade.currentEntity.relativeUrl
                    }, function () {
                        blade.hasChanges = false;
                        blade.published = true;
                        getDocumentIndex();
                        updateToolbarCommands();
                        broadcastChanges({ published: true, hasChanges: false });
                    });
                },
                canExecuteMethod: function () { return true; }
            };
            var unpublishCommand = {
                name: "content.commands.unpublish", icon: 'fa fa-file-alt',
                executeMethod: function () {
                    contentApi.unpublish({
                        contentType: blade.contentType,
                        storeId: blade.storeId,
                        relativeUrl: blade.currentEntity.relativeUrl
                    }, function () {
                        blade.hasChanges = true;
                        blade.published = false;
                        updateToolbarCommands();
                        broadcastChanges({ published: false, hasChanges: true });
                    });
                },
                canExecuteMethod: function () { return true; }
            };


            if (!blade.isNew) {
                toolbarCommands = [
                    {
                        name: "platform.commands.save",
                        icon: 'fa fa-save',
                        executeMethod: $scope.saveChanges,
                        canExecuteMethod: function () { return isDirty() && formScope && formScope.$valid; },
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

            toolbarCommands.push(
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

            blade.toolbarCommands = toolbarCommands;

            function isDirty() {
                return !!blade.origEntity && !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
            }

            function canSave() {
                return blade.currentEntity && blade.currentEntity.name && ((isDirty() && !blade.isNew) || (blade.currentEntity.content && blade.isNew));
            }

            // #region Search index

            function addIndexToolbarButton() {
                blade.toolbarCommands.push({
                    name: "content.commands.preview-index",
                    icon: 'fa fa-file-alt',
                    executeMethod: function () {
                        getDocumentIndex(function (data) {
                            var doc = getSearchDocumentInfo();
                            const searchBlade = {
                                id: 'sesarchDetails',
                                currentEntityId: doc.documentId,
                                currentEntity: blade.currentEntity,
                                data: $scope.index,
                                indexDate: $scope.indexDate,
                                documentType: doc.documentType,
                                controller: 'virtoCommerce.searchModule.indexDetailController',
                                template: 'Modules/$(VirtoCommerce.Search)/Scripts/blades/index-detail.tpl.html'
                            };

                            bladeNavigationService.showBlade(searchBlade, blade);
                        });
                    },
                    canExecuteMethod: function () { return true; }
                });
            }

            function updateIndexStatus(data, doc) {
                if (_.any(data)) {
                    $scope.index = data[0];
                    $scope.indexDate = moment.utc($scope.index.indexationdate, momentFormat);
                }
            }

            function updateToolbarCommands() {
                $scope.blade.toolbarCommands = blade.toolbarCommands.filter(x => x !== publishCommand && x !== unpublishCommand);
                if ($scope.blade.published && !$scope.blade.hasChanges) {
                    $scope.blade.toolbarCommands.splice(4, 0, unpublishCommand);
                } else {
                    $scope.blade.toolbarCommands.splice(4, 0, publishCommand);
                }
            }

            function getSearchDocumentInfo() {
                var relativeUrl = filesDraftService.undraftUrl(blade.currentEntity.relativeUrl);
                var documentId = btoa(`${blade.storeId}::${blade.contentType}::${relativeUrl}`).replaceAll('=', '-');
                var documentType = 'ContentFile';
                return { documentType: documentType, documentId: documentId };
            }

            function getDocumentIndex(callback) {
                if ($scope.searchEnabled) {
                    var doc = getSearchDocumentInfo();
                    searchApi.getDocIndex(doc, function (data) {
                        updateIndexStatus(data, doc);
                        callback && _.any(data) && callback();
                    });
                }
            }

            function loadSearchIndex() {
                if (blade.isNew || !blade.currentEntity.published) {
                    return;
                }
                contentApi.indexedSearchEnabled({}, function (data) {
                    $scope.searchEnabled = data.result;
                    getDocumentIndex(addIndexToolbarButton);
                });
            }

            // #endregion

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

            channel.onmessage = function (event) {
                if (event.data.contentType === blade.contentType &&
                    filesDraftService.undraftUrl(blade.currentEntity.relativeUrl) === filesDraftService.undraftUrl(event.data.relativeUrl)) {
                    blade.currentEntity.hasChanges = event.data.hasChanges;
                    blade.currentEntity.published = event.data.published;
                    blade.hasChanges = blade.currentEntity.hasChanges;
                    blade.published = blade.currentEntity.published;
                    updateToolbarCommands();
                    $scope.$apply();
                }
            };

            function broadcastChanges(msg) {
                msg.content = blade.currentEntity.content;
                msg.dynamicProperties = blade.currentEntity.dynamicProperties;
                channel.postMessage(msg);
            }

        }]);
