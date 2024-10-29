docomposer.util.ace = docomposer.util.ace || {};
(function () {
    this.editors = [];

    this.init = function(editorId, mode="sql") {
        
        let instance = this.getEditorById(editorId);

        if (instance === null) {
            instance = ace.edit(editorId);
            this.editors.push(instance);
        } else {
            instance = ace.edit(editorId);
        }

        instance["editorId"] = editorId;
        let SqlMode = ace.require("ace/mode/" + mode).Mode;
        instance.session.setMode(new SqlMode());

        instance.getSession().on('change', function(){
            let textarea = document.getElementById("textarea-" + editorId);
            textarea.value = instance.getSession().getValue();
            let event = new Event('change');
            textarea.dispatchEvent(event);
        });
        
        return instance;
        //console.log(JSON.stringify(JSON.decycle(_editor)));
    }
    
    this.setValue = function(editorId, value, mode = "sql") {
        let editor = this.init(editorId, mode)
        editor.setValue(value, -1);
    }
    
    this.getValue = function(editorId, mode = "sql") {
        let editor = this.init(editorId, mode)
        editor.getValue();
    }
    
    this.getEditorById = function (editorId) {
        for(let i= 0; i < this.editors.length; i++) {
            if(this.editors[i]["editorId"] === editorId) {
                return this.editors[i];
            }
        }
        return null;
    }
  
}).apply(docomposer.util.ace);