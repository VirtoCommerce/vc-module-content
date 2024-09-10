angular.module('virtoCommerce.contentModule')
    .factory('virtoCommerce.contentModule.broadcastChannelFactory', ['virtoCommerce.contentModule.files-draft',
        function (filesDraftService) {
            return function (blade) {
                var currentBlade = blade;
                var channel = new BroadcastChannel('vc-module-content-channel');
                var service = {
                    close: channel.close,
                    postMessage: function (message) {
                        if (channel) {
                            var templateKey = filesDraftService.getTemplateKey(currentBlade);
                            message.templateKey = templateKey;
                            message.relativeUrl = currentBlade.currentEntity.relativeUrl;
                            message.contentType = currentBlade.contentType;
                            message.source = 'platform';

                            channel.postMessage(message)
                        }
                    },
                    onmessage: function (message) {
                        console.log(message);
                    }
                };

                var defaultClose = blade.onClose;

                currentBlade.onClose = function (callback) {

                    var closeHandler = function () {
                        if (channel) {
                            channel.close();
                            channel = null;
                        }
                        callback();
                    };

                    if (defaultClose) {
                        defaultClose(closeHandler);
                    } else {
                        closeHandler();
                    }
                }



                channel.onmessage = function (message) {
                    service.onmessage(message);
                }

                return service;
            };
        }])
