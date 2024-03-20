angular.module('virtoCommerce.contentModule')
    .provider('virtoCommerce.contentModule.fileHandlerFactory',
        function () {
            var _handlers = [];

            function getService($injector, dialogService) {
                var service = {
                    getHandlers: getHandlers,
                    handleAction: handleAction
                };

                function getHandlers(operation, context) {
                    return _handlers.map(function (handlerName) {
                        return $injector.get(handlerName);
                    }).filter(function (handler) {
                        var op = handler[operation];
                        return op && op.isMatch && op.isMatch(context.file);
                    }).map(function (handler) {
                        return handler[operation];
                    });
                }

                function handleAction(operation, context) {
                    // context = {
                    //    blade - current blade
                    //    store - current store
                    //    file - current file
                    // }

                    var handlers = getHandlers(operation, context);

                    if (handlers) {

                        var blade = context.blade;
                        var store = context.store;

                        let publicStoreUrl = store && store.url ? store.url : (blade && blade.storeUrl ? blade.storeUrl : '');

                        if (publicStoreUrl === null || publicStoreUrl === '') {
                            var dialog = {
                                id: "noUrlDialog",
                                title: 'content.dialogs.store-no-url.title',
                                message: 'content.dialogs.store-no-url.message'
                            };
                            dialogService.showErrorDialog(dialog);
                            return;
                        };

                        var newBlade = {
                            contentType: blade.contentType || 'pages',
                            storeId: blade.storeId,
                            storeUrl: publicStoreUrl,
                            languages: blade.languages || store.languages,
                            folderUrl: (blade.currentEntity && blade.currentEntity.relativeUrl) || '/',
                            currentEntity: context.file
                        };

                        if (handlers.length > 1) {
                            angular.extend(newBlade, {
                                title: `content.blades.choose-action.${operation}.title`,
                                subtitle: `content.blades.choose-action.${operation}.subtitle`,
                                handlers: handlers,
                                controller: 'virtoCommerce.contentModule.chooseActionController',
                                template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/pages/choose-action.tpl.html'
                            });
                            var bladeNavigationService = $injector.get('platformWebApp.bladeNavigationService');
                            bladeNavigationService.showBlade(newBlade, blade);
                        } else {
                            handlers[0].execute(newBlade, blade);
                        }
                    }
                }

                return service;
            }

            this.addHandler = function (handler) {
                _handlers.push(handler);
            }

            this.$get = ['$injector', 'platformWebApp.dialogService', function ($injector, dialogService) {
                return getService($injector, dialogService);
            }];
        }
    );
