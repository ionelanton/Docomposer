// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

var docomposer = docomposer || {};
(function () {

    let _apiToken = "";
    let _filesPath = "";
   
    this.filesPath = function() {
        if (_filesPath === "") {
            throw "Error: files path required!";
        }
        return _filesPath;
    }
    
    this.apiToken = function() {
        if (_apiToken === "") {
            throw "Error: API token required!";
        }
        return _apiToken;
    }
    
    this.configure = function(config = null) {
        if (config == null) {
            throw "Error: application is not configured!";
        }
        _filesPath = config.filesPath;
        _apiToken = config.apiToken;
    }
    
}).apply(docomposer);
