//////////
//
// name: Tree
// desc: General purpose tree data structure
// auth: William Cava <william.cava@ektron.com>
// date: May 2005
//

function Tree(text, id, parentId, data, treeViewId)
{
    this.treeViewId = treeViewId;
    this.node = new Node(text, id, parentId, data);
    this.children = new Array();

    this.appendNode = function(node)
    {
        this.children[this.children.length] = node;
    }

    // keep around reference
    var treeId = TreeUtil.getTreeId(id, treeViewId);
    TREES[treeId] = this;
}

//////////
//
// name: Node
// desc: Node element of the tree
// auth: William Cava <william.cava@ektron.com>
// date: May 2005
//

function Node(name, id, parentId, data)
{
    this.name = name;
    this.action = GetToggleHandler(data.type);
    this.iconOp = null;
    this.iconCl = null;
    this.id = id;
    this.parentId = parentId;
    this.data = data;
    this.status = "closed";
    this.renderedSelf = false;
    this.renderedChildren = false;

    if (typeof data.langid != 'undefined') {
        this.langid = data.langid;
    }

    this.setAction = function(actn) { this.action = actn; }
    this.setOpenIcon = function(icon) { this.iconOp = icon; }
    this.setCloseIcon = function(icon) { this.iconCl = icon; }
}

function GetToggleHandler(type)
{
    switch (type)
    {
        case 10: // Taxonomies
            return onTaxonomyToggleClick;
        case 11: // Collections
            return null;
        case 12: // Menus
            return onMenuToggleClick;
        default:
            return onFolderToggleClick;
    }
}

var TREES = new Array();

//////////
//
// name: TreeDisplayUtil
// desc: A static utility class for displaying a Tree object
// auth: William Cava <william.cava@ektron.com>
// date: May 2005
//

