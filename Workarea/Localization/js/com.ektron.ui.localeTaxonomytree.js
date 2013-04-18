function Tree(text, id, parentId, data) {
    this.node = new Node(text, id, parentId, data);
    this.children = new Array();

    this.appendNode = function(node) {
        this.children[this.children.length] = node;
    }

    // keep around reference
    TREES[TreeUtil.getTreeId(id)] = this;
}
function Node(n, id, parentId, data) {
    this.name = n;
    this.action = onToggleClick;
    this.iconOp = null;
    this.iconCl = null;
    this.id = id;
    this.parentId = parentId;
    this.data = data;
    this.status = "closed";
    this.renderedSelf = false;
    this.renderedChildren = false;

    this.setAction = function(actn) { this.action = actn; }
    this.setOpenIcon = function(icon) { this.iconOp = icon; }
    this.setCloseIcon = function(icon) { this.iconCl = icon; }
}

var TREES = new Array();

var TreeDisplayUtil =
{
    getAncestry: function(id, ancestry) {
        ancestry = ancestry != null ? ancestry : [];
        var tree = TreeUtil.getTreeById(id);

        if (tree != null) {
            ancestry[ancestry.length] = tree;
            var id = tree.node.id;
            var parentId = TreeDisplayUtil.getParentId(id);
            if (parentId != -1) {
                return TreeDisplayUtil.getAncestry(parentId, ancestry);
            }
            else {
                return ancestry;
            }
        }

        return null;
    },

    getRootNodeId: function(id) {
        var ancestry = TreeDisplayUtil.getAncestry(id);
        var rootNodeId = null;

        if (ancestry != null) {
            if (ancestry.length > 0) {
                var root = ancestry[ancestry.length - 1];
                rootNodeId = root.node.id;
            }
        }

        return rootNodeId;
    },

    getParentId: function(id) {
        var tree = TreeUtil.getTreeById(id);
        var parentId = null;

        if (tree != null) {
            parentId = -1;
            if (tree.node.parentId != null) {
                parentId = tree.node.parentId;
            }
        }

        return parentId;
    },

    getElementById: function(id) {
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"getElementById(" + id + ")",
				"starting...");

        var element = document.getElementById(
						TreeUtil.getTreeId(id));

        return element;
    },

    reloadTree: function(id) {
        var tree = TreeUtil.getTreeById(id);

        if (tree != null) {
            if (tree.node.renderedChildren != true) {
                TreeDisplayUtil.expandTree(tree);
            }
            else {
                tree.node.renderedChildren = false;
                var containerId = TreeUtil.getChildId(id);
                var containerElement = document.getElementById(containerId);
                containerElement.parentNode.removeChild(containerElement);

                //alert(TreeUtil.getTreeId(tree.node.id));
                TREES[TreeUtil.getTreeId(tree.node.id)].children = [];
                TreeDisplayUtil.expandTree(tree);
            }
        }
    },

    removeTree: function(id) {
        var tree = TreeUtil.getTreeById(id);

        if (tree != null) {
            // Set the renderedChildren flag to false
            tree.node.renderedChildren = false;

            // Remove the children from the DOM
            var containerId = TreeUtil.getChildId(id);
            var containerElement = document.getElementById(containerId);
            containerElement.parentNode.removeChild(containerElement);

            // This is where tree data is cached. Remove the child data from it.
            TREES[TreeUtil.getTreeId(tree.node.id)].children = [];
        }
    },

    reloadParentTree: function(id) {
        var tree = TreeUtil.getTreeById(id);

        if (tree != null) {
            var parentId = tree.node.parentId;
            TreeDisplayUtil.reloadTree(parentId);
        }
    },

    getChildElementById: function(id) {
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"getChildElementById(" + id + ")",
				"starting...");

        var childId = TreeUtil.getChildId(id);
        return document.getElementById(childId);
    },

    toggleTree: function(parentId) {
        document.getElementById("LastClickedOn").value = parentId;
        var tree = TreeUtil.getTreeById(parentId);

        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"toggleTree(" + parentId + ")",
				"tree.node.status = " + tree.node.status);

        if (tree.node.status == "open") {
            TreeDisplayUtil.collapseTree(tree);
        }
        else {
            TreeDisplayUtil.expandTree(tree);
        }
    },

    expandTreeSet: function(ids) {
        return TreeDisplayUtil.__expandTreeSet([ids]);
    },

    __expandTreeSet: function(vargs) {
        var retVal = false;
        var ids = [];
        if (vargs.length > 0) {
            ids = vargs[0];
        }

        var currentIndex = 0;
        if (vargs.length > 1) {
            currentIndex = vargs[1];
            currentIndex++;
        }

        onComplete = TreeDisplayUtil.expandTreeSet;
        if (ids.length > 0) {
            var id = ids[currentIndex];
            if (id != null) {
                var vargs = [ids, currentIndex, onComplete];
                var tree = TreeUtil.getTreeById(id);
                if (tree != null && tree.node.renderedChildren == true) {
                    retVal = TreeDisplayUtil.showChildren(tree);
                    if (retVal) {
                        retVal = TreeDisplayUtil.__expandTreeSet(vargs);
                    }
                }
                else {
                    retVal = TreeDisplayUtil.expandTreeById(id, TreeDisplayUtil.__expandTreeSet, vargs);
                }
            }
        }

        return retVal;
    },

    expandTree: function(tree) {
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"expandTree(" + tree.node.id + ")",
				"starting...");

        // the "tree.node.id == 0" is a workaround for a bug on in the ContentService
        // that has the root element returning hasChildren = false.
        if (tree.node.data.hasChildren == "true") {
            if (!tree.node.renderedChildren) {
                tree.node.renderedChildren = true;
                TreeUtil.addChildren(tree.node.id);
            }
            else {
                TreeDisplayUtil.showChildren(tree);
            }
        }
    },

    expandTreeEx: function(tree, onComplete, vargs) {
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"expandTreeEx(" + tree.node.id + ")",
				"starting...");

        // the "tree.node.id == 0" is a workaround for a bug on in the ContentService
        // that has the root element returning hasChildren = false.
        if (tree.node.data.hasChildren == "true") {
            if (!tree.node.renderedChildren) {
                tree.node.renderedChildren = true;
                TreeUtil.addChildrenEx(tree.node.id, onComplete, vargs);
            }
            else {
                TreeDisplayUtil.showChildren(tree);
            }
        }
    },

    expandTreeById: function(id) {
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"expandTreeById(" + id + ")",
				"starting...");

        var tree = TreeUtil.getTreeById(id);
        TreeDisplayUtil.expandTree(tree);
    },

    expandTreeById: function(id, onComplete, vargs) {
        var retVal = false;
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"expandTreeById(" + id + ")",
				"starting...");

        var tree = TreeUtil.getTreeById(id);
        if (tree) {
            TreeDisplayUtil.expandTreeEx(tree, onComplete, vargs);
            retVal = true;
        }
        else {
            retVal = false;
        }
        return (retVal);
    },

    collapseTree: function(tree) {
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"collapseTree(" + tree.node.id + ")",
				"starting...");

        if (tree.node.data.hasChildren == "true") {
            TreeDisplayUtil.hideChildren(tree);
        }
    },

    hideChildren: function(tree) {
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"hideChildren(" + tree.node.id + ")",
				"starting...");

        var element = TreeDisplayUtil.getChildElementById(tree.node.id);
        if (element == null) {
            return; // Happens when loading the root node
        }

        element.style["display"] = "none";
        tree.node.status = "closed";

        var iconElement = document.getElementById("I" + tree.node.id);
        if (iconElement) {
            if (TreeDisplayUtil.plusclosefolders == undefined) {
                iconElement.src = TreeDisplayUtil.plusclosefolder;
            }
            else {
                iconElement.src = TreeDisplayUtil.plusclosefolders[tree.node.data.type];
            }
        }
    },

    showChildren: function(tree) {
        var retVal = false;
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"showChildren(" + tree.node.id + ")",
				"starting...");

        var childElement = TreeDisplayUtil.getChildElementById(tree.node.id);
        var iconElement = document.getElementById("I" + tree.node.id);

        if (!childElement) {
            retVal = false;
            LogUtil.addMessage(LogUtil.CRITICAL,
					"TreeDisplayUtil",
					"showChildren(" + tree.node.id + ")",
					"Couldn't find container for child element");
        }
        else {
            childElement.style["display"] = "";

            if (iconElement) {
                if (TreeDisplayUtil.minusclosefolders == undefined) {
                    iconElement.src = TreeDisplayUtil.minusclosefolder;
                }
                else {
                    iconElement.src = TreeDisplayUtil.minusclosefolders[tree.node.data.type];
                }
            }

            tree.node.status = "open";
            if ("true" == $ektron(".ShowCheckBoxTaxonomy").val().toLowerCase())
            {
                TreeDisplayUtil.UpdateSubTree(tree.node.data.id);
            }
            retVal = true;
        }
        return (retVal);
    },

    UpdateSubTree: function(parentTreeId)
    {	 
        // #55469: timing issue. the subTree are invisible/hidden and therefore this following might not work if subTree are not yet expanded.
        var isParentChecked = $ektron("input#chkTree_CB" + parentTreeId).attr("checked");
        if (true == isParentChecked)
        {
            var $ekSubTree = $ektron("ul#C" + parentTreeId);
            $ektron("input:checkbox", $ekSubTree).attr("checked", isParentChecked).attr("disabled", isParentChecked);
        }
    },
    
    showSelf: function(tree, container) {
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"showSelf(" + tree.node.id + ")",
				"starting...");

        if (!tree.node.rendered) {
            TreeDisplayUtil.renderSelf(tree, container);
        }
        if (tree.node.data.id == 0 || tree.node.data.id == __TaxonomyOverrideId) {
            var element = TreeDisplayUtil.getElementById(tree.node.id);
            if (element != null) {
                element.style["display"] = "";
            }
            tree.node.status = "closed";
        }
        else {
            tree.node.status = "open";
        }
    },

    renderChildren: function(tree) {
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"renderChildren(" + tree.node.id + ")",
				"starting...");
        var childContainerId = TreeUtil.getChildId(tree.node.id);
        var ul = document.createElement("ul");
        ul.setAttribute("className", "ekSubTree");
        ul.setAttribute("id", childContainerId);
        var numberChildren = tree.children.length;
        for (var i = 0; i < numberChildren; i++) {
            var child = tree.children[i];
            var li = document.createElement("li");
            li.setAttribute("className", "ekTreeItem");
            li.setAttribute("id", TreeUtil.getTreeId(child.node.id));
            li.innerHTML = TreeDisplayUtil.createNodeHTML(child);
            ul.appendChild(li);
            child.node.rendered = true;

        }
        tree.node.renderedChildren = true;

        var parentElement = document.getElementById(TreeUtil.getTreeId(tree.node.id));
        if (parentElement != null) {
            parentElement.appendChild(ul);
        }
    },

    displayContents: function(id, clickedElement) {
        return onFolderClick(id, clickedElement);
    },


    renderSelf: function(tree, container) {
        var src = "";
        if (tree.node.data.hasChildren == "true") {
            if (TreeDisplayUtil.plusclosefolders == undefined) {
                src = TreeDisplayUtil.plusclosefolder;
            }
            else {
                src = TreeDisplayUtil.plusclosefolders[0];
            }
        }
        else {
            if (TreeDisplayUtil.folders == undefined) {
                src = TreeDisplayUtil.folder;
            }
            else {
                src = TreeDisplayUtil.folders[0];
            }
        }
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"renderSelf(" + tree.node.id + ")",
				"starting...");

        var ul = document.createElement("ul");
        ul.setAttribute("className", "ektree");
        var li = document.createElement("li");
        if (tree.node.data.id == 0) { li.setAttribute("className", "ekTreeRootItem"); }
        li.setAttribute("id", TreeUtil.getTreeId(tree.node.id));

        var strClass = "class='hasChildIcon'";
        var buffer = "";
        var buffer1 = "";
        var chkcheckedoption = " ";
        var chkvalue = false;
        if (tree.node.data.id != 0) {
            buffer += "<img treeid='" + tree.node.id + "' onclick='TreeDisplayUtil.toggleTree(" + tree.node.id + ")' " + strClass + "id='I" + tree.node.id + "' src='" + src + "'>";
        }
        buffer1 += "<span treeid='" + tree.node.id + "' onclick='TreeDisplayUtil.expandTreeById(" + tree.node.id + ")'>";
        buffer1 += "<a href='#' onclick='TreeDisplayUtil.displayContents(" + tree.node.id + ", this);return false;' id=" + tree.node.id + ">"
        if (tree.node.id == 0 && taxonomytreemode == "editor") { buffer += buffer1; }
        else if (taxonomytreemode == "editor" && __TaxonomyOverrideId > 0) {

            if (fetchtaxonomyid(tree.node.id)) {
                chkvalue = true;
                chkcheckedoption = " checked=true ";
            }
            else if (fetchdisabletaxonomyid(tree.node.id)) {
                chkcheckedoption = " checked=true disabled=true ";
            }
            buffer += buffer1 + tree.node.name + " </a>";
        }
        else {
            buffer += buffer1 + tree.node.name + " </a>";
        }
        buffer += "</span>";

        li.innerHTML = buffer
        ul.appendChild(li);

        if (!container) {
            container = document.getElementById("TreeOutput");
        }
        if (container != null) {
            if (ul.outerHTML == 'undefined') {
                container.innerHTML = ul.outerHTML;
            }
            else {
                container.innerHTML = '<UL class=ektree>' + ul.innerHTML + '</UL>';
            }
        }

        tree.node.rendered = true;
    },

    createNodeHTML: function(tree) {
        var src = "";
        if (tree.node.data.hasChildren == "true") {
            if (TreeDisplayUtil.plusclosefolders == undefined) {
                src = TreeDisplayUtil.plusclosefolder;
            }
            else {
                src = TreeDisplayUtil.plusclosefolders[0];
            }
        }
        else {
            if (TreeDisplayUtil.folders == undefined) {
                src = TreeDisplayUtil.folder;
            }
            else {
                src = TreeDisplayUtil.folders[0];
            }
        }
        var strClass = "";
        strClass = "class='hasChildIcon'";

        var buffer = "";
        var buffer1 = "";
        var chkcheckedoption = " ";
        var chkvalue = false;
        if (tree.node.id != 0) {
            buffer += "<img treeid='" + tree.node.id + "' onclick='TreeDisplayUtil.toggleTree(" + tree.node.id + ");' " + strClass + "id='I" + tree.node.id + "' src='" + src + "'>";
        }
        if (taxonomytreemode == "editor") {
            if (fetchtaxonomyid(tree.node.id)) {
                chkvalue = true;
                chkcheckedoption = " checked=true ";
            }
            else if (fetchdisabletaxonomyid(tree.node.id)) {
                chkcheckedoption = " checked=true disabled=true ";
            }
        }
        //         buffer1 += "<span treeid='" + tree.node.id + "' " + strOnMouseOver + strOnMouseOut + " onclick='TreeDisplayUtil.expandTreeById(" + tree.node.id + ")'>";
        //        buffer1 += "<a href='#' onclick='TreeDisplayUtil.displayContents(" + tree.node.id + ", this);return false;' id=" + tree.node.id + ">"
        //get the hidden value of Check boxes.
        var showCheckBoxVar = $ektron(".ShowCheckBoxTaxonomy");
        var ShowcheckBoxesVar = showCheckBoxVar[0].value;
        if (ShowcheckBoxesVar == "True") {
            var rootId = this.getRootNodeId(tree.node.id);
            buffer += "<input type=\"hidden\" id=\"chkTree_T" + tree.node.id + "\" name=\"chkTree_T" + tree.node.id + "\" value=\"" + chkvalue + "\"/><input type=\"checkbox\" id=\"chkTree_CB" + tree.node.id + "\" name=\"chkTree\"" +
                ((TreeUtil.nodeIsPrechecked(tree.node.id) || (tree.node.parentId != rootId && TreeUtil.nodeIsPrechecked(tree.node.parentId))) ? " checked" : "") +
                " value=\"" + tree.node.id + "\" onclick=\"javascript:selecttaxonomy(this);\"" + chkcheckedoption + "/>";
            if (tree.node.data.visible == "false")
                buffer += "";
            else
                buffer += "<label for=\"chkTree_CB" + tree.node.id + "\">" + tree.node.name + "</label>";
        }
        else {
            buffer += "<span treeid='" + tree.node.id + "' onclick='TreeDisplayUtil.expandTreeById(" + tree.node.id + ")'>";
            if (tree.node.data.visible == "false")
                buffer += "<a class='linkStyleDisabled' href='localeTaxonomy.aspx?action=view&view=locale&taxonomyid=" + tree.node.id + "&treeViewId=-1&LangType=" + tree.node.data.langid + "'  id='" + tree.node.id + "'>"
            else
                buffer += "<a class='linkStyle' href='localeTaxonomy.aspx?action=view&view=locale&taxonomyid=" + tree.node.id + "&treeViewId=-1&LangType=" + tree.node.data.langid + "'  id='" + tree.node.id + "'>"
            buffer += tree.node.name + " </a>";
            buffer += "</span>";
        }


        // buffer += "(<span id=\"TIC" + tree.node.id + "\">" + tree.node.data.count + "</span>)";
        //buffer += "</span>";
        return buffer;
    }
}

