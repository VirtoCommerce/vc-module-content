angular.module('virtoCommerce.contentModule')
    .controller('virtoCommerce.contentModule.pagesListController',
        ['$rootScope', '$scope', '$translate', 'virtoCommerce.contentModule.contentApi',
            'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService',
            'platformWebApp.uiGridHelper', 'platformWebApp.bladeUtils', 'platformWebApp.validators',
            'virtoCommerce.contentModule.fileHandlerFactory',
            'virtoCommerce.contentModule.broadcastChannelFactory', 'virtoCommerce.contentModule.files-draft',
            function ($rootScope, $scope, $translate, contentApi, bladeNavigationService,
                dialogService, uiGridHelper, bladeUtils, validators, fileHandlerFactory, broadcastChannelFactory, filesDraftService) {
                var blade = $scope.blade;
                var channel;
                blade.updatePermission = 'content:update';
                $scope.selectedNodeId = null;
                $scope.searchEnabled = false;

                blade.refresh = function () {
                    blade.isLoading = true;
                    var query = $scope.searchEnabled && blade.searchKeyword ? contentApi.search : contentApi.query;
                    query(
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
                                var handler = fileHandlerFactory.getHandlers('edit', { file: x });
                                x.isOpenable = handler && handler.length;
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
                        contentApi.createFolder(
                            { contentType: blade.contentType, storeId: blade.storeId },
                            { name: result, parentUrl: blade.currentEntity.url },
                            blade.refresh,
                            function (error) { bladeNavigationService.setError('Error ' + error.status, blade); }
                        );
                    }
                }

                $scope.copyUrl = function (data) {
                    window.prompt("Copy to clipboard: Ctrl+C, Enter", data.url);
                };

                $scope.downloadUrl = function (data) {
                    setTimeout(function () {
                        const link = document.createElement('a');
                        link.setAttribute('href', data.url);
                        link.setAttribute('download', "");
                        link.style.display = 'none';
                        document.body.appendChild(link);
                        link.click();
                        document.body.removeChild(link);
                    });
                };

                $scope.duplicate = function (data) {
                    contentApi.copyFile({ srcFile: data.relativeUrl, contentType: blade.contentType, storeId: blade.storeId }, blade.refresh);
                };

                $scope.selectNode = function (listItem) {
                    if (listItem.type === 'folder') {
                        var newBlade = {
                            id: blade.id,
                            contentType: blade.contentType,
                            storeId: blade.storeId,
                            storeUrl: blade.storeUrl,
                            languages: blade.languages,
                            currentEntity: listItem,
                            breadcrumbs: blade.breadcrumbs,
                            title: blade.title,
                            subtitle: blade.subtitle,
                            controller: blade.controller,
                            template: blade.template,
                            disableOpenAnimation: true,
                            isClosingDisabled: blade.isClosingDisabled
                        };
                        bladeNavigationService.showBlade(newBlade, blade.parentBlade);
                    } else {
                        blade.selectedNodeId = listItem.url;
                        openDetailsBlade(listItem, false);
                    }
                };

                function openDetailsBlade(listItem, isNew) {
                    var action = isNew ? 'create' : 'edit';
                    fileHandlerFactory.handleAction(action, { blade: blade, file: listItem });
                }

                function openBlogDetailsBlade(listItem, isNew) {
                    var newBlade = {
                        id: 'blogDetail',
                        contentType: blade.contentType,
                        storeId: blade.storeId,
                        currentEntity: listItem,
                        existedBlogsName: $scope.listEntries.map((blog) => blog.name),
                        isNew: isNew,
                        title: listItem.name,
                        subtitle: 'content.blades.edit-blog.subtitle',
                        controller: 'virtoCommerce.contentModule.editBlogController',
                        template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/pages/edit-blog.tpl.html'
                    };

                    if (isNew) {
                        angular.extend(newBlade, {
                            title: 'content.blades.edit-blog.title-new',
                            subtitle: 'content.blades.edit-blog.subtitle-new',
                        });
                    }

                    bladeNavigationService.showBlade(newBlade, blade);
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
                                    var listEntryIds = _.pluck(selection, 'relativeUrl');
                                    contentApi.delete({
                                        contentType: blade.contentType,
                                        storeId: blade.storeId,
                                        urls: listEntryIds
                                    },
                                        function () {
                                            blade.refresh();
                                            $rootScope.$broadcast("cms-statistics-changed", blade.storeId);

                                            // todo: notify files were removed
                                        },
                                        function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
                                }
                            }
                        }

                        if (isBlogs() && !blade.currentEntity.type) {
                            angular.extend(dialog, {
                                title: 'content.dialogs.blog-delete.title',
                                message: 'content.dialogs.blog-delete.message',
                            });
                        }

                        dialogService.showConfirmationDialog(dialog);
                    });
                }

                function isItemsChecked() {
                    return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
                }

                function isPages() {
                    return blade.contentType === 'pages';
                }

                function isBlogs() {
                    return blade.contentType === 'blogs';
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
                    }
                ];

                if (isPages()) {
                    blade.toolbarCommands.splice(1, 0,
                        {
                            name: 'platform.commands.new-folder', icon: 'fa fa-folder-o',
                            executeMethod: function () { newFolder(); },
                            canExecuteMethod: function () { return true; },
                            permission: 'content:create'
                        },
                        {
                            name: "platform.commands.add", icon: 'fa fa-plus',
                            executeMethod: function () { openDetailsBlade({}, true); },
                            canExecuteMethod: function () { return true; },
                            permission: 'content:create'
                        }
                    );
                } else if (isBlogs()) {
                    if (blade.currentEntity.type && blade.currentEntity.type === 'folder') {
                        blade.toolbarCommands.splice(1, 0, {
                            name: "content.commands.add-post", icon: 'fa fa-plus',
                            executeMethod: function () { openDetailsBlade({}, true); },
                            canExecuteMethod: function () { return true; },
                            permission: 'content:create'
                        });
                    } else {
                        blade.toolbarCommands.splice(1, 0, {
                            name: 'content.commands.add-blog', icon: 'fa fa-plus',
                            executeMethod: function () { openBlogDetailsBlade({}, true); },
                            canExecuteMethod: function () { return true; },
                            permission: 'content:create'
                        });
                    }
                }

                blade.toolbarCommands.push({
                    name: "platform.commands.delete", icon: 'fa fa-trash-o',
                    executeMethod: function () { deleteList($scope.gridApi.selection.getSelectedRows()); },
                    canExecuteMethod: isItemsChecked,
                    permission: 'content:delete'
                });

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
                        if (blade.currentEntity.url && _.all(breadcrumbs, function (x) { return x.id !== blade.currentEntity.url; })) {
                            var breadCrumb = generateBreadcrumb(blade.currentEntity.url, blade.currentEntity.name);
                            breadcrumbs.push(breadCrumb);
                        }
                        blade.breadcrumbs = breadcrumbs;
                    } else {
                        blade.breadcrumbs = [generateBreadcrumb(blade.currentEntity.url, 'all')];
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

                blade.headIcon = isBlogs() ? 'fa fa-inbox' : 'fa fa-folder-o';
                contentApi.indexedSearchEnabled({}, function (data) {
                    $scope.searchEnabled = data.result;
                    blade.refresh();
                });

                channel = broadcastChannelFactory(blade);
                channel.onmessage = function (event) {
                    var contentType = event.data.contentType;
                    if (contentType === blade.contentType) {
                        var relativeUrl = filesDraftService.undraftUrl(event.data.relativeUrl);
                        var entry = $scope.listEntries.find(function (entry) {
                            return filesDraftService.undraftUrl(entry.relativeUrl) === relativeUrl;
                        });
                        if (!!entry) {
                            entry.hasChanges = event.data.hasChanges;
                            entry.published = event.data.published;
                            $scope.$apply();
                        }
                    }
                };
            }]);
