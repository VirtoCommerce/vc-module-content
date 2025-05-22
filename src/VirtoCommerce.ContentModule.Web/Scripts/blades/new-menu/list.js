angular.module('virtoCommerce.contentModule')
    .controller('virtoCommerce.contentModule.newMenuListController', [
        '$scope',
        'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'uiGridConstants', 'platformWebApp.uiGridHelper',
        'virtoCommerce.contentModule.newMenus',
        function ($scope, bladeNavigationService, dialogService, uiGridConstants, uiGridHelper, menus) {
            $scope.uiGridConstants = uiGridConstants;
            const blade = $scope.blade;

            blade.refresh = function () {
                blade.isLoading = true;
                menus.get({ storeId: blade.storeId }, function (data) {
                    blade.currentEntities = data;
                    blade.isLoading = false;
                },
                function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
            };

            blade.selectNode = function (menu, isNew) {
                $scope.selectedNodeId = menu.id;

                const newBlade = {
                    id: 'menuDetailsBlade',
                    storeId: blade.storeId,
                    parentRefresh: blade.refresh,
                    controller: 'virtoCommerce.contentModule.newMenuDetailsController',
                    template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/new-menu/details.html'
                };

                if (isNew) {
                    angular.extend(newBlade, {
                        isNew: true,
                        title: 'content.blades.menu-details.title-new',
                    });
                } else {
                    angular.extend(newBlade, {
                        currentEntityId: menu.id,
                        title: menu.name,
                    });
                }
                bladeNavigationService.showBlade(newBlade, blade);
            }

            blade.deleteMenu = function (selection) {
                bladeNavigationService.closeChildrenBlades(blade, function () {
                    const dialog = {
                        id: "confirmDelete",
                        title: "content.dialogs.menu-list-delete.title",
                        message: "content.dialogs.menu-list-delete.message",
                        callback: function (remove) {
                            if (remove) {
                                blade.isLoading = true;

                                const menuEntryIds = _.pluck(selection, 'id');
                                menus.delete({ storeId: blade.storeId, menuIds: menuEntryIds }, function () {
                                    blade.refresh();
                                    //$rootScope.$broadcast("cms-menus-changed", blade.storeId);
                                },
                                function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
                            }
                        }
                    };
                    dialogService.showConfirmationDialog(dialog);
                });
            };

            blade.headIcon = 'fa fa-list-ol';

            blade.toolbarCommands = [
                {
                    name: "platform.commands.add", icon: 'fa fa-plus',
                    executeMethod: function () { blade.selectNode({}, true); },
                    canExecuteMethod: function () { return true; },
                    permission: 'content:create'
                },
                {
                    name: "platform.commands.delete", icon: 'fa fa-trash-o',
                    executeMethod: function () { blade.deleteMenu($scope.gridApi.selection.getSelectedRows()); },
                    canExecuteMethod: function () {
                        return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
                    },
                    permission: 'content:delete'
                },
            ];

            // ui-grid
            $scope.setGridOptions = function (gridOptions) {
                uiGridHelper.initialize($scope, gridOptions);
            };

            blade.refresh();
        }
    ]);