var TreeDisplayUtil =
{
    //////////
    //
    // name: getAncestry
    // desc: Given an id, this function returns the ancestry for
    //       the node, starting the node itself, tracing back
    //       to the root node. It returns an array of tree nodes.
    //

    getAncestry: function(id, ancestry, treeViewId)
    {
        ancestry = ancestry != null ? ancestry : [];
        var tree = TreeUtil.getTreeById(id);

        if (tree != null)
        {
            ancestry[ancestry.length] = tree;
            var id = tree.node.id;
            var parentId = TreeDisplayUtil.getParentId(id);
            if (parentId != -1) 
            {
                return TreeDisplayUtil.getAncestry(parentId, ancestry, treeViewId);
            }
            else
            {
                return ancestry;
            }
        }

        return null;
    },

    //////////
    //
    // name: getRootNode
    // desc: Given an id, this function returns its root. If the passed
    //       id represents a tree not seen, the function returns null.
    //       If it represents the root node, the function returns its id.
    //       Otherwise, it recurses until it finds the root.
    //

    getRootNodeId: function(id, treeViewId)
    {
        var ancestry = TreeDisplayUtil.getAncestry(id, treeViewId);
        var rootNodeId = null;

        if (ancestry != null)
        {
            if (ancestry.length > 0)
            {
                var root = ancestry[ancestry.length - 1];
                rootNodeId = root.node.id;
            }
        }

        return rootNodeId;
    },

    //////////
    //
    // name: getParentId
    // desc: Given an id, this function returns its parent id. If the
    //       passed id represents a tree not seen, the function returns
    //       null. If the id represents the root node (who doesn't have
    //       a parent) the function returns -1. Otherwise, it returns
    //       the parent id.
    //

    getParentId: function(id, treeViewId)
    {
        var tree = TreeUtil.getTreeById(id, treeViewId);
        var parentId = null;

        if (tree != null)
        {
            parentId = -1;
            if (tree.node.parentId != null)
            {
                parentId = tree.node.parentId;
            }
        }

        return parentId;
    },

    //////////
    //
    // name: getElementById
    // desc: Given an id, returns the document element for that item
    //

    getElementById: function(id, treeViewId)
    {
        var id = TreeUtil.getTreeId(id, treeViewId);
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"getElementById(" + id + ")",
				"starting...");

        return document.getElementById(id);
    },

    //////////
    //
    // name: reloadTree
    // desc: Given an id, this function forces a reload of a subtree
    //

    reloadTree: function(id, treeViewId, langCode)
    {
        var tree = TreeUtil.getTreeById(id, treeViewId);

        if (tree != null)
        {
            if (tree.node.renderedChildren == true)
            {
                TreeDisplayUtil.removeTree(id, treeViewId);
            }
            TreeDisplayUtil.expandTree(tree, langCode);
        }
    },

    //////////
    //
    // name: removeTree
    // desc: Given an id, this function removes a subtree from the display
    //

    removeTree: function(id, treeViewId)
    {
        var tree = TreeUtil.getTreeById(id, treeViewId);

        if (tree != null)
        {
            // Set the renderedChildren flag to false
            tree.node.renderedChildren = false;

            // Remove the children from the DOM
            var containerId = TreeUtil.getChildId(id, treeViewId);
            var containerElement = document.getElementById(containerId);
            containerElement.parentNode.removeChild(containerElement);

            // This is where tree data is cached. Remove the child data from it.
            var treeId = TreeUtil.getTreeId(tree.node.id, treeViewId);
            TREES[treeId].children = [];
        }
    },

    //////////
    //
    // name: reloadParentTree
    // desc: Given an id, this function forces the load of a parent subtree
    //

    reloadParentTree: function(id, treeViewId)
    {
        var tree = TreeUtil.getTreeById(id, treeViewId);

        if (tree != null)
        {
            var parentId = tree.node.parentId;
            TreeDisplayUtil.reloadTree(parentId, treeViewId);
        }
    },

    //////////
    //
    // name: getChildElementById
    // desc: Given an id, this function returns the child document element
    //

    getChildElementById: function(tree)
    {
        var treeNodeId = tree.node.id;
        var treeViewId = tree.treeViewId;
        var childId = TreeUtil.getChildId(treeNodeId, treeViewId);
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"getChildElementById(" + childId + ")",
				"starting...");

        return document.getElementById(childId);
    },

    //////////
    //
    // name: toggleTree
    // desc: If a node is open, close it. Otherwise, open it.
    //
    toggleTree: function(parentId, treeViewId)
    {
        var tree = TreeUtil.getTreeById(parentId, treeViewId);

        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"toggleTree(" + parentId + ")",
				"tree.node.status = " + tree.node.status);

        if (tree.node.status == "open")
        {
            TreeDisplayUtil.collapseTree(tree);
        }
        else
        {
            TreeDisplayUtil.expandTree(tree, -99);
        }
    },
    //////////
    //
    // name: expandTreeSet
    // desc: expandTreeSet accepts a set of ids and asynchronously & recursively
    //       expands them. The ids must be formatted in the following way:
    //       ids[0]   -> root
    //       its[1]   -> first-level
    //        ...
    //       ids[n-3] -> grandparent
    //       ids[n-2] -> parent
    //       ids[n-1] -> self
    //
    //       e.g.:
    //
    //       TreeDisplayUtil.expandTreeSet( [0, 23, 45, 47, 76] );
    //       TreeDisplayUtil.expandTreeSet( new Array( "0, 23, 45, 47, 76" ) );
    //

    expandTreeSet: function(ids)
    {
        return TreeDisplayUtil.__expandTreeSet([ids], 0);
    },

    //////////
    //
    // name: toggleTreeSet
    // desc: toggleTreeSet accepts a set of ids and asynchronously & recursively
    //       toggles them. The ids must be formatted in the following way:
    //       ids[0]   -> root
    //       its[1]   -> first-level
    //        ...
    //       ids[n-3] -> grandparent
    //       ids[n-2] -> parent
    //       ids[n-1] -> self
    //
    //       e.g.:
    //
    //       TreeDisplayUtil.toggleTreeSet( [0, 23, 45, 47, 76] );
    //       TreeDisplayUtil.toggleTreeSet( new Array( "0, 23, 45, 47, 76" ) );
    //

    toggleTreeSet: function (ids, treeViewId) {
        var retVal = false;

        if (ids.length > 0) {

            var tree = TreeUtil.getTreeById(ids[0], treeViewId);
            if (tree == null) {
                setTimeout(function () { TreeDisplayUtil.toggleTreeSet(ids, treeViewId) }, 750);
            }
            else {
                TreeDisplayUtil.toggleTree(ids[0], treeViewId);
                if (ids.length > 1) {
                    ids = ids.slice(1);
                    if (ids.length > 0) {
                        setTimeout(function () { TreeDisplayUtil.toggleTreeSet(ids, treeViewId) }, 750);
                    }
                }
            }
        }

        return (retVal);
    },

    //////////
    //
    // name: __expandTreeSet
    // desc: __expandTreeSet is a private method that does the work for
    //		 expandTreeSet. This function handles the asynchronous calls
    //       and the oncomplete event handling. Upon oncomplete, it recursively
    //       calls itself.
    //

    __expandTreeSet: function(vargs, treeViewId)
    {
        var retVal = false;
        var ids = [];
        if (vargs.length > 0)
        {
            ids = vargs[0];
        }

        var currentIndex = 0;
        if (vargs.length > 1)
        {
            currentIndex = vargs[1];
            currentIndex++;
        }

        onComplete = TreeDisplayUtil.expandTreeSet;
        if (ids.length > currentIndex)
        {
            var id = ids[currentIndex];
            if (id != null)
            {
                var vargs = [ids, currentIndex, onComplete];
                var tree = TreeUtil.getTreeById(id, treeViewId);
                if (tree != null && tree.node.renderedChildren == true)
                {
                    retVal = TreeDisplayUtil.showChildren(tree);
                    if (retVal)
                    {
                        retVal = TreeDisplayUtil.__expandTreeSet(vargs, treeViewId);
                    }
                }
                else
                {
                    retVal = TreeDisplayUtil.expandTreeById(id, TreeDisplayUtil.__expandTreeSet, vargs, treeViewId);
                }
            }
			if (retVal && ids.length > 1)
            {   
                var onCompleteArgs = [];
                for (var i = 1; i < ids.length; i++) {
                    if (ids[i] == id) continue;
                    onCompleteArgs[i - 1] = ids[i];
                }
                setTimeout(function(){TreeDisplayUtil.expandTreeSet(onCompleteArgs)},750);
            }
        }
        else
        {
            // reached end of id array, signal success:
            retVal = true;
        }

        return (retVal);
    },
    //////////
    //
    // name: toggleTree
    // desc: If a node hasn't been opened before, fetch data and display it
    //

    expandTree: function(tree, langCode)
    {
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"expandTree(" + tree.node.id + ")",
				"starting...");

        // the "tree.node.id == 0" is a workaround for a bug on in the ContentService
        // that has the root element returning hasChildren = false.
        if ((tree.node.data.hasChildren == "true") || (tree.node.id == 0))
        {
            if (!tree.node.renderedChildren)
            {
                tree.node.renderedChildren = true;
                TreeUtil.addChildren(tree.node.id, tree.treeViewId, -99, langCode);
            }
            else
            {
                TreeDisplayUtil.showChildren(tree);
            }
        }
    },

    expandTreeEx: function(tree, onComplete, vargs, treeViewId)
    {
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"expandTreeEx(" + tree.node.id + ")",
				"starting...");

        // the "tree.node.id == 0" is a workaround for a bug on in the ContentService
        // that has the root element returning hasChildren = false.
        if ((tree.node.data.hasChildren == "true") || (tree.node.id == 0))
        {
            if (!tree.node.renderedChildren)
            {
                tree.node.renderedChildren = true;
                TreeUtil.addChildrenEx(tree.node.id, onComplete, vargs, treeViewId);
            }
            else
            {
                TreeDisplayUtil.showChildren(tree);
            }
        }
    },

    //////////
    //
    // name: expandTreeById
    // desc: Wrapper around expandTree, which expects a tree object
    //

    expandTreeById: function(id, onComplete, vargs, treeViewId, langId)
    {
        var retVal = false;
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"expandTreeById(" + id + ")",
				"starting...");

        var tree = TreeUtil.getTreeById(id, treeViewId);
        if (tree)
        {
            TreeDisplayUtil.expandTree(tree, langId);
            retVal = true;
        }
        else
        {
            retVal = false;
        }
        return (retVal);
    },

    //////////
    //
    // name: collapseTree
    // desc: hides a subtree
    //
    collapseTree: function(tree)
    {
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"collapseTree(" + tree.node.id + ")",
				"starting...");

        if (tree.node.data.hasChildren)
        {
            TreeDisplayUtil.hideChildren(tree);
        }
    },

    hideChildren: function(tree)
    {
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"hideChildren(" + tree.node.id + ")",
				"starting...");

        var element = TreeDisplayUtil.getChildElementById(tree);
        if (element == null)
        {
            return; // Happens when loading the root node
        }

        element.style["display"] = "none";
        tree.node.status = "closed";

        var iconId = TreeUtil.getImageId(tree.node.id, tree.treeViewId);
        var imageElement = document.getElementById(iconId);
        if (imageElement)
        {
            imageElement.src = TreeDisplayUtil.getCollapsedImage(tree.node.data);
        }
    },

    showChildren: function(tree)
    {
        var retVal = false;
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"showChildren(" + tree.node.id + ")",
				"starting...");

        var iconId = TreeUtil.getImageId(tree.node.id, tree.treeViewId);
        var imageElement = document.getElementById(iconId);

        var childElement = TreeDisplayUtil.getChildElementById(tree);
        if (!childElement)
        {
            retVal = false;
            LogUtil.addMessage(LogUtil.CRITICAL,
					"TreeDisplayUtil",
					"showChildren(" + tree.node.id + ")",
					"Couldn't find container for child element");
        }
        else
        {
            childElement.style["display"] = "";

            if (imageElement)
            {
                imageElement.src = TreeDisplayUtil.getExpandedImage(tree.node.data);
            }

            tree.node.status = "open";
            retVal = true;
        }

        return retVal;
    },

    showSelf: function(tree, container)
    {
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"showSelf(" + tree.node.id + ")",
				"starting...");
        if (!tree.node.rendered)
        {
            TreeDisplayUtil.renderSelf(tree, container);
        }

        var element = TreeDisplayUtil.getElementById(tree.node.id, tree.treeViewId);
        element.style["display"] = "";
        tree.node.status = "closed";
    },

    //////////
    //
    // name: renderChildren
    // desc: Given a tree, iterate through children and build up display
    //

    renderChildren: function(tree)
    {
        var treeViewId = tree.treeViewId;
        var treeNodeId = tree.node.id;
        var childContainerId = TreeUtil.getChildId(treeNodeId, treeViewId);

        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"renderChildren(" + childContainerId + ")",
				"starting...");

        var ul = document.createElement("ul");
        ul.setAttribute("className", "ekSubTree");
        ul.setAttribute("id", childContainerId);

        var numberChildren = tree.children.length;
        for (var i = 0; i < numberChildren; i++)
        {
            var child = tree.children[i];

            var li = document.createElement("li");
            li.setAttribute("className", "ekTreeItem");
            li.setAttribute("id", TreeUtil.getTreeId(child.node.id, treeViewId));

            li.innerHTML = TreeDisplayUtil.createNodeHTML(child);
            ul.appendChild(li);
            child.node.rendered = true;
        }
        tree.node.renderedChildren = true;

        var treeId = TreeUtil.getTreeId(treeNodeId, treeViewId);
        var parentElement = document.getElementById(treeId);
        if (parentElement != null)
        {
            parentElement.appendChild(ul);
        }
    },

    displayContents: function(id, clickedElement, treeViewId, langId)
    {
        onFolderNodeClick(id, clickedElement, treeViewId, langId);
    },

    //////////
    //
    // name: renderSelf
    // desc: Given a tree, display it.
    //

    renderSelf: function(tree, container)
    {
        var src = "";
        var treeNodeId = tree.node.id;
        var treeViewId = tree.treeViewId;

        if (tree.node.data.hasChildren == "true")
        {
            src = TreeDisplayUtil.getCollapsedImage(tree.node.data);
            }
            else
            {
            src = TreeDisplayUtil.getNoChildImage(tree.node.data);
        }
        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeDisplayUtil",
				"renderSelf(" + tree.node.id + ")",
				"starting...");

        var ul = document.createElement("ul");
        ul.setAttribute("className", "ektree " + FrameName);
        var li = document.createElement("li");
        li.setAttribute("className", "ekTreeRootItem");
        li.setAttribute("id", TreeUtil.getTreeId(treeNodeId, treeViewId));

        var imageId = TreeUtil.getImageId(treeNodeId, treeViewId);
        var linkId = TreeUtil.getLinkId(treeNodeId, treeViewId);

        var buffer = "<img id='" + imageId + "' onclick='TreeDisplayUtil.toggleTree(" + treeNodeId + ", " + treeViewId + ")' class='hasChildIcon' src='" + src + "'>";
        
        if (FrameName == 'Library')
        {
            if (tree.node.data.grantReadLib == "true")
            {
                buffer += "<span onclick='TreeDisplayUtil.expandTreeById(" + treeNodeId + ",[],[], " + treeViewId + ")'>";
                buffer += "<a id='" + linkId + "' href='#' onclick='TreeDisplayUtil.displayContents(" + treeNodeId + ", this, " + treeViewId + ");return false;'>";
                buffer += tree.node.name;
                buffer += "</a>"
                buffer += "</span>";
            }
            else
            {
                buffer += tree.node.name;
            }
            buffer += ""
        }
        else
        {
            buffer += "<span onclick='TreeDisplayUtil.expandTreeById(" + treeNodeId + ",[],[], " + treeViewId
            if (typeof tree.node.langid != 'undefined') {
                buffer += ", " + tree.node.langid;
            }
            buffer += ")'>";
            buffer += "<a id='" + linkId + "' href='#' onclick='TreeDisplayUtil.displayContents(" + treeNodeId + ", this, " + treeViewId;
            if (typeof tree.node.langid != 'undefined') {
                buffer += ", " + tree.node.langid;
            }
            buffer += ");return false;'";
            switch (treeViewId) {
                case -1:    // taxonomy
                    buffer += " data-ektron-datatype=10";
                    break
                case -2:    // collection
                    buffer += " data-ektron-datatype=11";
                    break;
                case -3:    // menu
                    buffer += " data-ektron-datatype=12";
                    break;
            }
            buffer += " data-ektron-folderid=" + treeNodeId + "";
            buffer += ">";
            buffer += tree.node.name;
            buffer += "</a>";
            buffer += "</span>";
        }

        li.innerHTML = buffer
        ul.appendChild(li);

        if (!container)
        {
            container = document.getElementById("TreeOutput");
        }

        if (ul.outerHTML == 'undefined')
        {
            container.innerHTML = ul.outerHTML;
        }
        else
        {
            container.innerHTML = '<ul class="ektree ' + FrameName + '">' + ul.innerHTML + '</ul>';
        }

        tree.node.rendered = true;
    },

    createNodeHTML: function(tree)
    {
        var src = "";
        var treeNode = tree.node;
        var treeNodeId = treeNode.id;
        var treeNodeData = treeNode.data;
        var treeViewId = tree.treeViewId;

        if (treeNodeData.hasChildren == "true")
        {
            src = TreeDisplayUtil.getCollapsedImage(treeNodeData);
            }
            else
            {
            src = TreeDisplayUtil.getNoChildImage(treeNodeData);
        }

        var imageId = TreeUtil.getImageId(tree.node.id, tree.treeViewId);
        var buffer = "<img onclick='TreeDisplayUtil.toggleTree(" + treeNodeId + ", " + treeViewId + ")' class='hasChildIcon' id='" + imageId + "' src='" + src + "'>";

        buffer += "<span onclick='TreeDisplayUtil.expandTreeById(" + treeNodeId + ",[],[], " + treeViewId
        if (typeof treeNodeData.langid != 'undefined') {
            buffer += ", " + treeNodeData.langid;
        }
        buffer += ")'>";

        //var onclickDispatch = "return false;"
        //if( tree.node.data.canRead == "true" ) {
        //	onclickDispatch = "TreeDisplayUtil.displayContents(" + treeNode.id + ", this);return false;"
        //}
        //ONLY CHANGES FOR CMS

        // TODO: Ross - Top condition is temporary, for taxonomy and collections
        if (FrameName == "Content" && treeNodeData.grantRead == null ||
            FrameName == "Content" && treeNodeData.grantRead == "true" ||
            FrameName == "Tax" ||
			FrameName == "Menu" ||
            FrameName == "Coll" ||
            FrameName == "Forms" && treeNodeData.grantRead == "true" ||
            FrameName == "Library" && treeNodeData.grantReadLib == "true")
        {
            var linkId = TreeUtil.getLinkId(treeNodeId, treeViewId);
            buffer += "<a href='#' id='" + linkId + "'";
            if (typeof treeNodeData.langid != 'undefined')
            {
                // add language type if we need it for the treenodes
                buffer += " data-ektron-languagetype=" + treeNodeData.langid;
            }
            switch ((treeNodeData.type).toString())
            {
                case "0":   // regular content folder
                case "3":   // board folder
                case "4":
                    buffer += " data-ektron-parentid='" + treeNodeData.parentid + "'";
                case "10":
                    break;
                case "11":
                    buffer += " data-ektron-folderid='" + treeNodeData.folderid + "'";
                    buffer += " data-ektron-status='" + treeNodeData.status + "'";
                    buffer += " data-ektron-approvalrequired='" + treeNodeData.approvalRequired + "'";
                    break;
                case "12":
                    buffer += " data-ektron-folderid='" + treeNodeData.folderid + "'";
                    buffer += " data-ektron-parentid='" + treeNodeData.parentid + "'";
                    buffer += " data-ektron-itemcount='" + treeNodeData.itemCount + "'";
                    buffer += " data-ektron-ancestorid='" + treeNodeData.ancestorid + "'";
                    break;
                default:
                    break;
            }

            buffer += " data-ektron-folderid=" + treeNodeId + "";
            buffer += " data-ektron-datatype='" + treeNodeData.type
            + "' onclick='TreeDisplayUtil.displayContents(" + treeNodeId + ", this, " + treeViewId;
            if (typeof treeNodeData.langid != 'undefined') {
                buffer += ", " + treeNodeData.langid;
            }
            buffer += ");return false;'>" + tree.node.name + "</a>";
        }
        else
        {
            buffer += treeNode.name;
        }
        //buffer += "<a href='#' oncontextmenu='return onContextMenuHandler(" + tree.node.id + ", this)' onclick='" + onclickDispatch + "' onmouseover='onMouseOverHandler(" + tree.node.id + ",this)' ondragleave='onDragLeaveHandler(" + tree.node.id + ",this)' ondragenter='onDragEnterHandler(" + tree.node.id + ",this);event.returnValue=false' ondragover='event.returnValue=false' id=" + tree.node.id + ">";
        //buffer += tree.node.name;
        //buffer += "</a>"

        buffer += "</span>";

        return buffer;
    },

    getExpandedImage: function(nodedata)
    {
        var src;
        if (TreeDisplayUtil.minusclosefolders == undefined)
        {
            src = TreeDisplayUtil.minusclosefolder;
        }
        else
        {
            src = TreeDisplayUtil.minusclosefolders[nodedata.type];
            if (nodedata.type == 10) {
                // special handling for taxonomy
                if (nodedata.visible == "false") {
                    src = src.replace("taxonomyExpanded.png", "taxonomyExpandedDisabled.png");
                }
            }
        }
        return src;
    },

    getCollapsedImage: function(nodedata)
    {
        var src;
        if (TreeDisplayUtil.plusclosefolders == undefined)
        {
            src = TreeDisplayUtil.plusclosefolder;
        }
        else
        {
            src = TreeDisplayUtil.plusclosefolders[nodedata.type];
            if (nodedata.type == 10) {

                // special handling for taxonomy
                if (nodedata.visible == "false") {
                    src = src.replace("taxonomyCollapsed.png", "taxonomyCollapsedDisabled.png");
                }
            }
        }
        return src;
    },

    getNoChildImage: function(nodedata)
    {
        var src;
        if (TreeDisplayUtil.folders == undefined)
        {
            src = TreeDisplayUtil.folder;
        }
        else
        {
            src = TreeDisplayUtil.folders[nodedata.type];
            if (nodedata.type == 10) {
                // special handling for taxonomy
                if (nodedata.visible == "false") {
                    src = src.replace("taxonomy.png", "taxonomyDisabled.png");
                }
            }
        }
        return src;
    }
}

