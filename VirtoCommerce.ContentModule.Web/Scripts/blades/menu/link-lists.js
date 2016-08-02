angular.module('virtoCommerce.contentModule')
.controller('virtoCommerce.contentModule.linkListsController', ['$rootScope', '$scope', 'virtoCommerce.contentModule.menus', 'virtoCommerce.storeModule.stores', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'uiGridConstants', 'platformWebApp.uiGridHelper',
    function ($rootScope, $scope, menus, menusStores, bladeNavigationService, dialogService, uiGridConstants, uiGridHelper) {
        $scope.uiGridConstants = uiGridConstants;
        $scope.selectedNodeId = null;
        var blade = $scope.blade;

        blade.initialize = function () {
            blade.isLoading = true;
            menus.get({ storeId: blade.storeId }, function (data) {
                blade.currentEntities = data;
                blade.isLoading = false;
                blade.parentBlade.refresh(blade.storeId, 'menus');
            },
            function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
        }

        blade.selectNode = function (data) {
            $scope.selectedNodeId = data.id;

            var newBlade = {
                id: 'editMenuLinkListBlade',
                chosenStoreId: blade.storeId,
                chosenListId: data.id,
                newList: false,
                title: 'content.blades.menu-link-list.title',
                titleValues: { name: data.name },
                subtitle: 'content.blades.menu-link-list.subtitle',
                controller: 'virtoCommerce.contentModule.menuLinkListController',
                template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/menu/menu-link-list.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);
        }

        function openBladeNew() {
            $scope.selectedNodeId = null;

            var newBlade = {
                id: 'addMenuLinkListBlade',
                chosenStoreId: blade.storeId,
                newList: true,
                title: 'content.blades.menu-link-list.title-new',
                subtitle: 'content.blades.menu-link-list.subtitle-new',
                controller: 'virtoCommerce.contentModule.menuLinkListController',
                template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/menu/menu-link-list.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);
        }

        blade.deleteList = function (node) {
            var dialog = {
                id: "confirmDelete",
                title: "content.dialogs.link-list-delete.title",
                message: "content.dialogs.link-list-delete.message",
                messageValues: { name: node.name },
                callback: function (remove) {
                    if (remove) {
                        blade.isLoading = true;

                        menus.delete({ storeId: blade.storeId, listId: node.id }, function () {
                            blade.initialize();
                            $rootScope.$broadcast("cms-menus-changed", blade.storeId);
                        },
                        function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
                    }
                }
            };
            dialogService.showConfirmationDialog(dialog);
        };

        blade.headIcon = 'fa-archive';

        blade.toolbarCommands = [
            {
                name: "platform.commands.add", icon: 'fa fa-plus',
                executeMethod: function () {
                    openBladeNew();
                },
                canExecuteMethod: function () {
                    return true;
                },
                permission: 'content:create'
            },
			{
			    name: "platform.commands.delete", icon: 'fa fa-trash-o',
			    executeMethod: function () {
			        blade.deleteList(_.findWhere(blade.currentEntities, { id: $scope.selectedNodeId }));
			    },
			    canExecuteMethod: function () {
			        return $scope.selectedNodeId;
			    },
			    permission: 'content:delete'
			}
        ];

        // ui-grid
        $scope.setGridOptions = function (gridOptions) {
            uiGridHelper.initialize($scope, gridOptions);
        };

        blade.initialize();
    }]);
