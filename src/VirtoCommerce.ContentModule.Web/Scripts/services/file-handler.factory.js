angular.module('virtoCommerce.contentModule')
    .provider('virtoCommerce.contentModule.fileHandlerFactory',
        function () {
            var _handlers = [];

            function getService($injector) {

                var service = {
                    handleAction: handleAction
                };

                function handleAction(operation, context) {
                    // context = {
                    //    blade - current blade
                    //    store - current store
                    //    file - current file
                    // }

                    var handlers = _handlers.map(function (handlerName) {
                        return $injector.get(handlerName);
                    }).filter(function (handler) {
                        var op = handler[operation];
                        return op && op.isMatch && op.isMatch(context.file);
                    }).map(function (handler) {
                        return handler[operation];
                    });

                    if (handlers) {

                        var blade = context.blade;
                        var store = context.store;
                        var newBlade = {
                            contentType: blade.contentType || 'pages',
                            storeId: blade.storeId,
                            storeUrl: blade.storeUrl || store.url,
                            languages: blade.languages || store.languages,
                            folderUrl: (blade.currentEntity && blade.currentEntity.relativeUrl) || '/',
                            currentEntity: context.file
                        };

                        if (handlers.length > 1) {
                            angular.extend(newBlade, {
                                fileTypes: handlers,
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

            this.$get = ['$injector', function ($injector) {
                return getService($injector);
            }];
        }
    );