var TreeUtil =
{
    appendChildren: function(children, vargs) {
        // This arguments are passed above in addChildren via the callback
        var parentId = vargs[0];
        var onComplete = vargs[1];
        var onCompleteArgs = vargs[2];

        var element = document.getElementById("load" + parentId);
        if (element) {
            var parent = element.parentNode;
            parent.removeChild(element);
        }


        var tree = TreeUtil.getTreeById(parentId);

        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeUtil",
				"appendChildren( children, " + parentId + ", onCompleteCallback )",
				"starting...");

        if (children != null) {
            // for each child, make a new node, and append it to tree
            for (var i = 0; i < children.assets.length; i++) {
                var child = children.assets[i];
                tree.appendNode(new Tree(child.name, child.id, parentId, child, 0));
            }
        }
        TreeDisplayUtil.renderChildren(tree);
        TreeDisplayUtil.showChildren(tree);
        if (onComplete != null) {
            onComplete(onCompleteArgs);
        }
        else {
            LogUtil.addMessage(LogUtil.WARNING,
				"TreeUtil",
				"appendChildren( children, " + parentId + ", onCompleteCallback )",
				"failed to call onComplete callback (probably not specified, since not required)");
        }
    },

    precheckedNodes: null,
    nodeIsPrechecked: function(id) {
        if (this.precheckedNodes == null) {
            var qs = location.href.substring(location.href.indexOf('?') || 0);
            if (qs != '')
                qs = qs.substring(1);
            var params = qs.split('&');
            var tax = '';
            for (var i = 0; i < params.length; i++) {
                var s = params[i];
                if (s.length > 1 && s.substring(0, 2).toLowerCase() == 't=') {
                    tax = s.substring(2);
                    break;
                }
            }
            var taxlist = tax.split(',');
            var taxarr = new Array(taxlist.length);
            for (var i = 0; i < taxlist.length; i++)
                taxarr.push(taxlist[i] - 0);

            this.precheckedNodes = taxarr;
        }

        for (var i = 0; i < this.precheckedNodes.length; i++) {
            if (this.precheckedNodes[i] == id) {
                return true;
            }
        }
        return false;
    },

    addChildren: function() {
        switch (arguments.length) {
            case 1:
                // we get one argument, the parent id
                var parentId = arguments[0];
                var tree = this.getTreeById(parentId);

                LogUtil.addMessage(LogUtil.DEBUG,
						"TreeUtil",
						"addChildren(" + parentId + ")",
						"starting...");

                // Each tree node defines the method in which it
                // obtains its child data. Get its action here.
                var getChildContents = tree.node.action;

                // display loading message
                var loadMessage = document.createElement("div");
                loadMessage.id = "load" + parentId;
                loadMessage.className = "loadingMessage";
                loadMessage.innerHTML = "loading...";

                var element = TreeDisplayUtil.getElementById(parentId);
                if (element != null) {
                    element.appendChild(loadMessage);
                }

                // Call our data gathering function. It's async,
                // so we'll need to pass a callback function
                // for rendering the resulting data set. We also
                // pass along the parent id (parentId) as an argument
                // for the callback method.
                getChildContents(tree.node.id, TreeUtil.addChildren, parentId);

                break;

            default:
                // we get three arguments, but the 3rd (treeViewId) does not apply here and is therefore ignored
                var children = arguments[0];
                var parentId = arguments[1];

                // remove the "loading..." message
                var element = document.getElementById("load" + parentId);
                if (element) {
                    var parent = element.parentNode;
                    parent.removeChild(element);
                }

                var tree = TreeUtil.getTreeById(parentId);

                LogUtil.addMessage(LogUtil.DEBUG,
						"TreeUtil",
						"addChildren( children, " + parentId + " )",
						"starting...");

                if (children != null) {
                    // for each child, make a new node, and append it to tree
                    for (var i = 0; i < children.assets.length; i++) {
                        var child = children.assets[i];
                        tree.appendNode(new Tree(child.name, child.id, parentId, child, 0));
                    }
                }

                TreeDisplayUtil.renderChildren(tree);
                TreeDisplayUtil.showChildren(tree);

                break;
        }
    },

    addChildrenEx: function(parentId, onComplete, vargs) {
        var tree = this.getTreeById(parentId);

        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeUtil",
				"addChildrenEx(" + parentId + ")",
				"starting...");

        // Each tree node defines the method in which it
        // obtains its child data. Get its action here.
        var getChildContents = tree.node.action;

        // display loading message
        var loadMessage = document.createElement("div");
        loadMessage.id = "load" + parentId;
        loadMessage.className = "loadingMessage";
        loadMessage.innerHTML = "loading...";


        var element = TreeDisplayUtil.getElementById(parentId);
        element.appendChild(loadMessage);

        // Call our data gathering function. It's async,
        // so we'll need to pass a callback function
        // for rendering the resulting data set. We also
        // pass along the parent id (parentId) as an argument
        // for the callback method.

        /* getChildContents == tree.node.action == toolkit.getChildFolders( id, callback, args ); */
        getChildContents(tree.node.id, TreeUtil.appendChildren, [parentId, onComplete, vargs]);
    },

    getTreeId: function(id) {
        return "T" + id;
    },

    getChildId: function(id) {
        return "C" + id;
    },

    getTreeById: function(id) {
        return TREES[this.getTreeId(id)]
    }
}

