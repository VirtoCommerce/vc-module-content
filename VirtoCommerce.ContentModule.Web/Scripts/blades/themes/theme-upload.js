angular.module('virtoCommerce.contentModule')
.controller('virtoCommerce.contentModule.themeUploadController', ['$rootScope', '$scope', 'platformWebApp.dialogService', 'virtoCommerce.contentModule.contentApi', 'FileUploader', 'platformWebApp.bladeNavigationService', function ($rootScope, $scope, dialogService, contentApi, FileUploader, bladeNavigationService) {
    var blade = $scope.blade;

    // create the uploader
    var uploader = $scope.uploader = new FileUploader({
        scope: $scope,
        headers: { Accept: 'application/json' },
        url: 'api/content/themes/' + blade.storeId + '?folderUrl=',
        queueLimit: 1,
        autoUpload: false,
        removeAfterUpload: false
    });

    // ADDING FILTERS
    // Zips only
    uploader.filters.push({
        name: 'zipFilter',
        fn: function (i /*{File|FileLikeObject}*/, options) {
            return i.name.toLowerCase().endsWith('.zip');
        }
    });


    uploader.onWhenAddingFileFailed = function (item, filter, options) {
        if (filter.name === "queueLimit") {
            uploader.clearQueue();
            uploader.addToQueue(item);
        }
    };

    uploader.onAfterAddingFile = function (item) {
        $scope.themeName = item.file.name.substring(0, item.file.name.lastIndexOf('.'));

        if (!!blade.currentEntities && blade.currentEntities.length > 0
            && _.any(blade.currentEntities, function (item) { return item.name === $scope.themeName; })) {

            var dialog = {
                id: "confirmUploadTheme",
                title: "content.dialogs.theme-upload-confirmation.title",
                message: "content.dialogs.theme-upload-confirmation.message",
                callback: function (upload) {
                    if (upload) {
                        uploader.uploadAll();
                    }
                }
            };
            dialogService.showConfirmationDialog(dialog);
        } else {
            uploader.uploadAll();
        }
        
       
    };   

    uploader.onSuccessItem = function (fileItem, files) {
        contentApi.unpack({
            contentType: 'themes',
            storeId: blade.storeId,
            archivepath: files[0].name,
            destPath: $scope.themeName
        }, function (data) {
            if (blade.isActivateAfterSave) {
                var prop = _.findWhere(blade.store.dynamicProperties, { name: 'DefaultThemeName' });
                prop.values = [{ value: $scope.themeName }];

                blade.store.$update(refreshParentAndClose, function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
            } else {
                refreshParentAndClose();
            }
        },
        function (error) {
            uploader.clearQueue();
            bladeNavigationService.setError('Error ' + error.status, $scope.blade);
        });
    };

    function refreshParentAndClose() {
        $scope.bladeClose();
        blade.parentBlade.refresh();
        $rootScope.$broadcast("cms-statistics-changed", blade.storeId);
    }

    uploader.onErrorItem = function (item, response, status, headers) {
        bladeNavigationService.setError(item._file.name + ' failed: ' + (response.message ? response.message : status), blade);
    };

    blade.title = 'content.blades.theme-upload.title',
    blade.isLoading = false;
}]);
