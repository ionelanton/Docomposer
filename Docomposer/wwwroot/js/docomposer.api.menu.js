docomposer.api.menu = docomposer.api.menu || {};
(function () {
    this.sections = function (selector) {
        docomposer.api.client.get('/Menus/Sections/')
            .then((r) => {
                createMenu(selector, r.data);
            });

        function createMenu(selector, data) {
            $(selector).jstree({
                core: {
                    data: data,
                    check_callback: function (operation, node) {
                    },
                    error: function(err) {
                        docomposer.util.alertError(err.reason);
                    }
                },
                types: {
                    root: {
                        icon: "fas fa-home",
                        valid_children: ["folder"]
                    },
                    folder: {
                        icon: "far fa-folder",
                        valid_children: ["file"]
                    },
                    file: {
                        icon: "fas fa-align-left",
                        valid_children: []
                    }
                },
                plugins: [ "types", "contextmenu", "unique", "changed", "sort", "state" ],
                contextmenu: {
                    items: function ($node) {
                        return ContextMenuItems($node, selector, "section");
                    }
                }
            }).bind("rename_node.jstree", function (event, data) {
                
                let tree = $(selector).jstree(true);
                let node = data.node;
                
                switch(node.type) {
                    case "root":
                        break;
                    case "folder":
                        if (node.id.includes("c-")) {
                            OnRenameProject(node, tree);
                        } else {
                            OnCreateProject(node, tree);
                        }
                        break;
                    case "file":
                        if (node.id.includes("s-")) {
                            OnRenameSection(node, tree);
                        } else {
                            OnCreateSection(node, tree);
                        }
                        break;
                }
            }).bind("hide_node.jstree", function (event, data) {
                let tree = $(selector).jstree(true);
                let node = data.node;
                
                switch(node.type) {
                    case "folder":
                        OnDeleteProject(node, tree);
                        break;
                    case "file":
                        OnDeleteSection(node, tree);
                        break;
                }
            });
        }

        WaitForMenuAndShowSelectedSectionIfAny(selector);

        function ContextMenuItems($node, selector, name) {
            let tree = $(selector).jstree(true);
            switch ($node.type) {
                case "root":
                    return {
                        New: {
                            label: "New project",
                            icon: "far fa-folder",
                            action: function () {
                                $node = tree.create_node($node, {
                                    text: 'New project',
                                    type: 'folder',
                                    icon: 'far fa-folder',
                                    a_attr: {
                                        onclick: "docomposer.api.doc.hide();"
                                    }
                                });
                                tree.edit($node);
                            }
                        }
                    }
                case "folder": {
                    return {
                        New: {
                            label: "New " + name,
                            icon: "fas fa-align-left",
                            action: function () {
                                $node = tree.create_node($node, {
                                    text: 'New ' + name,
                                    type: 'file',
                                    icon: 'fas fa-align-left',
                                    a_attr: {
                                        onclick: "",
                                        ondblclick: ""
                                    }
                                });
                                tree.edit($node);
                            }
                        },
                        Rename: {
                            label: "Rename project",
                            icon: "fas fa-edit",
                            action: function () {
                                tree.edit($node);
                            }
                        },
                        Delete: {
                            label: "Delete project",
                            icon: "fas fa-trash",
                            action: function () {
                                bootbox.confirm("Delete project '" + $node.text + "' ?",
                                    function (result) {
                                        if (result) {
                                            tree.hide_node($node, true);
                                        }
                                    });
                            }
                        }
                    }
                }
                default: { // file
                    return {
                        Edit: {
                            label: "Edit " + name,
                            icon: "far fa-file-word",
                            action: function () {
                                docomposer.api.doc.open($node.parent.substring(2),"Sections", $node.text);
                            }
                        },
                        Refresh: {
                            label: "Refresh " + name,
                            icon: "fas fa-sync",
                            action: function () {
                                docomposer.api.doc.preview("Sections", parseInt($node.id.substring(2)), true);
                            }
                        },
                        Rename: {
                            label: "Rename " + name,
                            icon: "fas fa-edit",
                            action: function () {
                                tree.edit($node);
                            }
                        },
                        Delete: {
                            label: "Delete " + name,
                            icon: "fas fa-trash",
                            action: function () {
                                bootbox.confirm("Delete " + name + " '" + $node.text + "' ?",
                                    function (result) {
                                        if (result) {
                                            tree.hide_node($node, true);
                                        }
                                    });
                            }
                        }
                    }
                }
            }
        }

        function OnCreateSection(node, tree) {
            docomposer.api.client.post("/Sections/", {
                projectId: parseInt(node.parent.substring(2)),
                name: node.text
            }).then(function(r) {
                docomposer.util.alertSuccess("Section created");
                tree.set_id(node, "s-" + r.data);
                node.a_attr.onclick = "docomposer.api.doc.preview(\"Sections\", " + r.data + ");";
                node.a_attr.ondblclick = "docomposer.api.doc.open(" + node.parent.substring(2) + ", \"Sections\", \"" + node.text + "\");";
                tree.redraw_node(node);
                tree.select_node(node);
                $("a", "#" + node.id).click();
            }).catch(function(err) {
                docomposer.util.alertError("Error creating section", err);
                tree.delete_node(node);
            });
        }

        function OnRenameSection(node, tree) {
            let id = parseInt(node.id.substring(2));
            docomposer.api.client.put("/Sections/" + id, {
                id: id,
                name: node.text,
                projectId: parseInt(node.parent.substring(2))
            }).then(function() {
                docomposer.util.alertSuccess("Section renamed");
                node.a_attr.ondblclick = "docomposer.api.doc.open(" + node.parent.substring(2) + ", \"Sections\", \"" + node.text + "\");";
                tree.redraw_node(node);
            }).catch(function(err) {
                docomposer.util.alertError("Error renaming section", err);
                if (node.text !== node.original.text) {
                    tree.rename_node(node.id, node.original.text);
                }
            });
        }

        function OnDeleteSection(node, tree) {
            docomposer.api.client.delete("/Sections/" + node.id.substring(2)).then(function() {
                docomposer.util.alertSuccess("Section deleted");
                tree.delete_node(node);
            }).catch(function(err) {
                docomposer.util.alertError("Error deleting section", err);
                tree.show_node(node, true);
            });
        }

        function WaitForMenuAndShowSelectedSectionIfAny(selector) {
            setTimeout(function() {
                let selected = $(selector).jstree(true).get_selected(true);

                if (selected.length > 0 && selected[0].id.includes("s-")) {
                    docomposer.api.doc.preview("Sections", selected[0].id.substring(2));
                }
            }, 1000);
        }
    }

    this.documents = function (selector) {
        docomposer.api.client.get('/Menus/Documents/')
            .then((r) => {
                createMenu(selector, r.data);
            });

        function createMenu(selector, data) {
            $(selector).jstree({
                core: {
                    data: data,
                    check_callback: function (operation, node) {
                    },
                    error: function(err) {
                        docomposer.util.alertError(err.reason);
                    }
                },
                types: {
                    root: {
                        icon: "fas fa-home",
                        valid_children: ["folder"]
                    },
                    folder: {
                        icon: "far fa-folder",
                        valid_children: ["file"]
                    },
                    file: {
                        icon: "far fa-file-alt",
                        valid_children: []
                    }
                },
                plugins: [ "types", "contextmenu", "unique", "changed", "sort", "state" ],
                contextmenu: {
                    items: function ($node) {
                        return ContextMenuItems($node, selector, "template");
                    }
                }
            }).bind("rename_node.jstree", function (event, data) {

                let tree = $(selector).jstree(true);
                let node = data.node;

                switch(node.type) {
                    case "root":
                        break;
                    case "folder":
                        if (node.id.includes("c-")) {
                            OnRenameProject(node, tree);
                        } else {
                            OnCreateProject(node, tree);
                        }
                        break;
                    case "file":
                        if (node.id.includes("d-")) {
                            OnRenameTemplate(node, tree);
                        } else {
                            OnCreateTemplate(node, tree);
                        }
                        break;
                }
            }).bind("hide_node.jstree", function (event, data) {
                let tree = $(selector).jstree(true);
                let node = data.node;

                switch(node.type) {
                    case "folder":
                        OnDeleteProject(node, tree);
                        break;
                    case "file":
                        OnDeleteTemplate(node, tree);
                        break;
                }
            });
        }

        WaitForMenuAndShowSelectedDocumentIfAny(selector);

        function ContextMenuItems($node, selector, name) {
            let tree = $(selector).jstree(true);
            switch ($node.type) {
                case "root":
                    return {
                        New: {
                            label: "New project",
                            icon: "far fa-folder",
                            action: function () {
                                $node = tree.create_node($node, {
                                    text: 'New project',
                                    type: 'folder',
                                    icon: 'far fa-folder',
                                    a_attr: {
                                        onclick: "docomposer.api.doc.hide();"
                                    }
                                });
                                tree.edit($node);
                            }
                        }
                    }
                case "folder": {
                    return {
                        New: {
                            label: "New " + name,
                            icon: "far fa-file-alt",
                            action: function () {
                                $node = tree.create_node($node, {
                                    text: 'New ' + name,
                                    type: 'file',
                                    icon: 'far fa-file-alt',
                                    a_attr: {
                                        onclick: "",
                                        ondblclick: ""
                                    }
                                });
                                tree.edit($node);
                            }
                        },
                        Rename: {
                            label: "Rename project",
                            icon: "fas fa-edit",
                            action: function () {
                                tree.edit($node);
                            }
                        },
                        Delete: {
                            label: "Delete project",
                            icon: "fas fa-trash",
                            action: function () {
                                bootbox.confirm("Delete project '" + $node.text + "' ?",
                                    function (result) {
                                        if (result) {
                                            tree.hide_node($node, true);
                                        }
                                    });
                            }
                        }
                    }
                }
                default: { // file
                    return {
                        Edit: {
                            label: "Edit " + name,
                            icon: "far fa-file-word",
                            action: function () {
                                docomposer.api.doc.open($node.parent.substring(2),"Templates", $node.text);
                            }
                        },
                        Refresh: {
                            label: "Refresh " + name,
                            icon: "fas fa-sync",
                            action: function () {
                                docomposer.api.doc.preview("Documents", parseInt($node.id.substring(2)), true);
                            }
                        },
                        Rename: {
                            label: "Rename " + name,
                            icon: "fas fa-edit",
                            action: function () {
                                tree.edit($node);
                            }
                        },
                        Delete: {
                            label: "Delete " + name,
                            icon: "fas fa-trash",
                            action: function () {
                                bootbox.confirm("Delete " + name + " '" + $node.text + "' ?",
                                    function (result) {
                                        if (result) {
                                            tree.hide_node($node, true);
                                        }
                                    });
                            }
                        }
                    }
                }
            }
        }

        function OnCreateTemplate(node, tree) {
            docomposer.api.client.post("/Documents/", {
                projectId: parseInt(node.parent.substring(2)),
                name: node.text
            }).then(function(r) {
                docomposer.util.alertSuccess("Document created");
                tree.set_id(node, "d-" + r.data);
                node.a_attr.onclick = "docomposer.api.doc.preview(\"Documents\", " + r.data + ");";
                node.a_attr.ondblclick = "docomposer.api.doc.open(" + node.parent.substring(2) + ", \"Documents\", \"" + node.text + "\");";
                tree.redraw_node(node);
                $("a", "#" + node.id).click();
            }).catch(function(err) {
                docomposer.util.alertError("Error creating document", err);
                tree.delete_node(node);
            });
        }

        function OnRenameTemplate(node, tree) {
            let id = parseInt(node.id.substring(2));
            docomposer.api.client.put("/Documents/" + id, {
                id: id,
                name: node.text,
                projectId: parseInt(node.parent.substring(2))
            }).then(function() {
                docomposer.util.alertSuccess("Document renamed");
                node.a_attr.ondblclick = "docomposer.api.doc.open(" + node.parent.substring(2) + ", \"Documents\", \"" + node.text + "\");";
                tree.redraw_node(node);
            }).catch(function(err) {
                docomposer.util.alertError("Error renaming document", err);
                if (node.text !== node.original.text) {
                    tree.rename_node(node.id, node.original.text);
                }
            });
        }

        function OnDeleteTemplate(node, tree) {
            docomposer.api.client.delete("/Documents/" + node.id.substring(2)).then(function() {
                docomposer.util.alertSuccess("Document deleted");
                tree.delete_node(node);
            }).catch(function(err) {
                docomposer.util.alertError("Error deleting document", err);
                tree.show_node(node, true);
            });
        }

        function WaitForMenuAndShowSelectedDocumentIfAny(selector) {
            setTimeout(function() {
                let selected = $(selector).jstree(true).get_selected(true);

                if (selected.length > 0 && selected[0].id.includes("d-")) {
                    docomposer.api.doc.preview("Documents", selected[0].id.substring(2));

                    docomposer.api.client.put("/Util/", {
                        id: parseInt(selected[0].id.substring(2)),
                        type: "Documents",
                        clientInstance: docomposer.util.clientInstance()
                    }).catch(function(err) {
                        console.log(err);
                        docomposer.util.alertError("Connection error: ", err);
                    });
                }
            }, 1000);
        }
    }
    
    this.compositions = function(selector) {
        docomposer.api.client.get('/Menus/Compositions')
            .then((r) => {
                createMenu(selector, r.data);
            });
        
        function createMenu(selector, data) {
            $(selector).jstree({
                core: {
                    data: data,
                    check_callback: function (operation, node) {
                    },
                    error: function(err) {
                        docomposer.util.alertError(err.reason);
                    }
                },
                types: {
                    root: {
                        icon: "fas fa-home",
                        valid_children: ["folder"]
                    },
                    folder: {
                        icon: "far fa-folder",
                        valid_children: ["file"]
                    },
                    file: {
                        icon: "fas fa-layer-group",
                        valid_children: []
                    }
                },
                plugins: [ "types", "contextmenu", "unique", "changed", "sort", "state" ],
                contextmenu: {
                    items: function ($node) {
                        return ContextMenuItems($node, selector, "composition");
                    }
                }
            }).bind("rename_node.jstree", function (event, data) {

                let tree = $(selector).jstree(true);
                let node = data.node;

                switch(node.type) {
                    case "root":
                        break;
                    case "folder":
                        if (node.id.includes("c-")) {
                            OnRenameProject(node, tree);
                        } else {
                            OnCreateProject(node, tree);
                        }
                        break;
                    case "file":
                        if (node.id.includes("o-")) {
                            OnRenameFolder(node, tree);
                        } else {
                            OnCreateFolder(node, tree);
                        }
                        break;
                }
            }).bind("hide_node.jstree", function (event, data) {
                let tree = $(selector).jstree(true);
                let node = data.node;

                switch(node.type) {
                    case "folder":
                        OnDeleteProject(node, tree);
                        break;
                    case "file":
                        OnDeleteFolder(node, tree);
                        break;
                }
            });
        }

        WaitForMenuAndShowSelectedCompositionIfAny(selector);
        
        function ContextMenuItems($node, selector, name) {
            let tree = $(selector).jstree(true);
            switch ($node.type) {
                case "root":
                    return {
                        New: {
                            label: "New project",
                            icon: "far fa-folder",
                            action: function () {
                                $node = tree.create_node($node, {
                                    text: 'New project',
                                    type: 'folder',
                                    icon: 'far fa-folder',
                                    a_attr: {
                                        onclick: "docomposer.api.doc.hide();"
                                    }
                                });
                                tree.edit($node);
                            }
                        }
                    }
                case "folder": {
                    return {
                        New: {
                            label: "New " + name,
                            icon: "fas fa-layer-group",
                            action: function () {
                                $node = tree.create_node($node, {
                                    text: 'New ' + name,
                                    type: 'file',
                                    icon: 'fas fa-layer-group',
                                    a_attr: {
                                        onclick: "",
                                        ondblclick: ""
                                    }
                                });
                                tree.edit($node);
                            }
                        },
                        Rename: {
                            label: "Rename project",
                            icon: "fas fa-edit",
                            action: function () {
                                tree.edit($node);
                            }
                        },
                        Delete: {
                            label: "Delete project",
                            icon: "fas fa-trash",
                            action: function () {
                                bootbox.confirm("Delete project '" + $node.text + "' ?",
                                    function (result) {
                                        if (result) {
                                            tree.hide_node($node, true);
                                        }
                                    });
                            }
                        }
                    }
                }
                default: { // file
                    return {
                        Refresh: {
                            label: "Refresh " + name,
                            icon: "fas fa-sync",
                            action: function () {
                                docomposer.api.doc.preview("Compositions", parseInt($node.id.substring(2)), true);
                            }
                        },
                        Rename: {
                            label: "Rename " + name,
                            icon: "fas fa-edit",
                            action: function () {
                                tree.edit($node);
                            }
                        },
                        Delete: {
                            label: "Delete " + name,
                            icon: "fas fa-trash",
                            action: function () {
                                bootbox.confirm("Delete " + name + " '" + $node.text + "' ?",
                                    function (result) {
                                        if (result) {
                                            tree.hide_node($node, true);
                                        }
                                    });
                            }
                        }
                    }
                }
            }
        }

        function OnCreateFolder(node, tree) {
            docomposer.api.client.post("/Compositions/", {
                projectId: parseInt(node.parent.substring(2)),
                name: node.text
            }).then(function(r) {
                docomposer.util.alertSuccess("Folder created");
                tree.set_id(node, "o-" + r.data);
                node.a_attr.onclick = "docomposer.api.doc.preview(\"Compositions\", " + r.data + ");";
                node.a_attr.ondblclick = "docomposer.api.doc.open(" + node.parent.substring(2) + ", \"Compositions\", \"" + node.text + "\");";
                tree.redraw_node(node);
                $("a", "#" + node.id).click();
            }).catch(function(err) {
                docomposer.util.alertError("Error creating folder", err);
                tree.delete_node(node);
            });
        }

        function OnRenameFolder(node, tree) {
            let id = parseInt(node.id.substring(2));
            docomposer.api.client.put("/Compositions/" + id, {
                id: id,
                name: node.text,
                projectId: parseInt(node.parent.substring(2))
            }).then(function() {
                docomposer.util.alertSuccess("Folder renamed");
                node.a_attr.ondblclick = "docomposer.api.doc.open(" + node.parent.substring(2) + ", \"Folder\", \"" + node.text + "\");";
                tree.redraw_node(node);
            }).catch(function(err) {
                docomposer.util.alertError("Error renaming folder", err);
                if (node.text !== node.original.text) {
                    tree.rename_node(node.id, node.original.text);
                }
            });
        }

        function OnDeleteFolder(node, tree) {
            docomposer.api.client.delete("/Compositions/" + node.id.substring(2)).then(function() {
                docomposer.util.alertSuccess("Folder deleted");
                tree.delete_node(node);
            }).catch(function(err) {
                docomposer.util.alertError("Error deleting folder", err);
                tree.show_node(node, true);
            });
        }

        function WaitForMenuAndShowSelectedCompositionIfAny(selector) {
            setTimeout(function() {
                let selected = $(selector).jstree(true).get_selected(true);

                if (selected.length > 0 && selected[0].id.includes("o-")) {
                    docomposer.api.doc.preview("Compositions", selected[0].id.substring(2));

                    docomposer.api.client.put("/Util/", {
                        id: parseInt(selected[0].id.substring(2)),
                        type: "Compositions",
                        clientInstance: docomposer.util.clientInstance()
                    }).catch(function(err) {
                        console.log(err);
                        docomposer.util.alertError("Connection error: ", err);
                    });
                }
            }, 1000);
        }
    }

    this.dataSources = function(selector) {
        docomposer.api.client.get('/Menus/DataSources')
            .then((r) => {
                createMenu(selector, r.data);
            });

        function createMenu(selector, data) {
            $(selector).jstree({
                core: {
                    data: data,
                    check_callback: function (operation, node) {
                    },
                    error: function(err) {
                        docomposer.util.alertError(err.reason);
                    }
                },
                types: {
                    root: {
                        icon: "fas fa-home",
                        valid_children: ["folder"]
                    },
                    folder: {
                        icon: "fas fa-folder",
                        valid_children: ["file"]
                    },
                    file: {
                        icon: "fas fa-database",
                        valid_children: []
                    }
                },
                plugins: [ "types", "contextmenu", "unique", "changed", "sort", "state" ],
                contextmenu: {
                    items: function ($node) {
                        return ContextMenuItems($node, selector, "data source");
                    }
                }
            }).bind("rename_node.jstree", function (event, data) {
                let tree = $(selector).jstree(true);
                let node = data.node;

                switch(node.type) {
                    case "root":
                        break;
                    case "folder":
                        if (node.id.includes("c-")) {
                            OnRenameProject(node, tree);
                        } else {
                            OnCreateProject(node, tree);
                        }
                        break;
                    case "file":
                        if (node.id.includes("d-")) {
                            OnRenameDataSource(node, tree);
                        } else {
                            OnCreateDataSource(node, tree);
                        }
                        break;
                }
            }).bind("hide_node.jstree", function (event, data) {
                let tree = $(selector).jstree(true);
                let node = data.node;

                switch(node.type) {
                    case "folder":
                        OnDeleteProject(node, tree);
                        break;
                    case "file":
                        OnDeleteDataSource(node, tree);
                        break;
                }
            });
        }

        WaitForMenuAndShowSelectedDataSourcesIfAny(selector);

        function ContextMenuItems($node, selector, name) {
            let tree = $(selector).jstree(true);
            switch ($node.type) {
                case "root":
                    return {
                        New: {
                            label: "New project",
                            icon: "far fa-folder",
                            action: function () {
                                $node = tree.create_node($node, {
                                    text: 'New project',
                                    type: 'folder',
                                    icon: 'far fa-folder',
                                    a_attr: {
                                        onclick: "alert('clear page')"
                                    }
                                });
                                tree.edit($node);
                            }
                        }
                    }
                case "folder": {
                    return {
                        New: {
                            label: "New " + name,
                            icon: "fas fa-database",
                            action: function () {
                                $node = tree.create_node($node, {
                                    text: 'New ' + name,
                                    type: 'file',
                                    icon: 'fas fa-database',
                                    a_attr: {
                                        onclick: "",
                                        ondblclick: ""
                                    }
                                });
                                tree.edit($node);
                            }
                        },
                        Rename: {
                            label: "Rename project",
                            icon: "fas fa-edit",
                            action: function () {
                                tree.edit($node, null, function(n) {
                                    
                                });
                            }
                        },
                        Delete: {
                            label: "Delete project",
                            icon: "fas fa-trash",
                            action: function () {
                                bootbox.confirm("Delete project '" + $node.text + "' ?",
                                    function (result) {
                                        if (result) {
                                            tree.hide_node($node, true);
                                        }
                                    });
                            }
                        }
                    }
                }
                default: { // file
                    return {
                        Rename: {
                            label: "Rename " + name,
                            icon: "fas fa-edit",
                            action: function () {
                                tree.edit($node);
                            }
                        },
                        Delete: {
                            label: "Delete " + name,
                            icon: "fas fa-trash",
                            action: function () {
                                bootbox.confirm("Delete " + name + " '" + $node.text + "' ?",
                                    function (result) {
                                        if (result) {
                                            tree.hide_node($node, true);
                                        }
                                    });
                            }
                        }
                    }
                }
            }
        }

        function OnCreateDataSource(node, tree) {
            docomposer.api.client.post("/DataSources/", {
                projectId: parseInt(node.parent.substring(2)),
                name: node.text,
                configuration: "{}"
            }).then(function(r) {
                docomposer.util.alertSuccess("Data source created");
                tree.set_id(node, "d-" + r.data);
                node.a_attr.onclick = "docomposer.api.datasource.get(" + r.data + ")";
                tree.redraw_node(node);
                $("a", "#" + node.id).click();
            }).catch(function(err) {
                docomposer.util.alertError("Error creating data source", err);
                tree.delete_node(node);
            });
        }

        function OnRenameDataSource(node, tree) {
            let id = parseInt(node.id.substring(2));
            docomposer.api.client.put("/DataSources/" + id, {
                id: id,
                name: node.text,
                projectId: parseInt(node.parent.substring(2))
            }).then(function() {
                docomposer.util.alertSuccess("Data source renamed");
                tree.redraw_node(node);
                docomposer.api.datasource.get(id);
            }).catch(function(err) {
                docomposer.util.alertError("Error renaming data source", err);
                if (node.text !== node.original.text) {
                    tree.rename_node(node.id, node.original.text);
                }
            });
        }

        function OnDeleteDataSource(node, tree) {
            docomposer.api.client.delete("/DataSources/" + node.id.substring(2)).then(function() {
                docomposer.util.alertSuccess("Data source deleted");
                tree.delete_node(node);
                docomposer.api.datasource.hide();
            }).catch(function(err) {
                console.log(err)
                docomposer.util.alertError("Error deleting data source", err);
                tree.show_node(node, true);
            });
        }

        function WaitForMenuAndShowSelectedDataSourcesIfAny(selector) {
            setTimeout(function() {
                let selected = $(selector).jstree(true).get_selected(true);
                if (selected.length > 0 && selected[0].id.includes("d-")) {
                    docomposer.api.datasource.get(selected[0].id.substring(2));
                }
            }, 1000);
        }
    }

    this.workflows = function(selector) {
        docomposer.api.client.get('/Menus/Workflows')
            .then((r) => {
                createMenu(selector, r.data);
            });

        function createMenu(selector, data) {
            $(selector).jstree({
                core: {
                    data: data,
                    check_callback: function (operation, node) {
                    },
                    error: function(err) {
                        docomposer.util.alertError(err.reason);
                    }
                },
                types: {
                    root: {
                        icon: "fas fa-home",
                        valid_children: ["folder"]
                    },
                    folder: {
                        icon: "far fa-folder",
                        valid_children: ["file"]
                    },
                    file: {
                        icon: "fas fa-cog",
                        valid_children: []
                    }
                },
                plugins: [ "types", "contextmenu", "unique", "changed", "sort", "state" ],
                contextmenu: {
                    items: function ($node) {
                        return ContextMenuItems($node, selector, "workflow");
                    }
                }
            }).bind("rename_node.jstree", function (event, data) {

                let tree = $(selector).jstree(true);
                let node = data.node;

                switch(node.type) {
                    case "root":
                        break;
                    case "folder":
                        if (node.id.includes("c-")) {
                            OnRenameProject(node, tree);
                        } else {
                            OnCreateProject(node, tree);
                        }
                        break;
                    case "file":
                        if (node.id.includes("p-")) {
                            OnRenameWorkflow(node, tree);
                        } else {
                            OnCreateWorkflow(node, tree);
                        }
                        break;
                }
            }).bind("hide_node.jstree", function (event, data) {
                let tree = $(selector).jstree(true);
                let node = data.node;

                switch(node.type) {
                    case "folder":
                        OnDeleteProject(node, tree);
                        break;
                    case "file":
                        OnDeleteWorkflow(node, tree);
                        break;
                }
            });
        }

        WaitForMenuAndShowSelectedWorkflowIfAny(selector);

        function ContextMenuItems($node, selector, name) {
            let tree = $(selector).jstree(true);
            switch ($node.type) {
                case "root":
                    return {
                        New: {
                            label: "New project",
                            icon: "far fa-folder",
                            action: function () {
                                $node = tree.create_node($node, {
                                    text: 'New project',
                                    type: 'folder',
                                    icon: 'far fa-folder',
                                    a_attr: {
                                        onclick: "docomposer.api.doc.hide();"
                                    }
                                });
                                tree.edit($node);
                            }
                        }
                    }
                case "folder": {
                    return {
                        New: {
                            label: "New " + name,
                            icon: "fas fa-cog",
                            action: function () {
                                $node = tree.create_node($node, {
                                    text: 'New ' + name,
                                    type: 'file',
                                    icon: 'fas fa-cog',
                                    a_attr: {
                                        onclick: "",
                                        ondblclick: ""
                                    }
                                });
                                tree.edit($node);
                            }
                        },
                        Rename: {
                            label: "Rename project",
                            icon: "fas fa-edit",
                            action: function () {
                                tree.edit($node);
                            }
                        },
                        Delete: {
                            label: "Delete project",
                            icon: "fas fa-trash",
                            action: function () {
                                bootbox.confirm("Delete project '" + $node.text + "' ?",
                                    function (result) {
                                        if (result) {
                                            tree.hide_node($node, true);
                                        }
                                    });
                            }
                        }
                    }
                }
                default: { // file
                    return {
                        Rename: {
                            label: "Rename " + name,
                            icon: "fas fa-edit",
                            action: function () {
                                tree.edit($node);
                            }
                        },
                        Delete: {
                            label: "Delete " + name,
                            icon: "fas fa-trash",
                            action: function () {
                                bootbox.confirm("Delete " + name + " '" + $node.text + "' ?",
                                    function (result) {
                                        if (result) {
                                            tree.hide_node($node, true);
                                        }
                                    });
                            }
                        }
                    }
                }
            }
        }

        function OnCreateWorkflow(node, tree) {
            docomposer.api.client.post("/Workflows/", {
                projectId: parseInt(node.parent.substring(2)),
                name: node.text
            }).then(function(r) {
                docomposer.util.alertSuccess("Workflow created");
                tree.set_id(node, "p-" + r.data);
                node.a_attr.onclick = "docomposer.api.workflow.get(" + r.data + ");";
                tree.redraw_node(node);
                $("a", "#" + node.id).click();
            }).catch(function(err) {
                docomposer.util.alertError("Error creating workflow", err);
                tree.delete_node(node);
            });
        }

        function OnRenameWorkflow(node, tree) {
            let id = parseInt(node.id.substring(2));
            docomposer.api.client.put("/Workflows/" + id, {
                id: id,
                name: node.text,
                projectId: parseInt(node.parent.substring(2))
            }).then(function() {
                docomposer.util.alertSuccess("Workflow renamed");
                tree.redraw_node(node);
                docomposer.api.workflow.get(id);
            }).catch(function(err) {
                docomposer.util.alertError("Error renaming workflow", err);
                if (node.text !== node.original.text) {
                    tree.rename_node(node.id, node.original.text);
                }
            });
        }

        function OnDeleteWorkflow(node, tree) {
            docomposer.api.client.delete("/Workflows/" + node.id.substring(2)).then(function() {
                docomposer.util.alertSuccess("Workflow deleted");
                tree.delete_node(node);
                docomposer.api.workflow.hide();
            }).catch(function(err) {
                docomposer.util.alertError("Error deleting workflow", err);
                tree.show_node(node, true);
            });
        }

        function WaitForMenuAndShowSelectedWorkflowIfAny(selector) {
            setTimeout(function() {
                let selected = $(selector).jstree(true).get_selected(true);

                if (selected.length > 0 && selected[0].id.includes("p-")) {
                    docomposer.api.client.put("/Util/", {
                        id: parseInt(selected[0].id.substring(2)),
                        type: "Workflows",
                        clientInstance: docomposer.util.clientInstance()
                    }).catch(function(err) {
                        console.log(err);
                        docomposer.util.alertError("Connection error: ", err);
                    });
                }
            }, 1000);
        }
    }
    
    function OnCreateProject(node, tree) {
        docomposer.api.client.post("/Projects/", {
            name: node.text
        }).then(function(r) {
            tree.set_id(node, "c-" + r.data);
            $("a", "#" + node.id).click();
            docomposer.util.alertSuccess("Project created.");
        }).catch(function(err) {
            docomposer.util.alertError("Error creating project: ", err);
            tree.delete_node(node);
        });
    }
    
    function OnRenameProject(node, tree) {
        let id = node.id.substring(2);
        docomposer.api.client.put("/Projects/" + id, {
            id: id,
            name: node.text
        }).then(function() {
            docomposer.util.alert("Project renamed");
        }).catch(function(err) {
            docomposer.util.alertError("Error renaming project", err);
            if (node.text !== node.original.text) {
                tree.rename_node(node.id, node.original.text);
            }
        });
    }

    function OnDeleteProject(node, tree) {
        docomposer.api.client.delete("/Projects/" + node.id.substring(2)).then(function() {
            docomposer.util.alert("Project deleted.", "warning");
            tree.delete_node(node);
        }).catch(function(err) {
            docomposer.util.alertError("Error deleting project", err);
            tree.show_node(node, true);
        });
    }
    
}).apply(docomposer.api.menu);