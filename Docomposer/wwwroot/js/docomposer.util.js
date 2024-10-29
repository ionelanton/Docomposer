docomposer.util = docomposer.util || {};
(function () {
    
    let _messages = []
    
    this.alert = function(msg, type = "info", timeout = 2000) {
        
        _messages.push({type: type, msg: msg});
        console.log(_messages);
        
        let div = document.createElement("div");
        $(div).attr({
            "class": "alert alert-" + type,
            "role": "alert",
            "style": "position: absolute; bottom: 0; width: 100%;"
        });
        $(div).append(msg);
        $(div).append("<button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\">\n" +
            "    <span aria-hidden=\"true\">&times;</span>\n" +
            "  </button>");
        $(div).appendTo($("body"));

        if (type === "danger") {
            timeout = timeout * 3;
        }
        
        $(div).delay(timeout).slideUp('slow', function(){
            $(div).remove();
        }).on("mouseover", function() {
            $(div).stop(true, false);
        });

        return true;
    }
    
    this.alertSuccess = function(msg) {
        this.alert(msg, "success");
        return true;
    }
    
    this.alertError = function(msg, err) {
        //info: https://stackoverflow.com/questions/38630076/asp-net-core-web-api-exception-handling
        let errMsg = msg;
        
        if (has(err, "response", "data", "detail")) {
            errMsg += " : " + err.response.data.detail;
        } else if (has(err, "response", "data")) {
            errMsg += " : " + JSON.stringify(err.response.data);
        }
        this.alert(errMsg, "danger", );
        return true;
    }
    
    this.setPdfContainerHeight = function(selector, ratio) {
        let height = $(window).height();
        $(selector).height(height*ratio - 150);
        return true;
    }

    this.clientInstance = function () {
        return $("#DocomposerClientInstance").val();
    }
    
    this.addClass = function(selector, cls) {
        $(selector).addClass(cls);
        return true;
    }

    this.removeClass = function(selector, cls) {
        $(selector).removeClass(cls);
        return true;
    }
    
    //todo: Word does not show protected view for localhost downloaded docx files
    // https://support.microsoft.com/en-us/topic/files-on-local-hosted-servers-are-opened-in-protected-view-in-excel-2010-in-powerpoint-2010-or-in-word-2010-e9a83464-fa27-8929-e7d3-a67d879b85f1
    this.downloadFile = function(filename, bytesBase64) {
        let link = document.createElement('a');
        link.download = filename;
        link.href = "data:application/octet-stream;base64," + bytesBase64;
        document.body.appendChild(link); // Needed for Firefox
        link.click();
        document.body.removeChild(link);
        return true;
    }
    
    function has(obj /*, level1, level2, ... levelN*/) {
        let args = Array.prototype.slice.call(arguments, 1);

        for (let i = 0; i < args.length; i++) {
            if (!obj || !obj.hasOwnProperty(args[i])) {
                return false;
            }
            obj = obj[args[i]];
        }
        return true;
    }

}).apply(docomposer.util);