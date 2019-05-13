angular.module('virtoCommerce.contentModule')
    .controller('virtoCommerce.contentModule.pageAddController', ['$rootScope', '$scope', 'platformWebApp.bladeNavigationService', function ($rootScope, $scope, bladeNavigationService) {
        $scope.addDesignPage = function () {
            var newBlade = {
                id: 'jsonPageDetail',
                contentType: $scope.blade.parentBlade.contentType,
                storeId: $scope.blade.parentBlade.storeId,
                currentEntity: {},
                isNew: true,
                title: 'content.blades.edit-page.title-new',
                subtitle: 'content.blades.edit-page.subtitle-new',
                controller: 'virtoCommerce.contentModule.editJsonPageController',
                template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/pages/edit-json-page.tpl.html'
            };

            bladeNavigationService.showBlade(newBlade, $scope.blade.parentBlade);
        };

        $scope.addHtmlPage = function () {
            var newBlade = {
                id: 'pageDetail',
                contentType: $scope.blade.parentBlade.contentType,
                storeId: $scope.blade.parentBlade.storeId,
                storeUrl: $scope.blade.parentBlade.storeUrl,
                languages: $scope.blade.parentBlade.languages,
                folderUrl: $scope.blade.parentBlade.currentEntity.url,
                currentEntity: {},
                isNew: true,
                title: 'content.blades.edit-page.title-new',
                subtitle: 'content.blades.edit-page.subtitle-new',
                controller: 'virtoCommerce.contentModule.pageDetailController',
                template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/pages/page-detail.tpl.html'
            };

            if (isBlogs()) {
                angular.extend(newBlade, {
                    title: 'content.blades.edit-page.title-new-post',
                    subtitle: 'content.blades.edit-page.subtitle-new-post'
                });
            };

            bladeNavigationService.showBlade(newBlade, $scope.blade.parentBlade);
        };

        function isBlogs() {
            return $scope.blade.parentBlade.contentType === 'blogs';
        }

        $scope.blade.isLoading = false;
    }]);
