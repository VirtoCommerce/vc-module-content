angular.module('virtoCommerce.contentModule')
    .factory('virtoCommerce.contentModule.menus',
        [
            '$resource', function($resource) {
                return $resource('api/cms/:storeId/menu/',
                    {},
                    {
                        get: { url: 'api/cms/:storeId/menu/', method: 'GET', isArray: true },
                        getList: { url: 'api/cms/:storeId/menu/:listId', method: 'GET' },
                        checkList: { url: 'api/cms/:storeId/menu/checkname', method: 'GET' },
                        update: { url: 'api/cms/:storeId/menu/', method: 'POST' },
                        delete: { url: 'api/cms/:storeId/menu/', method: 'DELETE' }
                    });
            }
        ])
    .factory('virtoCommerce.contentModule.themes',
        [
            '$resource', function($resource) {
                return $resource(null,
                    null,
                    {
                        cloneTheme: { url: 'api/content/themes/:storeId/cloneTheme', method: 'POST' }
                    });
            }
        ])
    .factory('virtoCommerce.contentModule.contentApi',
        [
            '$resource', 'moment', function($resource, moment) {
                return $resource('api/content/:contentType/:storeId',
                    null,
                    {
                        indexedSearchEnabled: { method: 'GET', url: '/api/content/search/enabled' },
                        getStatistics: { url: 'api/content/:storeId/stats' },
                        query: { url: 'api/content/:contentType/:storeId/search', isArray: true },
                        search: {
                            url: 'api/content/search',
                            method: 'POST',
                            cancellable: true,
                            transformRequest: function (request) {
                                var query = request.keyword;
                                if (request.contentType) {
                                    query += " contentType:" + request.contentType;
                                }
                                if (request.folderUrl && request.folderUrl !== '/') {
                                    request.folderUrl = request.folderUrl.replace(/\/$/, '');
                                }
                                return JSON.stringify(angular.extend({}, request, { searchPhrase: query }));
                            },
                            isArray: true,
                            transformResponse: function (rawData) { return JSON.parse(rawData).result; }
                        },
                        publish: {
                            url: 'api/content/:contentType/:storeId/publishing',
                            method: 'POST',
                            params: {
                                contentType: '@contentType',
                                storeId: '@storeId',
                                relativeUrl: '@relativeUrl',
                                publish: true
                            }
                        },
                        unpublish: {
                            url: 'api/content/:contentType/:storeId/publishing',
                            method: 'POST',
                            params: {
                                contentType: '@contentType',
                                storeId: '@storeId',
                                relativeUrl: '@relativeUrl',
                                publish: false
                            }
                        },
                        post: {
                            params: { draft: true },
                        },
                        get: {
                            params: { draft: true },
                            // using transformResponse to:
                            // 1. avoid automatic response result string converting to array;
                            transformResponse: function(rawData) { return { data: rawData }; }
                        },
                        // post data as multipart form
                        saveMultipartContent: {
                            method: 'POST',
                            headers: { 'Content-Type': undefined },
                            transformRequest: function(currentEntity) {
                                var fd = new FormData();
                                fd.append(currentEntity.name, currentEntity.content);
                                return fd;
                            },
                            isArray: true
                        },
                        uploadFromUrl: {
                            method: 'POST', params:
                            {
                                contentType: '@contentType', storeId: '@storeId', folderUrl: '@folderUrl', url: '@url'
                            },
                            isArray: true
                        },
                        getWithMetadata: {
                            params: { draft: true },
                            // using transformResponse to:
                            // 1. avoid automatic response result string converting to array;
                            // 2. parse metadata as needed.
                            transformResponse: function(rawData) {
                                var retVal = {};
                                var parts = rawData.split('---');
                                if (parts.length > 2) { // parts[0] is left empty
                                    retVal.content = parts[2].trim();
                                    var parsedMetadata = [];
                                    var parsedYAML = YAML.parse(parts[1].trim());
                                    retVal.metadata = _.map(parsedYAML,
                                        function(val, key) {
                                            if (!_.isArray(val)) {
                                                val = [val];
                                            }
                                            return {
                                                name: key,
                                                values: _.map(val, function(v) { return { value: v }; })
                                            };
                                        });
                                } else {
                                    retVal.content = rawData;
                                }
                                return retVal;
                            }
                        },
                        // post data as multipart form
                        saveWithMetadata: {
                            method: 'POST',
                            params: { draft: true },
                            headers: { 'Content-Type': undefined },
                            transformRequest: function(currentEntity) {
                                var metadata = {};
                                var nonEmptyProperties = _.filter(currentEntity.dynamicProperties, function(x) { return _.any(x.values, function(val) { return val.value || x.valueType == 'Boolean'; }); });
                                _.each(nonEmptyProperties,
                                    function(x) {
                                        var values;
                                        var isArray = x.isArray;
                                        if (x.isMultilingual && !x.isDictionary) {
                                            isArray = true;
                                            var nonEmptyMultilinguals = _.filter(x.values, function(val) { return val.value; });
                                            values = _.map(nonEmptyMultilinguals, function(val) { return { locale: val.locale, value: val.value } });
                                        } else {
                                            values = _.pluck(x.values, 'value');
                                            if (x.valueType == 'DateTime') {
                                                values = _.map(values,
                                                    function(val) {
                                                        return moment(val)
                                                            .format()
                                                            .substring(0, 10);
                                                    });
                                            } else if (x.isDictionary) {
                                                values = _.map(values,
                                                    function(val) {
                                                        var retVal = { id: val.id, name: val.name };
                                                        if (x.isMultilingual) {
                                                            retVal.displayNames = _.map(val.displayNames,
                                                                function(displayName) {
                                                                    return { locale: displayName.locale, name: displayName.name }
                                                                });
                                                        }
                                                        return retVal;
                                                    });
                                            }
                                        }
                                        metadata[x.name] = isArray ? values : values[0];
                                    });
                                if (metadata.permalink && metadata.permalink.length > 0 && metadata.permalink[0] !== '/') {
                                    metadata.permalink = '/' + metadata.permalink;
                                }
                                var dataToSave = '---\n' + YAML.stringify(metadata) + '\n---\n' + (currentEntity.content || '').trim();

                                var blobName = currentEntity.name;
                                var blobNameExtension = '.md';
                                var idx = blobName.lastIndexOf('.');
                                if (idx >= 0) {
                                    blobNameExtension = blobName.substring(idx);
                                    blobName = blobName.substring(0, idx);
                                    idx = blobName.lastIndexOf('.'); // language
                                    if (idx >= 0) {
                                        blobName = blobName.substring(0, idx); // cut language from name
                                    }
                                }

                                if (currentEntity.language) {
                                    blobName += '.' + currentEntity.language;
                                }

                                var fd = new FormData();
                                fd.append(blobName + blobNameExtension, dataToSave);
                                return fd;
                            },
                            isArray: true
                        },
                        unpack: { url: 'api/content/:contentType/:storeId/unpack' },
                        createFolder: { url: 'api/content/:contentType/:storeId/folder', method: 'POST' },
                        copy: { url: 'api/content/copy' },
                        copyFile: {
                            url: 'api/content/:contentType/:storeId/copy-file',
                            method: 'POST',
                            params: {
                                contentType: '@contentType',
                                storeId: '@storeId',
                                srcFile: '@srcFile',
                                destFile: '@destFile'
                            }
                        },
                        move: { url: 'api/content/:contentType/:storeId/move' },
                        delete: { url: 'api/content/:contentType/:storeId', method: 'DELETE' }
                    });
            }
        ]);
