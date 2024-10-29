docomposer.api.datasource = docomposer.api.datasource || {};
(function () {
    let _selector;
    let that = this;

    this.init = function(selector) {
        _selector = $(selector);
    }  

    this.get = function (id) {
        docomposer.api.client.put("/Util/", {
            id: id,
            type: "DataSources",
            clientInstance: docomposer.util.clientInstance()
        }).then(function() {
            that.show();
        }).catch(function(err) {
            console.log(err);
            docomposer.util.alertError("Connection error: ", err);
        });
    }

    this.show = function() {
        _selector.show();
    }

    this.hide = function() {
        _selector.hide();
    }
}).apply(docomposer.api.datasource);