//////////
//
// name: TreeUtil
// desc: A static utility class for manipulating a Tree object
// auth: William Cava <william.cava@ektron.com>
// date: May 2005
//

var TREE_VIEW_ID_PREFIX = "_tv";

var TreeUtil =
{
    appendChildren: function(children, vargs, treeViewId)
    {
        // This arguments are passed above in addChildren via the callback
        var parentId = vargs[0];
        var onComplete = vargs[1];
        var onCompleteArgs = vargs[2];

        var element = document.getElementById("load" + parentId);
        if (element)
        {
            var parent = element.parentNode;
            parent.removeChild(element);
        }


        var tree = TreeUtil.getTreeById(parentId, treeViewId);

        LogUtil.addMessage(LogUtil.DEBUG,
				"TreeUtil",
				"appendChildren( children, " + parentId + ", onCompleteCallback )",
				"starting...");

        TreeUtil.addChild(tree, children, parentId);

        TreeDisplayUtil.renderChildren(tree);
        TreeDisplayUtil.showChildren(tree);
        if (onComplete != null)
        {
            onComplete(onCompleteArgs);
        }
        else
        {
            LogUtil.addMessage(LogUtil.WARNING,
				"TreeUtil",
				"appendChildren( children, " + parentId + ", onCompleteCallback )",
				"failed to call onComplete callback (probably not specified, since not required)");
        }
    },

    addChild: function(tree, children, parentId)
    {
        if (children != null)
        {
            // for each child, make a new node, and append it to tree
            for (var i = 0; i < children.assets.length; i++)
            {
                var child = children.assets[i];
                if (child.type == null)
                {
                    // For non-folder trees (taxonomy, collection, etc.) set the type to that of the root node
                    child.type = tree.node.data.type;
                }
                tree.appendNode(new Tree(child.name, child.id, parentId, child, tree.treeViewId));
            }
        }
    },

    //////////
    //
    // name: addChildren
    // desc: This method is overloaded in the following way:
    //			addChildren( parentId, treeViewId );    // normal call
    //			addChildren( children, parentId, treeViewId );  // for callback from ajax call
    //			addChildren( parentId, treeViewId, -99, langCode ); // for forcing a language change...4 params
    //
    //		addChildren( parentId ): When given just a parent id, this function
    //		calls the data gathering function defined for the node
    //		to obtain its child data. Once the data gathering function
    //		has completed, it calls the call back function for
    //		inserting its results into the tree. In our case, we call
    //		addChildren( children, parentId ).
    //
    //		addChildren( children, parentId ): Given a set of child nodes and a
    //		parent id, this function iterates through the set of child
    //		nodes and inserts them as children of the parentNode (node
    //		with id = parentId).
    //

    addChildren: function()
    {
        var arglength = arguments.length;
        var langid = -99;   // -99 in the ajax code is "use langid in cookies"
        // special handling for language code
        if ((arglength == 4) && (arguments[2] == -99)) {
          if (arguments[3] != undefined) {
            langid = arguments[3];
          }
          arglength = 2;
        }
        switch (arglength)
        {
            case 2:
                // we get one argument, the parent id
                var parentId = arguments[0];
                var treeViewId = arguments[1];
                var tree = this.getTreeById(parentId, treeViewId);

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

                var element = TreeDisplayUtil.getElementById(parentId, treeViewId);
                element.appendChild(loadMessage);

                // Call our data gathering function. It's async,
                // so we'll need to pass a callback function
                // for rendering the resulting data set. We also
                // pass along the parent id (parentId) as an argument
                // for the callback method.
                getChildContents(tree.node.id, langid, TreeUtil.addChildren, parentId);

                break;

            default:
                // this handles the callback from the ajax call above
                // we get two arguments
                var children = arguments[0];
                var parentId = arguments[1];
                var treeViewId = arguments[2];

                // remove the "loading..." message
                var element = document.getElementById("load" + parentId);
                if (element)
                {
                    var parent = element.parentNode;
                    parent.removeChild(element);
                }

                var tree = TreeUtil.getTreeById(parentId, treeViewId);

                LogUtil.addMessage(LogUtil.DEBUG,
						"TreeUtil",
						"addChildren( children, " + parentId + " )",
						"starting...");

                TreeUtil.addChild(tree, children, parentId);

                TreeDisplayUtil.renderChildren(tree);
                TreeDisplayUtil.showChildren(tree);

                break;
        }
    },

    addChildrenEx: function(parentId, onComplete, vargs, treeViewId)
    {
        var tree = this.getTreeById(parentId, treeViewId);

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


        var element = TreeDisplayUtil.getElementById(parentId, treeViewId);
        element.appendChild(loadMessage);

        // Call our data gathering function. It's async,
        // so we'll need to pass a callback function
        // for rendering the resulting data set. We also
        // pass along the parent id (parentId) as an argument
        // for the callback method.

        /* getChildContents == tree.node.action == toolkit.getChildFolders( id, callback, args ); */
        getChildContents(tree.node.id, TreeUtil.appendChildren, [parentId, onComplete, vargs]);
    },

    getTreeId: function(id, treeViewId)
    {
        return "T" + id + TREE_VIEW_ID_PREFIX + treeViewId;
    },

    getChildId: function(id, treeViewId)
    {
        return "C" + id + TREE_VIEW_ID_PREFIX + treeViewId;
    },

    getImageId: function(id, treeViewId)
    {
        return "I" + id + TREE_VIEW_ID_PREFIX + treeViewId;
    },

    getLinkId: function(id, treeViewId)
    {
        return "L" + id + TREE_VIEW_ID_PREFIX + treeViewId;
    },

    getTreeById: function(id, treeViewId)
    {
        if (typeof(treeViewId) == "undefined") {
            // for backwards compatibility w/ code that used it w/ one arg
            treeViewId = 0;
        }
        var treeId = this.getTreeId(id, treeViewId);
        return TREES[treeId];
    }
}

