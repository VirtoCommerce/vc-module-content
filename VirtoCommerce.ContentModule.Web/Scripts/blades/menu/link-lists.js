angular.module('virtoCommerce.contentModule')
.controller('virtoCommerce.contentModule.linkListsController', ['$scope', 'virtoCommerce.contentModule.menus', 'virtoCommerce.storeModule.stores', 'platformWebApp.bladeNavigationService', function ($scope, menus, menusStores, bladeNavigationService) {
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

    blade.openBlade = function (data) {
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

    blade.headIcon = 'fa-archive';

    blade.toolbarCommands = [
        {
            name: "content.commands.add-list", icon: 'fa fa-plus',
            executeMethod: function () {
                openBladeNew();
            },
            canExecuteMethod: function () {
                return true;
            },
            permission: 'content:create'
        }
    ];

    blade.initialize();
}]);
