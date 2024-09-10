var moduleName = "virtoCommerce.contentModule";

angular.module(moduleName)
    .factory('virtoCommerce.contentModule.markdownFileHandler', ['platformWebApp.bladeNavigationService', function (bladeNavigationService) {
        var handler = {
            edit: {
                descriptor: {
                    icon: 'list-ico fa fa-file-code-o',
                    name: 'content.blades.markdownEditor.edit.title',
                    description: 'content.blades.markdownEditor.edit.description'
                },
                isMatch: isMatchForEdit,
                execute: editFile
            },
            create: {
                descriptor: {
                    icon: 'list-ico fa fa-file-code-o',
                    name: 'content.blades.markdownEditor.create.title',
                    description: 'content.blades.markdownEditor.create.description'
                },
                isMatch: function () { return true; },
                execute: createFile
            }
        };

        function isMatchForEdit(file, operation) {
            return file &&
                (
                (file.name && (file.name.endsWith('.md') || file.name.endsWith('.md-draft')))
                ||
                (file.mimeType && (file.mimeType.startsWith('application/j') || file.mimeType.startsWith('text/')))
                );
        }

        function createFile(blade, parentBlade) {
            var newBlade = {
                isNew: true,
                contentType: blade.contentType,
                storeId: blade.storeId,
                storeUrl: blade.storeUrl,
                languages: blade.languages,
                folderUrl: blade.folderUrl,
                currentEntity: {}
            };
            angular.extend(newBlade, {
                controller: 'virtoCommerce.contentModule.pageDetailController',
                template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/pages/page-detail.tpl.html',
            });

            if (blade.contentType === 'blogs') {
                angular.extend(newBlade, {
                    title: 'content.blades.edit-page.title-new-post',
                    subtitle: 'content.blades.edit-page.subtitle-new-post'
                });
            } else {
                angular.extend(newBlade, {
                    title: 'content.blades.edit-page.title-new',
                    subtitle: 'content.blades.edit-page.subtitle-new',
                });
            }

            bladeNavigationService.showBlade(newBlade, parentBlade);
        }

        function editFile(blade, parentBlade) {
            var newBlade = {
                isNew: false,
                contentType: blade.contentType,
                storeId: blade.storeId,
                storeUrl: blade.storeUrl,
                languages: blade.languages,
                folderUrl: blade.folderUrl,
                currentEntity: angular.copy(blade.currentEntity)
            };
            angular.extend(newBlade, {
                isNew: false,
                title: blade.currentEntity.name,
                controller: 'virtoCommerce.contentModule.pageDetailController',
                template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/pages/page-detail.tpl.html',
            });

            if (blade.contentType === 'blogs') {
                angular.extend(newBlade, {
                    subtitle: 'content.blades.edit-page.subtitle-post'
                });
            } else {
                angular.extend(newBlade, {
                    subtitle: 'content.blades.edit-page.subtitle',
                });
            }

            bladeNavigationService.showBlade(newBlade, parentBlade);
        }

        return handler;

    }]);