//////////
//
// The click handlers for the trees. These should be placed
// in an external file, I'm just throwing them in here for now,
// since we're only using the tree in once place.
//

var clickedElementPrevious = null;
var clickedIdPrevious = null;

function onContextMenuHandler(id, clickedElement)
{
    // todo: create a 'highlight node' function
    /*
    if( clickedElementPrevious != null ) {
    clickedElementPrevious.style["background"] = "#ffffff";
    clickedElementPrevious.style["color"] = "#000000";
    }

	clickedElement.style["background"] = "#3366CC";
    clickedElement.style["color"] = "#ffffff";
    clickedElementPrevious = clickedElement;
    clickedIdPrevious = id;

	ContextMenuUtil.use( event, "treeMenu", clickedElement );

	return false;
    */
}

function onDragEnterHandler(id, element)
{
    folderID = id;

    // todo: create a 'highlight node' function
    if (clickedElementPrevious != null)
    {
        clickedElementPrevious.style["background"] = "#ffffff";
        clickedElementPrevious.style["color"] = "#000000";
    }

    element.style["background"] = "#3366CC";
    element.style["color"] = "#ffffff";
}

//setInterval( function() { alert( folderID ) } , 5000 ) ;

function onMouseOverHandler(id, element)
{
    element.style["background"] = "#ffffff";
    element.style["color"] = "#000000";
}