var clickedElementPrevious = null;
var clickedIdPrevious = null;

function onDragEnterHandler(id, element) {
    folderID = id;

    if (clickedElementPrevious != null) {
        clickedElementPrevious.style["background"] = "#ffffff";
        clickedElementPrevious.style["color"] = "#000000";
    }

    element.style["background"] = "#3366CC";
    element.style["color"] = "#ffffff";
}

function onMouseOverHandler(id, element) {
    element.style["background"] = "#ffffff";
    element.style["color"] = "#000000";
}

function onDragLeaveHandler(id, element) {
    element.style["background"] = "#ffffff";
    element.style["color"] = "#000000";
}

function onFolderClick(id, clickedElement) {
    if (clickedElementPrevious != null) {
        clickedElementPrevious.style["background"] = "#ffffff";
        clickedElementPrevious.style["color"] = "#000000";
    }

    clickedElement.style["background"] = "#3366CC";
    clickedElement.style["color"] = "#ffffff";
    clickedElementPrevious = clickedElement;
    clickedIdPrevious = id;

    var name = clickedElement.innerText;
    var folder = new Asset();
    folder.set("name", name);
    folder.set("id", id);

    Explorer.setWorkingFolder(folder);
    SearchManager.execute("id=" + id);
}

function onToggleClick(id, callback, args) {
    toolkit.getAllSubCategory(id, -99, callback, args);
}

function makeElementEditable(element) {
    element.contentEditable = true;
    element.focus();
    element.style.background = "#fff";
    element.style.color = "#000";
}

var treeMenu = new ContextMenu("treeMenu");
treeMenu.addItem("Properties", function(node) { Explorer.showFolderPropertiesWindow(node.id); });
ContextMenuUtil.add(treeMenu);

var baseUrl = URLUtil.getAppRoot(document.location) + "images/ui/icons/tree/";
TreeDisplayUtil.plusclosefolder = baseUrl + "folderCollapsed.png";
TreeDisplayUtil.plusopenfolder = baseUrl + "folderCollapsed.png";
TreeDisplayUtil.minusclosefolder = baseUrl + "folderExpanded.png";
TreeDisplayUtil.minusopenfolder = baseUrl + "folderExpanded.png";
TreeDisplayUtil.folder = baseUrl + "folder.png";
