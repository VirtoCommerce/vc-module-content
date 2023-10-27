var moduleName = "virtoCommerce.contentModule";

angular.module(moduleName)
    .config(
        ['virtoCommerce.contentModule.fileProcessorsProvider', function (fileProcessorsProvider) {
            fileProcessorsProvider.addHandler(markdownFileHandler());
        }]
    );

function markdownFileHandler() {
    var handler = {
        descriptor: {
            icon: 'list-ico fa fa-file-code-o',
            name: 'content.blades.add-page.menu.html-page.title',
            description: 'content.blades.add-page.menu.html-page.description'
        },
        isMatch: isMatch,
        edit: {
            execute: editFile
        },
        create: {
            execute: createFile
        }
    };

    function isMatch(file, operation) {
        return true;
    }

    function createFile() { }

    function editFile() { }

    return handler;
}
