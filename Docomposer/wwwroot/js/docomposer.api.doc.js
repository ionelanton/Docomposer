docomposer.api.doc = docomposer.api.doc || {};
(function () {
    let _selector = {
        container: "",
        preview: ""
    };
    let _timer;

    this.init = function (selectorData) {
        _selector = selectorData;
        $(_selector.container).hide();
    }

    this.preview = function (type, id, refresh = false) {

        if (_timer) clearTimeout(_timer);

        _timer = setTimeout(function () {
            let container = $(_selector.preview);
            container.empty();

            container.append('<div class="progress" style="height: 100%"><div class="progress-bar progress-bar-striped progress-bar-animated" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%; background-color: dimgray !important;"><div style="transform: translateY(-50%);"></div></div></div>');

            let dialog = bootbox.dialog({
                message: '<div class="text-center"><i class="fas fa-spin fa-spinner"></i> Generating preview...</div>',
                centerVertical: true,
                closeButton: false
            }).on('shown.bs.modal', function () {
                docomposer.api.client.put("/Util/", {
                    id: id,
                    type: type,
                    clientInstance: docomposer.util.clientInstance()
                }).catch(function (err) {
                    dialog.modal('hide');
                    console.log(err);
                    docomposer.util.alertError("Connection error: ", err);
                });

                docomposer.api.client.get("/" + type + "/" + id,
                    {
                        params: {refresh: refresh}
                    }).then(function (r) {
                    container.empty();
                    dialog.modal('hide');

                    let base64 = "data:application/pdf;base64," + r.data;

                    PDFObject.embed(base64, container);

                }).catch(function (err) {
                    console.log(err);
                    docomposer.util.alertError("Error embedding PDF: ", err);
                    dialog.modal('hide');
                });
            });

            dialog.modal('hide');
        }, 250);

        $(_selector.container).show();
    }

    this.open = function (projectId, type, name) {
        if (_timer) clearTimeout(_timer);

        if (docomposer.filesPath().toLowerCase().startsWith("http")) {
            window.location.href = "ms-word:ofe|u|" + docomposer.filesPath() + "/" + projectId + "/" + type + "/" + name + ".docx";
        } else {
            docomposer.api.client.post("/Util/", {
                projectId: projectId,
                type: type,
                name: name
            }).catch(function (err) {
                docomposer.util.alertError("Connection error: ", err);
            });

            setTimeout(function () {
                    let dialog = bootbox.dialog({
                        message: '<div class="text-center"><i class="fas fa-spin fa-spinner"></i> Your are editing [' + name + '.docx] in [' + type + ']</div>',
                        centerVertical: true,
                        closeButton: false
                    }).on('shown.bs.modal', function () {
                        let timer = setInterval(function() {
                            docomposer.api.client.get("/Util/check/" + projectId + "/" + type + "/" + name, {
                                //todo: use request body ? 
                            }).then(function (r) {
                                if (r.data.toString() === "false") {
                                    clearInterval(timer);
                                    dialog.modal('hide')
                                }
                            }).catch(function (err) {
                                clearInterval(timer);
                                dialog.modal('hide')
                                docomposer.util.alertError("Connection error: ", err);
                            });
                        }, 2000);
                    });
                }, 2000);
        }
    }

    this.hide = function () {
        $(_selector.container).hide();
        $(_selector.preview).empty();
    }
}).apply(docomposer.api.doc);