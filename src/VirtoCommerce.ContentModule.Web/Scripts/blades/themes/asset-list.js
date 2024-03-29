angular.module('platformWebApp')
    .controller('virtoCommerce.contentModule.assetListController', ['$scope', '$translate', 'virtoCommerce.contentModule.contentApi', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.uiGridHelper', 'platformWebApp.bladeUtils', 'platformWebApp.validators',
        function ($scope, $translate, contentApi, bladeNavigationService, dialogService, uiGridHelper, bladeUtils, validators) {
            var blade = $scope.blade;

            blade.refresh = function () {
                blade.isLoading = true;
                contentApi.query(
                    {
                        contentType: blade.contentType,
                        storeId: blade.storeId,
                        keyword: blade.searchKeyword,
                        folderUrl: blade.currentEntity.relativeUrl
                    },
                    function (data) {
                        $scope.pageSettings.totalItems = data.length;
                        _.each(data, function (x) {
                            x.isImage = x.mimeType && x.mimeType.startsWith('image/');
                            x.isOpenable = x.mimeType && (x.mimeType.startsWith('application/j')
                                    || x.mimeType.startsWith('text/') || x.name.endsWith('.page') || x.name.endsWith('.template'));
                        });
                        $scope.listEntries = data;
                        blade.isLoading = false;

                        //Set navigation breadcrumbs
                        setBreadcrumbs();
                    }, function (error) {
                        bladeNavigationService.setError('Error ' + error.status, blade);
                    });
            };

            function newFolder() {
                var tooltip = $translate.instant('platform.dialogs.create-folder.title');

                var result = prompt(tooltip + "\n\nEnter folder name:");

                if (result != null) {
                    contentApi.createFolder({ contentType: blade.contentType, storeId: blade.storeId }, { name: result, parentUrl: blade.currentEntity.url }, blade.refresh);
                }
            }

            $scope.copyUrl = function (data) {
                window.prompt("Copy to clipboard: Ctrl+C, Enter", data.url);
            };

            $scope.downloadUrl = function (data) {
                window.open(data.url, '_blank');
            };

            $scope.rename = function (listItem) {
                var isFolder = /\/$/.test(listItem.url);
                var result = prompt("Enter new name", listItem.name);
                // if rename folder, then ulr name ends '/'
                var substrNameLenght = isFolder ? listItem.name.length + 1 : listItem.name.length;

                if (result) {
                    if (validators.webSafeFileNameValidator(result)) {
                        contentApi.move({
                            contentType: blade.contentType,
                            storeId: blade.storeId,
                            oldUrl: listItem.url,
                            newUrl: listItem.url.substring(0, listItem.url.length - substrNameLenght) + result
                        }, blade.refresh,
                            function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
                    } else {
                        var errorMessage = $translate.instant('content.blades.edit-asset.validations.name-invalid');
                        alert(errorMessage);
                    }
                }
            };

            function isItemsChecked() {
                return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
            }

            function isSingleChecked() {
                return isItemsChecked() && $scope.gridApi.selection.getSelectedRows().length === 1;
            }

            $scope.delete = function (data) {
                deleteList([data]);
            };

            function deleteList(selection) {
                bladeNavigationService.closeChildrenBlades(blade, function () {
                    var dialog = {
                        id: "confirmDeleteItem",
                        title: "platform.dialogs.folders-delete.title",
                        message: "platform.dialogs.folders-delete.message",
                        callback: function (remove) {
                            if (remove) {
                                var listEntryIds = _.pluck(selection, 'url');
                                contentApi.delete({
                                    contentType: blade.contentType,
                                    storeId: blade.storeId,
                                    urls: listEntryIds
                                },
                                    blade.refresh,
                                    function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
                            }
                        }
                    }
                    dialogService.showConfirmationDialog(dialog);
                });
            }

            $scope.selectNode = function (listItem) {
                if (listItem.type === 'folder') {
                    var newBlade = {
                        id: blade.id,
                        contentType: blade.contentType,
                        storeId: blade.storeId,
                        themeId: blade.themeId,
                        currentEntity: listItem,
                        breadcrumbs: blade.breadcrumbs,
                        controller: blade.controller,
                        template: blade.template,
                        disableOpenAnimation: true,
                        isClosingDisabled: blade.isClosingDisabled
                    };
                    bladeNavigationService.showBlade(newBlade, blade.parentBlade);
                } else {
                    blade.selectedNodeId = listItem.relativeUrl;
                    openDetailsBlade(listItem, false);
                }
            };

            function openDetailsBlade(listItem, isNew) {
                if (isNew || listItem.isOpenable) {
                    var newBlade = {
                        id: 'assetDetail',
                        contentType: blade.contentType,
                        storeId: blade.storeId,
                        themeId: blade.themeId,
                        currentEntity: listItem,
                        isNew: isNew,
                        controller: 'virtoCommerce.contentModule.assetDetailController',
                        template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/themes/asset-detail.tpl.html'
                    };

                    if (isNew) {
                        angular.extend(newBlade, {
                            folderUrl: blade.currentEntity.relativeUrl,
                            title: 'content.blades.edit-asset.title-new',
                            subtitle: 'content.blades.edit-asset.subtitle-new'
                        });
                    } else {
                        angular.extend(newBlade, {
                            folderUrl: listItem.relativeUrl.substring(0, listItem.relativeUrl.length - listItem.name.length - 1),
                            title: listItem.name,
                            subtitle: 'content.blades.edit-asset.subtitle'
                        });
                    }
                    bladeNavigationService.showBlade(newBlade, blade);
                }
            }

            blade.toolbarCommands = [
                {
                    name: "platform.commands.refresh", icon: 'fa fa-refresh',
                    executeMethod: blade.refresh,
                    canExecuteMethod: function () {
                        return true;
                    }
                },
                {
                    name: "platform.commands.new-folder", icon: 'fa fa-folder-o',
                    executeMethod: function () { newFolder(); },
                    canExecuteMethod: function () { return true; },
                    permission: 'content:create'
                },
                {
                    name: "platform.commands.add", icon: 'fa fa-plus',
                    executeMethod: function () {
                        openDetailsBlade({}, true);
                    },
                    canExecuteMethod: function () {
                        return true;
                    },
                    permission: 'content:create'
                },
                {
                    name: "platform.commands.upload", icon: 'fa fa-upload',
                    executeMethod: function () {
                        var newBlade = {
                            id: "assetUpload",
                            contentType: blade.contentType,
                            storeId: blade.storeId,
                            currentEntityId: blade.currentEntity.relativeUrl,
                            title: 'platform.blades.asset-upload.title',
                            controller: 'virtoCommerce.contentModule.assetUploadController',
                            template: 'Modules/$(VirtoCommerce.Assets)/Scripts/blades/asset-upload.tpl.html'
                        };
                        bladeNavigationService.showBlade(newBlade, blade);
                    },
                    canExecuteMethod: function () {
                        return true;
                    },
                    permission: 'content:create'
                },
                //{
                //    name: "Rename", icon: 'fa fa-font',
                //    executeMethod: function () {
                //        $scope.rename($scope.gridApi.selection.getSelectedRows()[0]);
                //    },
                //    canExecuteMethod: isSingleChecked,
                //    permission: 'content:update'
                //},
                {
                    name: "platform.commands.delete", icon: 'fa fa-trash-o',
                    executeMethod: function () { deleteList($scope.gridApi.selection.getSelectedRows()); },
                    canExecuteMethod: isItemsChecked,
                    permission: 'content:delete'
                }
            ];

            // ui-grid
            $scope.setGridOptions = function (gridOptions) {
                uiGridHelper.initialize($scope, gridOptions,
                    function (gridApi) {
                        $scope.$watch('pageSettings.currentPage', gridApi.pagination.seek);
                    });
            };
            bladeUtils.initializePagination($scope, true);

            //Breadcrumbs
            function setBreadcrumbs() {
                if (blade.breadcrumbs) {
                    //Clone array (angular.copy leaves the same reference)
                    var breadcrumbs = blade.breadcrumbs.slice(0);

                    //prevent duplicate items
                    if (blade.currentEntity.relativeUrl && _.all(breadcrumbs, function (x) { return x.id !== blade.currentEntity.relativeUrl; })) {
                        var breadCrumb = generateBreadcrumb(blade.currentEntity.relativeUrl, blade.currentEntity.name);
                        breadcrumbs.push(breadCrumb);
                    }
                    blade.breadcrumbs = breadcrumbs;
                } else {
                    blade.breadcrumbs = [generateBreadcrumb(blade.currentEntity.relativeUrl, 'all')];
                }
            }

            function generateBreadcrumb(id, name) {
                return {
                    id: id,
                    name: name,
                    blade: blade,
                    navigate: function (breadcrumb) {
                        bladeNavigationService.closeBlade(blade,
                            function () {
                                blade.disableOpenAnimation = true;
                                bladeNavigationService.showBlade(blade, blade.parentBlade);
                            });
                    }
                }
            }

            blade.headIcon = 'fa fa-folder-o';
            blade.title = blade.currentEntity.name;
            blade.refresh();
        }
    ]);