function onDragLeaveHandler(id, element)
{
    element.style["background"] = "#ffffff";
    element.style["color"] = "#000000";
}

function onFolderClick(id, clickedElement, treeViewId)
{
    // todo: create a 'highlight node' function
    if (clickedElementPrevious != null)
    {
        //if( clickedIdPrevious == id ) {
        //	return;
        //}
        clickedElementPrevious.style["background"] = "#ffffff";
        clickedElementPrevious.style["color"] = "#000000";
    }

    clickedElement.className = "selectedNode";
    clickedElementPrevious = clickedElement;
    clickedIdPrevious = id;

    var name = clickedElement.innerText;
    var folder = new Asset();
    folder.set("name", name);
    folder.set("id", id);

    Explorer.setWorkingFolder(folder);
    SearchManager.execute("id=" + id);
}

function onFolderToggleClick(id, langid, callback, args)
{
    toolkit.getChildFolders(id, langid, callback, args);
}

function onTaxonomyToggleClick(id, langid, callback, args)
{
    toolkit.getAllSubCategory(id, langid, callback, args);
}

function onMenuToggleClick(id, langid, callback, args)
{
    toolkit.getSubMenus(id, langid, callback, args);
}

function makeElementEditable(element)
{
    element.contentEditable = true;
    element.focus();
    element.style.background = "#fff";
    element.style.color = "#000";
}

