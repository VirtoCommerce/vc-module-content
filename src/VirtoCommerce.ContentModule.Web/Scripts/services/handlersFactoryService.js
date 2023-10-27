angular.module('virtoCommerce.contentModule')
    .provider('virtoCommerce.contentModule.fileProcessors', function () {

        var service = {
            getHandler: getHandler,

        };

        var handlers = [];

        function getHandler(operation, file) {
            var result = handlers.filter(function (handler) {
                var op = handler[operation];
                return (op && op.isMatch && op.isMatch(file)) ||
                    (handler.isMatch && handler.isMatch(file, operation));
            }).map(function (handler) {
                return handler[operation] || handler;
            });

            return result;
        }

        this.addHandler = function (handler) {
            handlers.push(handler);
        }

        this.$get = function () {
            return service;
        };
    });
