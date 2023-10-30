var moduleName = "virtoCommerce.contentModule";

angular.module(moduleName)
    .factory('virtoCommerce.contentModule.markdownFileHandler', ['platformWebApp.bladeNavigationService', function (bladeNavigationService) {
        var handler = {
            edit: {
                descriptor: {
                    icon: 'list-ico fa fa-file-code-o',
                    name: 'content.blades.add-page.menu.html-page.title',
                    description: 'content.blades.add-page.menu.html-page.description'
                },
                isMatch: isMatchForEdit,
                execute: editFile
            },
            create: {
                descriptor: {
                    icon: 'list-ico fa fa-file-code-o',
                    name: 'content.blades.add-page.menu.html-page.title',
                    description: 'content.blades.add-page.menu.html-page.description'
                },
                isMatch: function () { return true; },
                execute: createFile
            }
        };

        function isMatchForEdit(file, operation) {
            return file && file.name && file.name.endsWith('.md');
        }

        function createFile(blade, parentBlade) {
            angular.extend(blade, {
                isNew: true,
                controller: 'virtoCommerce.contentModule.pageDetailController',
                template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/pages/page-detail.tpl.html',
            });

            if (blade.contentType === 'blogs') {
                angular.extend(blade, {
                    title: 'content.blades.edit-page.title-new-post',
                    subtitle: 'content.blades.edit-page.subtitle-new-post'
                });
            } else {
                angular.extend(blade, {
                    title: 'content.blades.edit-page.title-new',
                    subtitle: 'content.blades.edit-page.subtitle-new',
                });
            }

            bladeNavigationService.showBlade(blade, parentBlade);
        }

        function editFile(blade, parentBlade) {
            angular.extend(blade, {
                isNew: false,
                title: blade.currentEntity.name,
                controller: 'virtoCommerce.contentModule.pageDetailController',
                template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/pages/page-detail.tpl.html',
            });

            if (blade.contentType === 'blogs') {
                angular.extend(blade, {
                    subtitle: 'content.blades.edit-page.subtitle-post'
                });
            } else {
                angular.extend(blade, {
                    subtitle: 'content.blades.edit-page.subtitle',
                });
            }

            bladeNavigationService.showBlade(blade, parentBlade);
        }

        return handler;

    }]);