//////////
//
// The following defines what the context menu will look
// like when fired from the tree
//

var treeMenu = new ContextMenu("treeMenu");
//treeMenu.addItem( "Rename", function( node ){ makeElementEditable( node ) } );
//treeMenu.addItem( "Delete", function(){alert("not implemented")} );
//treeMenu.addBreak();
treeMenu.addItem("Properties", function(node) { Explorer.showFolderPropertiesWindow(node.id); });
ContextMenuUtil.add(treeMenu);

//////////
//
// Define the default images for the tree
//

var baseUrl = "images/ui/icons/tree/";
TreeDisplayUtil.plusclosefolder = baseUrl + "folderCollapsed.png";
TreeDisplayUtil.plusopenfolder = baseUrl + "folderCollapsed.png";
TreeDisplayUtil.minusclosefolder = baseUrl + "folderExpanded.png";
TreeDisplayUtil.minusopenfolder = baseUrl + "folderExpanded.png";
TreeDisplayUtil.folder = baseUrl + "folder.png";

// define these variables if you're going to use multiple folder images
// currently, 0 = default, 1 = blog, 2 = domain folder
// TreeDisplayUtil.plusclosefolders = new Array(3);
// TreeDisplayUtil.plusopenfolders = new Array(3);
// TreeDisplayUtil.minusclosefolders = new Array(3);
// TreeDisplayUtil.minusopenfolders = new Array(3);
// TreeDisplayUtil.folders = new Array(3);
// only .plusclosefolder, .minusclosefolder, and .folder images seem to be used...
