angular.module('virtoCommerce.contentModule')
    .controller('virtoCommerce.contentModule.contentMainController', [
        '$scope', '$state', '$stateParams', 'virtoCommerce.contentModule.menus',
        'virtoCommerce.contentModule.contentApi', 'virtoCommerce.storeModule.stores',
        'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService',
        'platformWebApp.widgetService', 'platformWebApp.bladeUtils',
        'virtoCommerce.contentModule.fileHandlerFactory',
        function ($scope, $state, $stateParams, menus, contentApi, stores,
            bladeNavigationService, dialogService, widgetService, bladeUtils, fileHandlerFactory) {
	    var blade = $scope.blade;

        var filter = $scope.filter = {};

        filter.criteriaChanged = function () {
            if ($scope.pageSettings.currentPage > 1) {
                $scope.pageSettings.currentPage = 1;
            } else {
                blade.refresh();
            }
        };

        blade.refresh = function () {
	        blade.isLoading = true;
            blade.currentEntities = [];

	        if ($stateParams.storeId) {
	            stores.get({ id: $stateParams.storeId }, blade.openThemes);
	        };

            stores.search({
                keyword: filter.keyword ? filter.keyword : undefined,
                skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
                take: $scope.pageSettings.itemsPerPageCount
            }, function (data) {

                var loadCounter = data.results.length * 2;
                $scope.pageSettings.totalItems = data.totalCount;
	            var finnalyFunction = function () {
	                blade.isLoading = --loadCounter;
	            };

                blade.isLoading = _.any(data.results);

                $scope.pageSettings.totalItems = data.totalCount;

                _.each(data.results, function (x) {
	                blade.currentEntities.push({
	                    storeId: x.id,
	                    store: x,
	                    themesCount: '...',
	                    pagesCount: '...',
	                    blogsCount: '...',
	                    listLinksCount: '...'
                    });

                    var statsPromise = blade.refreshWidgets(x.id, 'stats')
                    if (statsPromise) {
                        statsPromise.finally(finnalyFunction);
                    }

                    var menusPromise = blade.refreshWidgets(x.id, 'menus');
                    if (menusPromise) {
                        menusPromise.finally(finnalyFunction);
                    }
	            });
	        });

	        $scope.thereIsWidgetToShow = _.any(widgetService.widgetsMap['contentMainListItem'], function (w) { return !angular.isFunction(w.isVisible) || w.isVisible(blade); });
	    };


	    blade.refreshWidgets = function (storeId, requestType, data) {
	        var entity = _.findWhere(blade.currentEntities, { storeId: storeId });

	        switch (requestType) {
	            case 'menus':
	                return menus.get({ storeId: storeId }, function (data) {
	                    entity.listLinksCount = data.length;
	                }, function (error) { bladeNavigationService.setError('Error ' + error.status, blade); }).$promise;
	            case 'stats':
	                return contentApi.getStatistics({ storeId: storeId }, function (data) {
	                    angular.extend(entity, data);
	                }, function (error) { bladeNavigationService.setError('Error ' + error.status, blade); }).$promise;
                case 'defaultTheme':
                    entity.activeThemeName = data;
                    return null;
	            default:
                    return null;
	        }
	    };

	    $scope.$on("cms-menus-changed", function (event, storeId) {
	        blade.refresh(storeId, 'menus');
	    });

	    $scope.$on("cms-statistics-changed", function (event, storeId) {
	        blade.refresh(storeId, 'stats');
	    });

	    blade.openThemes = function (store) {
	        var newBlade = {
	            id: "themesListBlade",
	            storeId: store.id,
	            title: 'content.blades.themes-list.title',
	            titleValues: { name: store.name },
	            subtitle: 'content.blades.themes-list.subtitle',
	            controller: 'virtoCommerce.contentModule.themesListController',
	            template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/themes/themes-list.tpl.html',
	        };
	        bladeNavigationService.showBlade(newBlade, blade);
	    };

	    blade.openPages = function (data) {
	        var newBlade = {
	            id: "pagesList",
	            contentType: 'pages',
	            storeId: data.storeId,
	            storeUrl: data.store.url,
	            languages: data.store.languages,
	            currentEntity: data,
	            title: data.store.name,
	            subtitle: 'content.blades.pages-list.subtitle-pages',
                controller: 'virtoCommerce.contentModule.pagesListController',
                template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/pages/pages-list.tpl.html'
            };
	        bladeNavigationService.showBlade(newBlade, blade);
	    };

	    blade.openLinkLists = function (data) {
	        var newBlade = {
	            id: "linkListBlade",
	            storeId: data.storeId,
	            title: 'content.blades.link-lists.title',
	            subtitle: 'content.blades.link-lists.subtitle',
	            subtitleValues: { name: data.store.name },
	            controller: 'virtoCommerce.contentModule.linkListsController',
	            template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/menu/link-lists.tpl.html',
	        };
	        bladeNavigationService.showBlade(newBlade, blade);
	    };

	    blade.openBlogs = function (data) {
	        var newBlade = {
	            id: "blogsListBlade",
	            contentType: 'blogs',
	            storeId: data.storeId,
	            storeUrl: data.store.url,
	            languages: data.store.languages,
	            currentEntity: data,
	            title: data.store.name,
	            subtitle: 'content.blades.pages-list.subtitle-blogs',
	            controller: 'virtoCommerce.contentModule.pagesListController',
                template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/pages/pages-list.tpl.html'
	        };
	        bladeNavigationService.showBlade(newBlade, blade);
	    };

	    blade.addNewTheme = function (data) {
	        var newBlade = {
	            id: 'addTheme',
	            isNew: true,
	            isActivateAfterSave: !data.themesCount,
	            store: data.store,
	            storeId: data.storeId,
	            controller: 'virtoCommerce.contentModule.themeDetailController',
	            template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/themes/theme-detail.tpl.html',
	        };
	        bladeNavigationService.showBlade(newBlade, blade);
	    };

        blade.addNewPage = function (data) {
            fileHandlerFactory.handleAction('create', { blade: blade, store: data });
	    };

	    blade.addNewLinkList = function (data) {
	        var newBlade = {
	            id: 'addMenuLinkListBlade',
	            storeId: data.storeId,
	            isNew: true,
	            title: 'content.blades.menu-link-list.title-new',
	            subtitle: 'content.blades.menu-link-list.subtitle-new',
	            controller: 'virtoCommerce.contentModule.menuLinkListController',
	            template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/menu/menu-link-list.tpl.html',
	        };
	        bladeNavigationService.showBlade(newBlade, blade);
	    };

	    blade.addBlog = function (data) {
	        var newBlade = {
	            id: 'newBlog',
	            contentType: 'blogs',
	            storeId: data.storeId,
	            currentEntity: {},
	            isNew: true,
	            title: 'content.blades.edit-blog.title-new',
	            subtitle: 'content.blades.edit-blog.subtitle-new',
	            controller: 'virtoCommerce.contentModule.editBlogController',
	            template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/pages/edit-blog.tpl.html',
	        };
	        bladeNavigationService.showBlade(newBlade, blade);
	    };

	    blade.openTheme = function (data) {
	        var newBlade = {
	            id: 'themeAssetListBlade',
	            contentType: 'themes',
	            storeId: data.storeId,
	            currentEntity: { name: data.activeThemeName, url: data.activeThemeURL },
	            subtitle: 'content.blades.asset-list.subtitle',
	            controller: 'virtoCommerce.contentModule.assetListController',
	            template: 'Modules/$(VirtoCommerce.Assets)/Scripts/blades/asset-list.tpl.html'
	        };
	        bladeNavigationService.showBlade(newBlade, blade);
	    };

	    blade.previewTheme = function (data) {
	        if (data.store.url) {
	            window.open(data.store.url + '?previewtheme=' + data.activeThemeName, '_blank');
	        }
	        else {
	            var dialog = {
	                id: "noUrlInStore",
	                title: "content.dialogs.set-store-url.title",
	                message: "content.dialogs.set-store-url.message"
	            }
	            dialogService.showNotificationDialog(dialog);
	        }
        }

        blade.openNewMenus = function (data) {
            var newBlade = {
                id: 'newMenuListBlade',
                title: 'content.blades.menu-list.title',
                storeId: data.storeId,
                currentEntity: { name: data.activeThemeName, url: data.activeThemeURL },
                controller: 'virtoCommerce.contentModule.newMenuListController',
                template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/new-menu/list.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);
        }

	    $scope.openStoresModule = function () {
	        $state.go('workspace.storeModule');
	    };

	    blade.headIcon = 'fa fa-code';

        bladeUtils.initializePagination($scope);
        $scope.pageSettings.itemsPerPageCount = 3;


	}]);
