angular.module('virtoCommerce.contentModule')
    .service('virtoCommerce.contentModule.files-draft', [function () {

        var $this = this;

        this.getDocumentId = function (blade, real) {
            if (!blade || !blade.currentEntity) {
                return null;
            }
            const relativeUrl = real ? $this.getRealFileName(blade) : $this.undraftUrl(blade.currentEntity.relativeUrl);
            return btoa(`${blade.storeId}::${blade.contentType}::${relativeUrl}`).replaceAll('=', '-');
        }

        this.getRealFileName = function (blade) {
            var relativeUrl = blade.currentEntity.relativeUrl;
            var isDraft = blade.currentEntity.hasChanges;
            if (!isDraft) {
                return $this.undraftUrl(relativeUrl);
            }
            if (!relativeUrl.endsWith('-draft')) {
                return $this.getDraftFileName(blade);
            }
            return relativeUrl;
        }

        this.getDraftFileName = function (blade) {
            var relativeUrl = blade.currentEntity.relativeUrl;
            // the draft page should be under editing in the designer

            if (!relativeUrl.endsWith('-draft')) {
                relativeUrl = relativeUrl + '-draft';
            }

            return relativeUrl;
        };

        this.undraftUrl = function(url) {
            if (!!url && url.endsWith('-draft')) {
                return url.substring(0, url.length - 6);
            }
            return url;
        }


        this.getTemplateKey = function (blade) {
            return `${blade.contentType}::${$this.getDraftFileName(blade)}`;
        };
    }])
