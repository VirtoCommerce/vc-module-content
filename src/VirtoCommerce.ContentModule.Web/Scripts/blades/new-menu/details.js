angular.module('virtoCommerce.contentModule')
    .controller('virtoCommerce.contentModule.newMenuDetailsController', [
        '$scope',

        'platformWebApp.bladeNavigationService',
        'virtoCommerce.contentModule.menuLinkList-associationTypesService', 'virtoCommerce.storeModule.stores', 'virtoCommerce.contentModule.newMenus',
        function ($scope, bladeNavigationService, associationTypesService, stores, menus) {
            const blade = $scope.blade;
            blade.associatedObjectTypes = associationTypesService.objects;

            blade.addItem = function (type) {
                blade.currentEntity.items.push({
                    type: type,
                    name: type === 'link' ? 'New Link' : 'New Block',
                    items: type === 'link' ? null : [],
                    url: type === 'link' ? '' : null,
                    associatedObjectType: null,
                    associatedObjectName: null,
                    associatedObjectId: null,
                    isCollapsed: true,
                    priority: blade.currentEntity.items.length + 1,
                    menuId: blade.currentEntity.id
                });
            }

            $scope.saveChanges = function () {
                if (blade.isNew) {
                    menus.create({ storeId: blade.storeId }, blade.currentEntity, function (data) {
                        blade.currentEntity = data;
                        blade.parentBlade.refresh();
                        $scope.bladeClose();
                    });
                } else {
                    menus.update({ storeId: blade.storeId }, blade.currentEntity, function (data) {
                        blade.currentEntity = data;
                        blade.originalEntity = angular.copy(blade.currentEntity);
                    });
                }
            }

            $scope.setForm = function (form) {
                $scope.formScope = blade.formScope = form;
            }

            blade.reset = function () {
                blade.currentEntity = angular.copy(blade.originalEntity);
            }

            function canSave() {
                return isDirty() && $scope.formScope && $scope.formScope.$valid;
            }

            function isDirty() {
                return !angular.equals(blade.currentEntity, blade.originalEntity);
            }

            blade.onClose = function (closeCallback) {
                bladeNavigationService.showConfirmationIfNeeded(
                    isDirty() && !blade.isNew,
                    canSave(),
                    blade,
                    $scope.saveChanges,
                    closeCallback,
                    "content.dialogs.menu-details-save.title",
                    "content.dialogs.menu-details-save.message");
            };

            blade.initialize = function () {
                if (blade.isNew) {
                    blade.currentEntity = { storeId: blade.storeId };
                    blade.currentEntity.items = [];
                    blade.originalEntity = angular.copy(blade.currentEntity);
                }
                else {
                    menus.getMenu({ menuId: blade.currentEntityId, storeId: blade.storeId },
                        function (data) {
                            blade.currentEntity = data;

                            if (!blade.currentEntity.items) {
                                blade.currentEntity.items = [];
                            }

                            blade.originalEntity = angular.copy(blade.currentEntity);
                        },
                        function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
                }

                stores.get({ id: blade.storeId },
                    function (data) {
                        blade.languages = data.languages;
                        blade.isLoading = false;
                    },
                    function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });

                blade.toolbarCommands = [
                    {
                        name: 'content.commands.add-block', icon: 'fa fa-plus',
                        executeMethod: function () { blade.addItem('block'); },
                        canExecuteMethod: function () { return true; },
                    },
                    {
                        name: 'content.commands.add-link', icon: 'fa fa-plus',
                        executeMethod: function () { blade.addItem('link'); },
                        canExecuteMethod: function () { return true; },
                    },
                    {
                        name: 'platform.commands.save', icon: 'fa fa-save',
                        executeMethod: $scope.saveChanges,
                        canExecuteMethod: canSave,
                    },
                ];

                if (!blade.isNew) {
                    blade.toolbarCommands.push({
                        name: 'platform.commands.reset', icon: 'fa fa-undo',
                        executeMethod: blade.reset,
                        canExecuteMethod: function () { return !angular.equals(blade.currentEntity, blade.originalEntity); },
                    });
                }
            }

            blade.initialize();
        }
    ])
    .directive('blockLinkManager', function () {
        return {
            restrict: 'A',
            scope: {
                items: '=blockLinkManager',
                associatedObjectTypes: '=?',
                parent: '=?',
                depth: '=?'
            },
            templateUrl: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/new-menu/block-link-template.html',
            controller: ['$scope', function ($scope) {
                $scope.depth = $scope.depth || 0;
                $scope.maxDepth = 5;

                // Sortable options for drag and drop
                $scope.sortableOptions = {
                    connectWith: '.block-container',
                    handle: '.drag-handle',
                    placeholder: 'sortable-placeholder',
                    tolerance: 'pointer',
                    start: function (e, ui) {
                        ui.item.addClass('dragging');
                    },
                    stop: function (e, ui) {
                        ui.item.removeClass('dragging');
                        updateSortOrder();
                        $scope.$apply();
                    },
                    receive: function(e, ui) {
                        // When an item is received from another level
                        updateSortOrder();
                        $scope.$apply();
                    }
                };

                function updateSortOrder() {
                    // Update priorities for the current level in descending order
                    $scope.items.forEach((item, index) => {
                        item.priority = $scope.items.length - index;
                        item.id = null;
                    });

                    // Recursively update priorities for nested items
                    function updateNestedPriorities(items) {
                        if (!items) return;
                        items.forEach((item, index) => {
                            item.priority = items.length - index;
                            item.id = null;
                            if (item.type === 'block' && item.items) {
                                updateNestedPriorities(item.items);
                            }
                        });
                    }

                    // Start with the current level's items
                    updateNestedPriorities($scope.items);
                }

                $scope.toggleAccordion = function (item) {
                    item.isCollapsed = !item.isCollapsed;
                };

                $scope.canAddItems = function () {
                    return $scope.depth < $scope.maxDepth;
                };

                $scope.addItem = function (parent, type) {
                    if (!$scope.canAddItems()) {
                        console.log('Cannot add item at depth:', $scope.depth);
                        return;
                    }
                    if (parent) {
                        if (!parent.items) {
                            parent.items = [];
                        }
                        const newPriority = parent.items.length > 0 ? 
                            Math.max(...parent.items.map(item => item.priority)) + 1 : 1;
                        
                        parent.items.push({
                            type: type,
                            name: type === 'link' ? 'New Link' : 'New Block',
                            items: type === 'link' ? null : [],
                            url: type === 'link' ? '' : null,
                            associatedObjectType: null,
                            associatedObjectName: null,
                            associatedObjectId: null,
                            isCollapsed: true,
                            priority: newPriority,
                        });
                    }
                };

                $scope.removeItem = function (index) {
                    $scope.items.splice(index, 1);
                };
            }]
        };
    }); 
